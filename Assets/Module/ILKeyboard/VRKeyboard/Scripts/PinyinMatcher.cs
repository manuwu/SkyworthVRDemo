using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class PinyinMatcher
{
    private PinyinMatcher(){
    }

    static char stxChar = '\u0002';
    static string etxStr = '\u0003'.ToString();

    private static PinyinMatcher mInstance;

    public static PinyinMatcher Instance{
        get{
            if (mInstance == null) {
                mInstance = new PinyinMatcher (); 
            }
            return mInstance;
        }
    }

    public static bool isMatch(string input, string pattern)
    {
        List<string> inputList = getPolyPhone(input,true);
        if (inputList.Count < pattern.Length) {
            return false;
        }
        else {
            if(PlatformManager.mInstance.platform != PlatformManager.PLATFORM.JAPAN)
            {
                foreach (char _item in pattern)
                {
                    if (!input.Contains(_item.ToString()))
                    {
                        return false;
                    }
                }
            }

            int j = 0;
            int len = pattern.Length;
            for (int i = 0; i < inputList.Count; ++i)
            {
                if (((inputList[i].Contains(pattern[0].ToString()) && !inputList[i].Contains(pattern[j].ToString())) && j < len))
                {
                    j = 1;
                }
                else if (j < len)
                {
                    if (PlatformManager.mInstance.platform == PlatformManager.PLATFORM.JAPAN)
                    {
                        if((inputList[i].Contains(pattern[j].ToString()) || ValidStr(pattern[j], inputList[i])))
                        {
                            j++;
                        }
                        else
                        {
                            j = 0;
                        }
                    }
                    else
                    {
                        if (inputList[i].Contains(pattern[j].ToString()))
                        {
                            j++;
                        }
                        else
                        {
                            j = 0;
                        }
                    }
                }

                if (j == len)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public static bool isAtoZMatch(string input, string pattern)
    {
        List<string> inputList = getPolyPhone(input,true);
        if(inputList.Count == 0)
        {
            return false;
        }
        return inputList[0].StartsWith(pattern);
    }
    
    public static List<string> getPolyPhone(String input,bool allIn = false)
    {
        int i = 0;
        List<string> inputList = new List<string>();
        while (i < input.Length)
        {
            if (input[i] == stxChar)
            {
                string _tmp = input.Substring(i);
                if (string.IsNullOrEmpty(_tmp) || !_tmp.Contains(etxStr))
                {
                    i++;
                    continue;
                }
                string str = input.Substring(i + 1, input.Substring(i).IndexOf(etxStr) - 1);
                inputList.Add(str);
                i += str.Length + 1;
            }
            else if (allIn)
            {
                inputList.Add(input[i].ToString());
            }
            else if (input[i] >= 'A' && input[i] <= 'Z'|| input [i] >= '0' && input [i] <= '9')
            {
                inputList.Add(input[i].ToString());
            }
            i++;
        }
        return inputList;
    }

    public bool isDigitOrLetter(char str)
    {
        bool _restult = false;
        if (str >= 'A' && str <= 'Z' || str >= '0' && str <= '9')
        {
            _restult = true;
        }

        if (PlatformManager.mInstance.platform == PlatformManager.PLATFORM.JAPAN)
        {
            int uCode = (int)str;
            if (uCode == 12540 || uCode >= 12353 && uCode <= 12534)
            {
                _restult = true;
            }
        }
        return _restult;
    }

    public char FristChar(string nameCapitals)
    {
        char c = nameCapitals[0];
        foreach (char _item in nameCapitals)
        {
            if (_item.Equals(stxChar))
            {
                continue;
            }
            else
            {
                c = _item;
                break;
            }
        }
        return c;
    }

    #region 日文
    private static bool IsRealted(int uCode, int uVaildCode)
    {
        // 日文平假/片假unicode范围
        if (uCode >= 12353 && uCode <= 12438)
        {
            if (uVaildCode == uCode + 96)
            {
                return true;
            }
            else
                return false;
        }
        else if (uCode >= 12449 && uCode <= 12534)
        {
            if (uCode == uVaildCode + 96)
            {
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }

    private static bool ValidChar(char pattern,char input)
    {
        if (pattern.Equals(input))
        {
            return true;
        }
        int uCode = (int)pattern;
        int[] factor = null;
        //如果是日文，按照50音图
        // 拨音特殊
        if (uCode == 12540 || uCode >= 12353 && uCode <= 12534)
        {
            int uVaildCode = (int)input;
            // 全角,不能用==2，因为个别全角字符是3如：は，
            if (Encoding.Default.GetByteCount(pattern.ToString()) > 1)
            {
                // 4行3列（有3种：半角/全角清/全角浊） 
                // 全角平清 全角片清 全角平浊 全角片浊
                if (uCode == 12388 || uCode == 12389 || 
                    uCode == 12484 || uCode == 12485)
                {
                    return IsRealted(uCode, uVaildCode);
                }
                else if (uCode == 12434 || uCode == 12530 || 
                            uCode == 12435 || uCode == 12531 ||
                            (uCode >= 12394 && uCode <= 12398)|| (uCode >= 12490 && uCode <= 12494) ||
                            (uCode >= 12414 && uCode <= 12418) || (uCode >= 12510 && uCode <= 12514) ||
                            (uCode >= 12425 && uCode <= 12429) || (uCode >= 12521 && uCode <= 12525))
                {
                    // 10行5列、11行1列、 5、 7、 9行（只有第2种：全角清）
                    return IsRealted(uCode, uVaildCode);
                }
                else if ((uCode >= 12399 && uCode <= 12413) ||
                        (uCode >= 12495 && uCode <= 12509))
                {
                    // 6行(有3种)
                    return IsRealted(uCode, uVaildCode);
                }
                else
                {
                    //其余行都是只有1.2种
                    return IsRealted(uCode, uVaildCode);
                }
            }
            else
            {
                // 半角
                return IsRealted(uCode, uVaildCode);
            }
        }
        else
        {
            return false;
        }
    }

    private static bool ValidStr(char pattern, string inputStr)
    {
        if (PlatformManager.mInstance.platform == PlatformManager.PLATFORM.JAPAN)
        {
            for (int i = 0; i < inputStr.Length; ++i)
            {
                if (ValidChar(pattern, inputStr[i]))
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion
}




