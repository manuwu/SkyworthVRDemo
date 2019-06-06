/*
 * Author:李传礼
 * DateTime:2018.4.23
 * Description:Wifi管理
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public enum WifiStatus { Enabled, Saved, Using, Connecting }
public enum WifiStrength { Low, Middle, High, Full }
public enum WifiConnectStatus { Idle, Disconnected, Connecting, Connected, PasswordError, NotSupport }

[Serializable]
public class JWifiInfo
{
    public string SSID;//网络名
    public string BSSID;//mac地址
    public string capabilities;//加密信息
    public int level;//信号强度等级
    public int networkID;//已保存wifi的 网络ID
    public int wifiStatus;//wifi状态
    public bool needPassword;//是否需要密码连接
    public bool is5G; //是否是5G信号
}

public class WifiInfo
{
    public int NetworkId;
    public string Name;
    public string MacAddress;
    public string Capabilities;
    public WifiStrength WifiStrength;
    public WifiStatus WifiStatus;
    public bool NeedPassword;
    public bool Is5G; //是否是5G信号

    public WifiInfo(JWifiInfo jWifiInfo)
    {
        NetworkId = jWifiInfo.networkID;
        Name = jWifiInfo.SSID;
        MacAddress = jWifiInfo.BSSID;
        Capabilities = jWifiInfo.capabilities;
        WifiStrength = (WifiStrength)jWifiInfo.level;
        WifiStatus = (WifiStatus)jWifiInfo.wifiStatus;
        NeedPassword = jWifiInfo.needPassword;
        Is5G = jWifiInfo.is5G;
    }

    public WifiInfo(int networkId, string name, string macAddress, string capabilities, WifiStrength wifiStrength, WifiStatus wifiStatus, bool needPassword, bool is5G)
    {
        NetworkId = networkId;
        Name = name;
        MacAddress = macAddress;
        Capabilities = capabilities;
        WifiStrength = wifiStrength;
        WifiStatus = wifiStatus;
        NeedPassword = needPassword;
        Is5G = is5G;
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(Name);
        stringBuilder.Append(":    ");
        stringBuilder.Append(WifiStrength);
        stringBuilder.Append("    ");
        stringBuilder.Append(WifiStatus);
        stringBuilder.Append("    ");
        stringBuilder.Append(NeedPassword? "password" : "NoPassword");
        stringBuilder.Append(Is5G? "    5G" : "");
        return stringBuilder.ToString();
    }
}

public class WifiManage : MonoBehaviour
{
    public WifiUtils WifiUtils;

    public Action<bool> WifiSwitchStatusChangeCallback;
    public Action<List<WifiInfo>> ScanWifiInfoListCallback;
    public Action<WifiConnectStatus, string> WifiConnectStatusChangeCallback;
    public Action<WifiStrength> WifiStrengthChangeCallback;

    private static WifiManage mwifiManager;
    public static WifiManage Instance
    {
        get
        {
            if (mwifiManager == null)
            {
                GameObject wifogo = new GameObject("WifiManage");
                mwifiManager = wifogo.AddComponent<WifiManage>();
            }
            return mwifiManager;
        }
    }

    public bool IsChangeStrength { get; internal set; }
    public WifiStrength ConnectedWifiStrength { get; internal set; }

    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        WifiUtils = SvrNativeInterface.Instance.WifiUtils;

        WifiUtils.SetOnWifiSwitchStatusChangeEvent(this.name, "OnWifiSwitchStatusChangeEvent");
        WifiUtils.SetOnScanWifiListEvent(this.name, "OnScanWifiListEvent");
        WifiUtils.SetOnWifiConnectStatusChangeEvent(this.name, "OnWifiConnectStatusChangeEvent");
        WifiUtils.SetOnLevelChangeEvent(this.name, "OnLevelChangeEvent");
    }
    private void OnDestroy()
    {
        mwifiManager = null;
    }
    /// <summary>
    /// OpenWifi成功后才回调
    /// </summary>
    /// <param name="isOpenStr">open status</param>
    void OnWifiSwitchStatusChangeEvent(string isOpenStr)
    {
        Debug.Log("OnWifiSwitchStatusChangeEvent:"+ isOpenStr);
        bool isOpen = bool.Parse(isOpenStr);

        if (WifiSwitchStatusChangeCallback != null)
            WifiSwitchStatusChangeCallback(isOpen);
    }

    void OnScanWifiListEvent(string jWifiInfoStrs)
    {
        Debug.Log("OnScanWifiListEvent:"+jWifiInfoStrs);
        JWifiInfo[] jWifiInfos = JsonArray<JWifiInfo>.GetJsonArray(jWifiInfoStrs);

        List<WifiInfo> wifiInfoList = new List<WifiInfo>();
        if (jWifiInfos != null)
        {
            foreach (JWifiInfo jWifiInfo in jWifiInfos)
            {
                wifiInfoList.Add(new WifiInfo(jWifiInfo));
            }
        }

        for (int i = 0; i < wifiInfoList.Count; i++)
        {
            Debug.Log("wifiInfoList[" + i + "].name = " + wifiInfoList[i].Name +
                ", id = " + wifiInfoList[i].NetworkId + ", status = " + wifiInfoList[i].WifiStatus
                + ", level = " + (int)wifiInfoList[i].WifiStrength
                + ", strength = " + wifiInfoList[i].WifiStrength
                + ", is5G = " + wifiInfoList[i].Is5G);
        }

        if (ScanWifiInfoListCallback != null)
            ScanWifiInfoListCallback(wifiInfoList);
    }

    void OnWifiConnectStatusChangeEvent(string wifiConnectStatusStr)
    {
        Debug.Log("OnWifiConnectStatusChangeEvent:" + wifiConnectStatusStr);
        string[] str = wifiConnectStatusStr.Split(';');

        int wifiConnectStatus = int.Parse(str[0]);

        if (str[1].Contains("\""))
            str[1] = str[1].Replace("\"", "");

        if (WifiConnectStatusChangeCallback != null)
            WifiConnectStatusChangeCallback((WifiConnectStatus)wifiConnectStatus, str[1]);
    }

    void OnLevelChangeEvent(string levelStr)
    {
        int level = int.Parse(levelStr);

        if (WifiStrengthChangeCallback != null)
            WifiStrengthChangeCallback((WifiStrength)level);
    }

    public WifiStrength GetWifiStrength()
    {
        if (WifiUtils != null)
        {
            int level = WifiUtils.GetWifiLevel();
            return (WifiStrength)level;
        }
        else
            return WifiStrength.Low;
    }
}
