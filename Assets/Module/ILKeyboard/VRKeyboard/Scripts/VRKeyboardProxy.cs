using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using IVRCommon.Keyboard.Enum;
using IVRCommon.Keyboard.Widet;
using IVRCommon.Keyboard.Action;

namespace IVRCommon.Keyboard
{
    public class KeyboardInfo
    {
        public KeyboardType type;
        public GameObject parent;
        public RectTransform rectTransform;
        public CanvasGroup canvasGroup;
        private List<KeyCodeButton> buttons;
        public List<KeyCodeButton> keys
        {
            get
            {
                return buttons;
            }
        }
        private bool isLower = true;
        public bool IsShow
        {
            get
            {
                return rectTransform.gameObject.activeInHierarchy;
            }
        }
        public KeyboardInfo(KeyboardType t, GameObject target)
        {
            type = t;
            parent = target;
            rectTransform = target.GetComponent<RectTransform>();
            canvasGroup = target.GetComponent<CanvasGroup>(); 
            buttons = new List<KeyCodeButton>();
        }

        public void AddButton(KeyCodeButton btn)
        {
            buttons.Add(btn);
        }

        public void AddButtons(KeyCodeButton[] btns)
        {
            if (btns == null)
                return;
            buttons.Clear();
            buttons.AddRange(btns);
        }

        public List<KeyCodeButton> GetButtons(KeyCodeType code)
        {
            List<KeyCodeButton> list = new List<KeyCodeButton>();
            for (int i = 0, max = buttons.Count; i < max; i++)
            {
                if (buttons[i].type == code)
                    list.Add(buttons[i]);
            }
            return list;
        }

        public List<KeyCodeButton> GetButtons(List<char> keys)
        {
            List<KeyCodeButton> list = new List<KeyCodeButton>();
            for (int i = 0, max = buttons.Count; i < max; i++)
            {
                if (keys.Contains(buttons[i].key))
                    list.Add(buttons[i]);
            }
            return list;
        }


        public void SwitchUpperOrLower()
        {
            SwitchLetters(!isLower);
        }

        public void SwitchLetters(bool lower)
        {
            if ((type & KeyboardType.Letter) == 0)
                return;
            isLower = lower;
            char tempChar;
            for (int i = 0, length = buttons.Count; i < length; i++)
            {
                KeyCodeButton btn = buttons[i];
                if (btn.type == KeyCodeType.Letter)
                {
                    tempChar = btn.key;
                    tempChar = isLower ? char.ToLower(tempChar) : char.ToUpper(tempChar);
                    btn.key = tempChar;
                }
                else if (btn.type == KeyCodeType.UpperLower)
                {
                    btn.UpdateIcon(isLower);
                }
            }
        }

        public void Show(bool show)
        {
            if (parent == null)
                return;
            //if (parent.activeInHierarchy != show)
                parent.SetActive(show);    
            rectTransform.localEulerAngles = Vector3.zero;
            SwitchLetters(true);
        }

        public void Clear()
        {

        }
    }

    public class VRKeyboardProxy
    {
        public char[] digitSymKeys = null;
        private char[] letterKeys = null;
        private char[] symbolKeys = null;
        private char[] digitKeys = null;
        private char[] japanKeys = null;

        //特殊字符需要特殊字体大小
        private Dictionary<char, int> specialChars = new Dictionary<char, int>();
        public static KeyboardType MASKKEYBOAD = KeyboardType.Digit;
        /// <summary>
        /// 键盘列表
        /// </summary>
        private List<KeyboardInfo> keyboards;

        /// <summary>
        /// 键盘点击回调函数
        /// </summary>
        private UnityAction<char> keyClickFun;

        /// <summary>
        /// 确定键的回调函数
        /// </summary>
        private UnityAction sureCallFun;

        /// <summary>
        /// 功能键删除回调函数
        /// </summary>
        private UnityAction deleteCallFun;

        public UnityAction<KeyCodeType> m_CommonFunctionButton;
        /// <summary>
        /// 键盘切换回调
        /// </summary>
        private UnityAction<KeyboardType> switchCallFun;

        /// <summary>
        /// 关闭键盘回调
        /// </summary>
        private UnityAction closeCallFun;

        private KeyboardType selectedType = KeyboardType.Letter;
        public KeyboardType currentKeyboard
        {
            get
            {
                return selectedType;
            }
            set
            {
                selectedType = value;
                ComplexBoardAction.Instance.associateKeyBoard.Visable((value & KeyboardType.Japan)!=0);
            }
        }

        public List<KeyCodeButton> commonButtons;
        
        public VRKeyboardProxy()
        {
            japanKeys = new char[] { 'あ', 'か', 'さ', 'た', 'な', 'は', 'ま', 'や', 'ら','わ','、'};
            digitKeys = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '.','0'  };
            digitSymKeys = new char[] { '.', '-'};
            letterKeys = new char[] { 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 'v', 'b', 'n', 'm', '!', '?', '.', ',' };
            //symbolKeys = new char[] { '~', '\\', '.', ',', '?', '!', '\'', '`', '_', '-', '/', ':', ';', '(', ')', '$', '&', '@', '"', '[', ']', '{', '}', '#', '%', '^', '*', '+', '=', '¥', '|', '<', '>' };
            symbolKeys = new char[] { '@', '#', '$', '%', '&', '*', '-', '+', '(', ')', '~', '`', '"', '\'', ':', ';', '_', '=', '\\', '/', '{', '}', '[', ']', '<', '>', '^', '|', '!', '?', ',', '.' };

            specialChars.Add(',', 40);
            specialChars.Add(';', 40);
            specialChars.Add(':', 40);
            specialChars.Add('*', 40);
            specialChars.Add('.', 40);
            specialChars.Add('`', 40);

            keyboards = new List<KeyboardInfo>();
            commonButtons = new List<KeyCodeButton>();
        }

        public void AddCommonButton(KeyCodeButton btn)
        {
            if (btn == null)
                return;
            for (int i = 0; i < commonButtons.Count; i++)
            {
                if (commonButtons[i].type == btn.type)
                {
                    Debug.Log("has the same type button!!!!"+btn.type);
                    return;
                }
            }
            btn.AddListener(ButtonClickHandler);
            commonButtons.Add(btn);
        }

        public List<KeyCodeButton> GetKeyboardButtons(KeyboardType type,KeyCodeType keyCode)
        {
            KeyboardInfo info = GetKeyboard(type);
            if (info == null)
                return null;
            return info.GetButtons(keyCode);
        }

        public KeyCodeButton GetCommonButton(KeyCodeType type)
        {
            for (int i = 0; i < commonButtons.Count; i++)
            {
                if (commonButtons[i].type == type)
                {
                    return commonButtons[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 切换键盘
        /// </summary>
        /// <param name="type"></param>
        public void SwitchKeyboard(KeyboardType type)
        {
            currentKeyboard = type;
            Show();
        }

        /// <summary>
        /// 查询是否拥有该键盘
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool HasKeyboad(KeyboardType type)
        {
            for (int i = 0; i < keyboards.Count; i++)
            {
                if (type == keyboards[i].type)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public KeyboardInfo GetKeyboard(KeyboardType type)
        {
            if (type == KeyboardType.Unknow) return null;
            if (type != MASKKEYBOAD) type = type ^ MASKKEYBOAD;
            for (int i = 0; i < keyboards.Count; i++)
            {
                if (type == keyboards[i].type)
                    return keyboards[i];
            }
            return null;
        }

        public void Show(KeyboardType type)
        {
            currentKeyboard = type;
            for (int i = 0; i < keyboards.Count; i++)
            {
                if ((type & keyboards[i].type)!=0)
                {
                    keyboards[i].Show(true);
                    if (keyboards[i].type != MASKKEYBOAD&& switchCallFun != null)
                    {
                        switchCallFun.Invoke(selectedType);
                        break;
                    }
                    
                }
            }
        }

        public void Show()
        {
            for (int i = 0; i < keyboards.Count; i++)
            {
                if ((selectedType & keyboards[i].type) == keyboards[i].type)
                {
                    keyboards[i].Show(true);
                    if (keyboards[i].type != MASKKEYBOAD && switchCallFun != null)
                        switchCallFun.Invoke(selectedType);
                }
                else
                    keyboards[i].Show(false);
            }
        }

        public void Unshow(KeyboardType type)
        {
            for (int i = 0; i < keyboards.Count; i++)
            {
                if ((keyboards[i].type & type)!=0)
                {
                    keyboards[i].Show(false);
                    break;
                }
            }
        }

        public void Unshow()
        {
            for (int i = 0; i < keyboards.Count; i++)
            {
                keyboards[i].Show(false);
            }
        }

        public void AddListeners(UnityAction<char> keyClick, UnityAction deleteFun, UnityAction sureFun = null, UnityAction<KeyboardType> switchFun = null, UnityAction closeFun = null)
        {
            //if (keyClickFun != null && keyClickFun != keyClick)
            //    Debug.Log("Change Keyboard!!");
            keyClickFun = keyClick;
            deleteCallFun = deleteFun;
            sureCallFun = sureFun;
            switchCallFun = switchFun;
            closeCallFun = closeFun;


        }

        /// <summary>
        /// 移除所有监听
        /// </summary>
        public void RemoveListeners()
        {
            keyClickFun = null;
            deleteCallFun = null;
            sureCallFun = null;
            closeCallFun = null;
            switchCallFun = null;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="type"></param>
        /// <param name="target"></param>
        public void RegisteKeyboard(KeyboardType type, GameObject target)
        {
            if (HasKeyboad(type))
            {
                Debug.Log("this keyboard has registed that type = " + type + ";taregt = " + target);
                return;
            }
            KeyCodeButton[] btns = target.GetComponentsInChildren<KeyCodeButton>();
            KeyboardInfo info = new KeyboardInfo(type, target);
            info.AddButtons(btns);
            keyboards.Add(info);

            if (type == KeyboardType.Japan)
            {
                List<KeyCodeButton> japanBtns = info.GetButtons(KeyCodeType.JaPan);
                UpdateKeyValue(japanKeys, japanBtns);
            }
            else if (type == KeyboardType.Letter)
            {
                List<KeyCodeButton> letterBtns = info.GetButtons(KeyCodeType.Letter);
                UpdateKeyValue(letterKeys, letterBtns);

                //List<KeyCodeButton> digitBtns = info.GetButtons(KeyCodeType.Digit);
                //UpdateKeyValue(digitSymKeys, digitBtns);
            }
            else if (type == KeyboardType.Digit)
            {
                List<KeyCodeButton> digitBtns = info.GetButtons(KeyCodeType.Digit);
                UpdateKeyValue(digitKeys, digitBtns);
            }
            //else if (type == KeyboardType.Digit_Symbol)
            //{
            //    List<char> list = new List<char>();
            //    list.AddRange(digitKeys);
            //    list.AddRange(digitSymKeys);
            //    List<KeyCodeButton> digitBtns = info.GetButtons(KeyCodeType.Digit);
            //    UpdateKeyValue(list.ToArray(), digitBtns);
            //}
            else if (type == KeyboardType.Symbol)
            {
                List<KeyCodeButton> symbolBtns = info.GetButtons(KeyCodeType.Symbol);
                UpdateKeyValue(symbolKeys, symbolBtns);
            }
            //else if (type == KeyboardType.Letter)
            //{
            //    List<KeyCodeButton> letterBtns = info.GetButtons(KeyCodeType.Letter);
            //    UpdateKeyValue(letterKeys, letterBtns);
            //}
            for (int i = 0; i < btns.Length; i++)
            {
                btns[i].AddListener(ButtonClickHandler);
            }
        }

        public void RemoveKeyboard(KeyboardType type)
        {
            for (int i = 0; i < keyboards.Count; i++)
            {
                if (type == keyboards[i].type)
                {
                    keyboards.Clear();
                    keyboards.RemoveAt(i);
                    break;
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < keyboards.Count; i++)
            {
                keyboards[i].Clear();
            }
            keyboards.Clear();
            RemoveListeners();
        }

        public void ClearKeyBoard()
        {
            keyboards.Clear();
        }

        public KeyboardInfo GetCurrentBoard()
        {
            return GetKeyboard(selectedType);
        }

        public KeyboardInfo GetNextBoard()
        {
            //int index = GetIndexByType(selectedType);
            //if (index < 0)
            //{
            //    Debug.LogWarning("can not finad the keyboard of type = " + selectedType);
            //    return null;
            //}
            //if (index < keyboards.Count-1)
            //    index += 1;
            //else
            //    index = 0;
            //return keyboards[index];
            return keyboards.GetNext(selectedType);
        }

        public KeyboardInfo GetPreBoard()
        {
            int index = GetIndexByType(selectedType);
            if (index < 0)
            {
                Debug.Log("can not finad the keyboard of type = " + selectedType);
                return null;
            }
            if (index > 0)
                index -= 1;
            else
                index = keyboards.Count - 1;
            return keyboards[index];
        }
        #region Private Function

        private void SwitchKeyboard(int index)
        {
            HighlightButton(false);
            for (int i = 0; i < keyboards.Count; i++)
            {
                if (index == i)
                {
                    keyboards[i].Show(true);
                    currentKeyboard = keyboards[i].type;
                    if (keyboards[i].type != MASKKEYBOAD && switchCallFun != null)
                        switchCallFun.Invoke(selectedType);
                }
                else
                {
                    keyboards[i].Show(false);
                }
            }
            HighlightButton(true);
        }

        private void HighlightButton(bool highlight)
        {
            KeyCodeType highType = KeyCodeType.Letter;
            if (selectedType == KeyboardType.Letter)
                highType = KeyCodeType.Letter;
            else if (selectedType == KeyboardType.Digit)
                highType = KeyCodeType.Digit;
            else if (selectedType == KeyboardType.Symbol)
                highType = KeyCodeType.SwitchSymbol;
            else if (selectedType == KeyboardType.Japan)
                highType = KeyCodeType.SwitchJaPan;

            KeyCodeButton btn = GetCommonButton(highType);
            if (btn != null)
                btn.isSelected = true;
        }

        private int GetIndexByType(KeyboardType type)
        {
            for (int i = 0; i < keyboards.Count; i++)
            {
                if (type == keyboards[i].type && type == KeyboardType.Digit)
                {
                    return i;
                }
                else if((type & keyboards[i].type) != 0)
                {
                    return i;
                }
            }
            return -1;
        }

        private void UpdateKeyValue(char[] chars, List<KeyCodeButton> buttons)
        {
            if (chars.Length != buttons.Count)
            {
                Debug.LogErrorFormat("chars size:[{0}] != buttons size:[{1}]",chars.Length,buttons.Count);
                return;
            }
            for (int i = 0, length = chars.Length; i < length; i++)
            {
                if (specialChars.ContainsKey(chars[i]))
                    buttons[i].UpdateLabel(chars[i], specialChars[chars[i]]);
                else
                    buttons[i].UpdateLabel(chars[i]);
            }
        }

        /// <summary>
        /// 按钮点击事件处理 中心 按钮点击后 都在此处统一进行事件的派发
        /// </summary>
        /// <param name="_btn_fun"></param>
        /// <param name="_key_value"></param>
        private void ButtonClickHandler(KeyCodeButton button)
        {
            switch (button.type)
            {
                case KeyCodeType.JaPan:
                    ComplexBoardAction.Instance.shotKeyBoard.Show(button, keyClickFun);
                    break;
                case KeyCodeType.Letter:
                case KeyCodeType.Digit:
                case KeyCodeType.Symbol:
                    if (keyClickFun != null)
                    {
                        keyClickFun.Invoke(button.key);
                    }
                    break;
                case KeyCodeType.Space:
                    if (keyClickFun != null)
                    {
                        keyClickFun.Invoke(' ');
                    }
                    break;
                case KeyCodeType.DeleteLast:
                    if (deleteCallFun != null)
                    {
                        deleteCallFun.Invoke();
                    }
                    break;
                case KeyCodeType.SwitchKeyboardButton:
                    //SwitchKeyboard(GetNextBoard().type | KeyboardType.Digit);
                    if (m_CommonFunctionButton != null) m_CommonFunctionButton.Invoke(KeyCodeType.SwitchKeyboardButton);
                    break;
                case KeyCodeType.SwitchSymbol:
                    //SwitchKeyboard(KeyboardType.Symbol | KeyboardType.Digit);
                    if (m_CommonFunctionButton != null) m_CommonFunctionButton.Invoke(KeyCodeType.SwitchSymbol);
                    break;
                case KeyCodeType.SwitchDigit:
                    //SwitchKeyboard(KeyboardType.Digit);
                    break;
                case KeyCodeType.SwitchLetter:
                    //SwitchKeyboard(KeyboardType.Letter | KeyboardType.Digit);
                    if (m_CommonFunctionButton != null) m_CommonFunctionButton.Invoke(KeyCodeType.SwitchLetter);
                    break;
                case KeyCodeType.UpperLower:
                    KeyboardInfo info = GetKeyboard(KeyboardType.Letter_Digit);
                    if (info == null)
                    {
                        info = GetKeyboard(KeyboardType.Letter);
                    }
                    if (info != null)
                        info.SwitchUpperOrLower();
                    break;
                case KeyCodeType.Back:
                    SwitchKeyboard(KeyboardType.Letter_Digit);
                    break;
                case KeyCodeType.Sure:
                    if (sureCallFun != null)
                        sureCallFun.Invoke();
                    break;
                case KeyCodeType.CloseKeyboard:
                    if (m_CommonFunctionButton != null) m_CommonFunctionButton.Invoke(KeyCodeType.CloseKeyboard);
                    if (closeCallFun != null)
                        closeCallFun.Invoke();
                    break;
            }
        }
        #endregion
    }
}
