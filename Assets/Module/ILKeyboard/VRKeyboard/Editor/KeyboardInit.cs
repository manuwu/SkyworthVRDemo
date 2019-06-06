using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class KeyboardInit  {

    static KeyboardInit()
    {
        string macros = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        if (!macros.Contains("SVR_KEYBOARD"))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, macros + ";SVR_KEYBOARD");
        }
    }
}
