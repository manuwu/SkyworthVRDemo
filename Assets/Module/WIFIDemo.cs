using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IVRCommon.Keyboard;

public class WIFIDemo : MonoBehaviour
{

    class WifiListContent
    {
        public WifiInfo wifiInfo;
        public GameObject wifiItemObject;
    }

    public Text m_WifiState;
    public GameObject m_WifiItem;
    public GameObject m_WifiRoot;
    public Text m_connectName;
    public IVRInputField m_IVRInputField;
    public Text m_ConnectText;
    public Text m_WifiConnectStatus;
    private WifiInfo m_SelectWiFi;
    private List<WifiListContent> wifiListContents = new List<WifiListContent>();
    // Use this for initialization
    void Start()
    {
        WifiManage.Instance.WifiSwitchStatusChangeCallback = WifiManage_Instance_WifiSwitchStatusChangeCallback;
        WifiManage.Instance.ScanWifiInfoListCallback = WifiManage_Instance_ScanWifiInfoListCallback;
        WifiManage.Instance.WifiConnectStatusChangeCallback = WifiManage_Instance_WifiConnectStatusChangeCallback;

        //List<WifiInfo> wifiInfos = new List<WifiInfo>();
        //for (int i = 0; i < 20; i++)
        //{
        //    wifiInfos.Add(new WifiInfo(1,"1","XXXX","123", WifiStrength.Full, WifiStatus.Enabled,true,false));
        //}
        //WifiManage_Instance_ScanWifiInfoListCallback(wifiInfos);
    }

    private void WifiManage_Instance_WifiConnectStatusChangeCallback(WifiConnectStatus arg1, string arg2)
    {
        Debug.Log("WifiConnectStatusChangeCallback:" + arg1 + ", " + arg2);
        m_WifiConnectStatus.text = arg1.ToString();

        if (arg1 == WifiConnectStatus.Connected || arg1 == WifiConnectStatus.Disconnected)
        {
            WifiManage.Instance.WifiUtils.SearchWifi();
        }
    }

    private void OnEnable()
    {
        WifiManage.Instance.WifiUtils.Resume();
    }
    private void OnDisable()
    {
        WifiManage.Instance.WifiUtils.Pause();
    }
    private void WifiManage_Instance_ScanWifiInfoListCallback(List<WifiInfo> obj)
    {
        if (wifiListContents.Count > 0)
        {
            foreach (var item in wifiListContents)
            {
                Destroy(item.wifiItemObject);
            }
            wifiListContents.Clear();
            m_SelectWiFi = null;
        }
        foreach (var item in obj)
        {
            GameObject wifiItemObject = Instantiate(m_WifiItem, m_WifiRoot.transform);
            Debug.Log("CreatWifiItem:" + item.Name);
            wifiItemObject.GetComponentInChildren<Text>().text = item.ToString();
            wifiItemObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                m_connectName.text = item.Name;
                m_SelectWiFi = item;
                if (m_SelectWiFi.WifiStatus == WifiStatus.Using)
                    m_ConnectText.text = "disconnect";
                else
                    m_ConnectText.text = "connect";
            });

            wifiListContents.Add(new WifiListContent() { wifiInfo = item, wifiItemObject = wifiItemObject });
        }
        Canvas.ForceUpdateCanvases();
    }

    private void WifiManage_Instance_WifiSwitchStatusChangeCallback(bool obj)
    {
        m_WifiState.text = obj ? "Open" : "Close";
        if (obj)
        {
            WifiManage.Instance.WifiUtils.SearchWifi();
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void Event_OpenWiFi()
    {
        WifiManage.Instance.WifiUtils.OpenWifi();
    }

    public void Event_CloseWiFi()
    {
        WifiManage.Instance.WifiUtils.CloseWifi();
    }

    public void Event_ForgetPassword()
    {
        Debug.Log("ForgetPassword");
        if (m_SelectWiFi != null)
        {
            Debug.Log("ForgetPassword:" + m_SelectWiFi.Name);
            WifiManage.Instance.WifiUtils.ForgetPassword(m_SelectWiFi.NetworkId);
            WifiManage.Instance.WifiUtils.SearchWifi();
        }

    }

    public void Event_Connect()
    {
        Debug.Log("Event_Connect:" + m_IVRInputField.text);
        if (m_SelectWiFi != null)
        {

            switch (m_SelectWiFi.WifiStatus)
            {
                case WifiStatus.Enabled:
                    if (!string.IsNullOrEmpty(m_IVRInputField.text))
                        WifiManage.Instance.WifiUtils.ConnectWifi(m_SelectWiFi.Name, m_SelectWiFi.MacAddress, m_SelectWiFi.Capabilities, m_IVRInputField.text);
                    break;
                case WifiStatus.Saved:
                    WifiManage.Instance.WifiUtils.ConnectWifi(m_SelectWiFi.NetworkId);
                    break;
                case WifiStatus.Using:
                    WifiManage.Instance.WifiUtils.DisconnectWifi(m_SelectWiFi.NetworkId);
                    break;
                case WifiStatus.Connecting:
                    break;
                default:
                    break;
            }
        }
    }
}
