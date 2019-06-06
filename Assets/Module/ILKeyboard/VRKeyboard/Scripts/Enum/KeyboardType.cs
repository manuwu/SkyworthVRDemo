using System;

namespace IVRCommon.Keyboard.Enum
{
    [Flags]
    public enum KeyboardType
    {
        Unknow = 0,
        /// <summary>
        /// 字母键盘
        /// </summary>
        Letter = 0x0001,

        /// <summary>
        /// 符号键盘
        /// </summary>
        Symbol = 0x0002,

        /// <summary>
        /// 数字键盘
        /// </summary>
        Digit = 0x0004,

        
        /// <summary>
        /// 日文键盘
        /// </summary>
        Japan = 0x0008,

        Letter_Digit = Letter | Digit,
        Digit_Symbol = Digit | Symbol
    }

    ///// <summary>
    ///// 由单独键盘组合成一个键盘
    ///// </summary>
    //public enum KeyboardMaping
    //{
    //    /// <summary>
    //    /// 字母数字键盘
    //    /// </summary>
    //    English = KeyboardType.Letter | KeyboardType.Digit,
    //    /// <summary>
    //    /// 日文键盘
    //    /// </summary>
    //    Japan = KeyboardType.Letter | KeyboardType.Japan,
    //    /// <summary>
    //    /// 数字符号键盘
    //    /// </summary>
    //    Symbol = KeyboardType.Digit | KeyboardType.Symbol,
    //    /// <summary>
    //    /// 只有数字的键盘
    //    /// </summary>
    //    Digit = KeyboardType.Digit

    //}
}
