using UnityEngine;
using System.Collections;
using IVRCommon.Keyboard.Widet;
using UnityEngine.Events;
using System.Text;
using IVRCommon.Keyboard.Action;
using DG.Tweening;
using System.Collections.Generic;

public class AssociateKeyBoard : MonoBehaviour
{
    private KeyCodeButton[] btns;
    private UnityAction<char> keyClickFun;

    void Awake()
    {
        btns = GetComponentsInChildren<KeyCodeButton>();
        for (int i = 0; i < btns.Length; ++i)
        {
            btns[i].AddListener(ButtonClickHandler);
        }
    }
    void Start()
    {
       
    }

    private void ButtonClickHandler(KeyCodeButton button)
    {
        button.isSelected = false;
        if (keyClickFun != null)
        {
            ClearKeys();
            keyClickFun.Invoke(button.key);
        }
    }

    public void Visable(bool visiable, List<char> result = null, UnityAction<char> _keyClickFun = null)
    {
        gameObject.SetActive(visiable);
        //ComplexBoardAction.Instance.rectTransform.DOLocalMove(visiable ? new Vector3(0f, -55f,0f) : Vector3.zero,0.3f);
        if(result != null && visiable)
        {
            this.keyClickFun = _keyClickFun;
            int len = result.Count;
            for (int i = 0;i < btns.Length; i++)
            {
                if(i < len)
                {
                    btns[i].UpdateLabel(result[i]);
                    btns[i].Show();
                }
                else
                {
                    btns[i].Hide();
                }
            }
        }
        else
        {
            ClearKeys();
        }
    }

    public void ClearKeys()
    {
        for (int i = 0; i < btns.Length; i++)
        {
            btns[i].Hide();
        }
    }
}