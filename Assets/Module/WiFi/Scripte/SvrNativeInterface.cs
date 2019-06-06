/*
 * Author:李传礼
 * DateTime:2018.4.23
 * Description:原生接口
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SvrNativeInterface 
{
    public WifiUtils WifiUtils;

    bool IsInit;
    AndroidJavaObject AndroidInterface;

    private static SvrNativeInterface mSvrNativeInterface;
    public static SvrNativeInterface Instance
    {
        get
        {
            if (mSvrNativeInterface == null)
                mSvrNativeInterface = new SvrNativeInterface();
            return mSvrNativeInterface;
        }
    }
    private AndroidJavaObject CurrentActivity;
    private AndroidJavaObject JApplication;
    public SvrNativeInterface()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        if (unityPlayer != null)
            CurrentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        if (CurrentActivity != null)
        {
            JApplication = CurrentActivity.Call<AndroidJavaObject>("getApplication");
        }
#endif
        InitAndroidInterface();

        WifiUtils = new WifiUtils(this);
    }

    ~SvrNativeInterface()
    {
        Release();
    }

    void InitAndroidInterface()
    {
        if (IsInit)
            return;

        IsInit = true;
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass AndroidInterfaceClass = new AndroidJavaClass("com.ssnwt.vr.androidmanager.AndroidInterface");
        if (AndroidInterfaceClass == null)
        {
            return;
        }

        AndroidInterface = AndroidInterfaceClass.CallStatic<AndroidJavaObject>("getInstance");
        if (AndroidInterface == null)
        {
            return;
        }

        AndroidInterface.Call("init", JApplication);
#endif
    }

    void Release()
    {
        if (AndroidInterface != null)
            AndroidInterface.Call("release");
        IsInit = false;
    }

    public AndroidJavaObject GetUtils()
    {
        AndroidJavaObject utils = null;

        if (AndroidInterface != null)
            utils = AndroidInterface.Call<AndroidJavaObject>("getWifiUtils");

        return utils;
    }
}
