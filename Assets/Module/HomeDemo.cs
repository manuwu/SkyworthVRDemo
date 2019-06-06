using IVRCommon.Keyboard.Action;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HomeDemo : MonoBehaviour {

    public GameObject player;
    public GameObject wifi;

    public void Event_ShutDown()
    {
        //DemoTools.ShutDown();
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.ssnwt.vr.common.PowerManager");
            unityPlayer.CallStatic("powerOption", true);
        }
        catch(Exception ee)
        {
            Debug.LogError("GetIsScreenOn exception:" + ee);
        }
#endif
    }
    public void Event_Reboot()
    {
        //DemoTools.Reboot();
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.ssnwt.vr.common.PowerManager");
            unityPlayer.CallStatic("powerOption", false);
        }
        catch(Exception ee)
        {
            Debug.LogError("GetIsScreenOn exception:" + ee);
        }
#endif
    }

    public void Event_OpenWifi()
    {
        wifi.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Event_OpenPlayer()
    {
        player.SetActive(true);
        gameObject.SetActive(false);
        ComplexBoardAction.Instance.HideBoards();// 临时键盘
    }

    public void Event_Back()
    {
        player.SetActive(false);
        wifi.SetActive(false);
        gameObject.SetActive(true);
        ComplexBoardAction.Instance.HideBoards();// 临时键盘
    }
}
