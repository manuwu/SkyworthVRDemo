/*
 * Author:李传礼
 * DateTime:2018.4.23
 * Description:Wifi工具
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WifiUtils
{
    AndroidJavaObject InterfaceObj;

    public WifiUtils(SvrNativeInterface svrNI)
    {
        if (svrNI != null)
            InterfaceObj = svrNI.GetUtils();

    }

    public void SetOnWifiSwitchStatusChangeEvent(string gameObjectName, string functionName)
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("setOnOpenedListener", gameObjectName, functionName);
    }

    public void SetOnScanWifiListEvent(string gameObjectName, string functionName)
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("setOnScanResultListener", gameObjectName, functionName);
    }

    public void SetOnWifiConnectStatusChangeEvent(string gameObjectName, string functionName)
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("setOnConnectingListener", gameObjectName, functionName);
    }

    public void SetMaxRssiLevel(int level)
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("setMaxRssiLevel", level);
    }

    public void SetOnLevelChangeEvent(string gameObjectName, string functionName)
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("setRssiLevelChangedListener", gameObjectName, functionName);
    }

    public bool GetWifiSwitchStatus()
    {
        bool isOpen = false;

        if (InterfaceObj != null)
            isOpen = InterfaceObj.Call<bool>("isOpenWifi");

        return isOpen;
    }

    public void OpenWifi()
    {
        Debug.Log("openWifi:"+ InterfaceObj);
        if (InterfaceObj != null)
            InterfaceObj.Call("openWifi");
    }

    public void CloseWifi()
    {
        Debug.Log("closeWifi:" + InterfaceObj);
        if (InterfaceObj != null)
            InterfaceObj.Call("closeWifi");
    }

    public bool SearchWifi()
    {
        if (InterfaceObj != null)
        {
            bool result = InterfaceObj.Call<bool>("searchWifi");
            Debug.Log("SearchWifi:" + result);
            return result;
        }
        else
            return false;
    }

    public int ConnectWifi(string name, string mac, string capabilities, string password)
    {
        if (InterfaceObj != null)
            return InterfaceObj.Call<int>("connectWifi", name, mac, capabilities, password);
        else
            return -1;
    }

    public void ConnectWifi(int networkId)
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("connectWifi", networkId);
    }

    public void DisconnectWifi(int networkId)
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("disconnectWifi", networkId);
    }

    public void DisconnectWifi()
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("disconnectWifi");
    }

    public void ForgetPassword(int networkId)
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("forget", networkId);
    }

    public int GetCurrentNetworkId()
    {
        if (InterfaceObj != null)
            return InterfaceObj.Call<int>("getCurrentNetworkID");
        else
            return -1;
    }

    public string GetCurrentConnectedJWifiInfo()
    {
        if (InterfaceObj != null)
            return InterfaceObj.Call<string>("getConnectedWifi");
        else
            return "";
    }

    //信号强度最小值
    public int GetMinLevel()
    {
        int minRSSI = -100;

        if (InterfaceObj != null)
            minRSSI = InterfaceObj.Call<int>("getMinRssi");

        return minRSSI;
    }

    public int GetWifiLevel()
    {
        int RSSI = 0;

        if (InterfaceObj != null)
            RSSI = InterfaceObj.Call<int>("getWifiRssiLevel");

        return RSSI;
    }

    public void Resume()
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("resume");
    }

    public void Pause()
    {
        if (InterfaceObj != null)
            InterfaceObj.Call("pause");
    }
}


[Serializable]
public class JsonArray<T>
{
    public T[] array;

    public JsonArray(T[] array)
    {
        this.array = array;
    }

    public static T[] GetJsonArray(string json)
    {
        string newJson = "{\"array\":" + json + "}";
        JsonArray<T> jsonArray = JsonUtility.FromJson<JsonArray<T>>(newJson);
        return jsonArray.array;
    }

    public static string ConvertToJson(T[] array)
    {
        JsonArray<T> jsonArray = new JsonArray<T>(array);
        string json = JsonUtility.ToJson(jsonArray);

        string sign = "{\"array\":";
        int index = json.IndexOf(sign);
        if (index != -1)
        {
            json = json.Substring(index + sign.Length);
            json = json.Substring(0, json.Length - 1);
            return json;
        }
        else
            return "";
    }
}