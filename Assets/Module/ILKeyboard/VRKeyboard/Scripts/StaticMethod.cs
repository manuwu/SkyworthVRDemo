using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IVRCommon.Keyboard.Enum;

namespace IVRCommon.Keyboard
{
    public static class StaticMethod
    {

        public static bool IsHave(this List<KeyboardInfo> keyboars, KeyboardInfo keyinfo)
        {
            if (keyboars == null) return false;
            for (int i = 0; i < keyboars.Count; i++)
            {
                if (keyboars[i] == keyinfo) return true;
            }
            return false;
        }

        //public static KeyboardGroup GetNext(this List<KeyboardGroup> keyboars, int currentindex = 0)
        //{
        //    if (keyboars == null || keyboars.Count == 0) return null;
        //    int nextindex = (currentindex + 1) % keyboars.Count;
        //    return keyboars[nextindex];
        //}
        //public static int GetKeyboardByType(this List<KeyboardGroup> keyboars, KeyboardMape maptype)
        //{
        //    for (int i = 0; i < keyboars.Count; i++)
        //    {
        //        if (keyboars[i].keyboardType == maptype) return i;
        //    }
        //    return -1;
        //}

        public static KeyboardInfo GetNext(this List<KeyboardInfo> keyboars, int currentindex = 0)
        {
            if (keyboars == null || keyboars.Count == 0) return null;
            int nextindex = (currentindex + 1) % keyboars.Count;
            return keyboars[nextindex];
        }
        public static KeyboardInfo GetNext(this List<KeyboardInfo> keyboars, KeyboardType currenttype = KeyboardType.Unknow)
        {
            int currentindex = 0;
            if (currenttype != KeyboardType.Unknow)
            {
                currenttype = currenttype ^ KeyboardType.Digit;
                if (currenttype == 0) currenttype = KeyboardType.Digit;
            }
            currentindex = keyboars.GetKeyboardIndexByType(currenttype);
            if (keyboars == null || keyboars.Count == 0) return null;
            int nextindex = (currentindex + 1) % keyboars.Count;
            return keyboars[nextindex];
        }
        public static int GetKeyboardIndexByType(this List<KeyboardInfo> keyboars, KeyboardType maptype)
        {
            for (int i = 0; i < keyboars.Count; i++)
            {
                if (keyboars[i].type == maptype) return i;
            }
            return -1;
        }
    }
}
