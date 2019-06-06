using UnityEngine;
using System.Collections;
using IVRCommon.Keyboard.Widet;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UI;
using IVRCommon.Keyboard.Action;
using UnityEngine.EventSystems;
using System;

public class ShotKeyBoard : MonoBehaviour,IPointerExitHandler
{
    
    /// <summary>
    /// 全角 清音 平假
    /// </summary>
    private Dictionary<char, char[]> full_japanDic = new Dictionary<char, char[]>()
    {
        { 'あ',new char[] { 'う','い',  'え', 'お' } },
        { 'か',new char[] { 'く','き',  'け', 'こ' } },
        { 'さ',new char[] { 'し', 'す', 'せ', 'そ' } },
        { 'た',new char[] { 'つ','ち',  'て', 'と' } },
        { 'な',new char[] { 'ぬ','に',  'ね', 'の' } },
        { 'は',new char[] { 'ふ','ひ',  'へ', 'ほ' } },
        { 'ま',new char[] {  'む', 'み','め', 'も' } },
        { 'や',new char[] { 'ゆ','『',  '』', 'よ' } },
        { 'ら',new char[] { 'り', 'る', 'れ', 'ろ' } },
        { 'わ',new char[] { 'ん', 'ー','を', } },
        { '、',new char[] { '?','。','!',} },
    };

    /// <summary>
    /// 半角 清音 平假
    /// </summary>
    private Dictionary<char, char> half_japanDic = new Dictionary<char, char>()
    {
        { 'あ','ぁ'},
        { 'い','ぃ'},
        { 'う','ぅ'},
        { 'え','ぇ'},
        { 'お','ぉ'},
        { 'か','ゕ'},
        { 'け','ゖ'},
        { 'つ','っ'},
        { 'や','ゃ'},
        { 'ゆ','ゅ'},
        { 'よ','ょ'},
        { 'わ','ゎ'},
    };

    /// <summary>
    /// 全角 平假 浊音
    /// </summary>
    private Dictionary<char, char> full_hiragana_sonantDic = new Dictionary<char, char>()
    {
        { 'か','が'},
        { 'き','ぎ'},
        { 'く','ぐ'},
        { 'け','げ'},
        { 'こ','ご'},

        { 'さ','ざ'},
        { 'し','じ'},
        { 'す','ず'},
        { 'せ','ぜ'},
        { 'そ','ぞ'},

        { 'た','だ'},
        { 'ち','ぢ'},
        { 'つ','づ'},
        { 'て','で'},
        { 'と','ど'},

        { 'は','ば'},
        { 'ひ','び'},
        { 'ふ','ぶ'},
        { 'へ','べ'},
        { 'ほ','ぼ'},
    };

    /// <summary>
    /// 全角 平假 半浊音
    /// </summary>
    private Dictionary<char, char> full_hiragana_half_sonantDic = new Dictionary<char, char>()
    {
        { 'は','ぱ'},
        { 'ひ','ぴ'},
        { 'ふ','ぷ'},
        { 'へ','ぺ'},
        { 'ほ','ぽ'},
    };

    public RectTransform m_EventMask;
    public GameObject m_fullMask, m_halfMask;
    public KeyCodeButton[] btns;
    public KeyCodeButton m_CenterButton;
    private UnityAction<char> keyClickFun;
    private Vector2 FULLMASK_SIZE = new Vector2(175, 62.8f);
    private Vector2 HALFMASK_SIZE = new Vector2(120.96f, 62.8f);
    private KeyCodeButton lastSelected = null;
    private Image normal_space, selected_space;
    private Text space_text;
    private bool onH, onV;
    //public GameObject goH, goV, goMask_japan,goMask_L, goMask_R, goMask_B;
    //BoxCollider boxColliderV;
    int originIdx = 0;

    void Awake()
    {
        //boxColliderV = goV.GetComponent<BoxCollider>();
//        btns = GetComponentsInChildren<KeyCodeButton>();
        for (int i = 0; i < btns.Length; ++i)
        {
            btns[i].AddListener(ButtonClickHandler);
        }

        m_CenterButton.AddListener(CenterButtonClickHandler);

    }
    private void CenterButtonClickHandler(KeyCodeButton button)
    {
        if (keyClickFun != null)
        {
            keyClickFun.Invoke(button.key);
        }

        Hide();
    }
    void Start()
    {
        //VREventListener.Get(goH).onHover = OnTriggerExit_H;
        //VREventListener.Get(goV).onHover = OnTriggerExit_V;

        //for (int i = 12353; i <= 12447; ++i)
        //{
        //    //Debug.LogError("当前：" + (char)i + "下一个:" + (char)(i + 96));
        //    Debug.LogError("当前：" + i + ":" + (char)i + "下一个:" + (i + 96) + ":" + (char)(i + 96));
        //}

        //for (int i = 12353; i <= 12447; i = i + 2)
        //{
        //    Debug.LogError("当前：" + (char)i + "   全角:" + (char)(i + 1) + " 下一个:" + (char)(i + 96) + "  全角:" + (char)(i + 97));
        //}
    }

    public void OnDisable()
    {
        //IVRTouchPad.RemoveKeyEvent(back, IVRTouchPad.KeyLayout.Layout_7);
    }

    private bool back()
    {
        Hide();
        return true;
    }

    /// <summary>
    /// 这个是处理最中心的那个按键
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="_callback"></param>
    public void Show(KeyCodeButton btn, UnityAction<char> _callback)
    {
        m_CenterButton.key = btn.key;
        if(btn.key.Equals(' '))
        {
            if (_callback != null)
            {
                ComplexBoardAction.Instance.associateKeyBoard.ClearKeys();
                _callback.Invoke(btn.key);
            }
            return;
        }

        if (btn.Equals(lastSelected))
        {
            Hide();
            SpecialCharacterHandle(btn, true);
            if (btn.key.Equals('、'))
            {
                if (_callback != null)
                {
                    ComplexBoardAction.Instance.associateKeyBoard.ClearKeys();
                    _callback.Invoke(btn.key);
                }
            }
            else
            {
                RunAssociate(btn);
            }
            return;
        }

        SpecialCharacterHandle(btn, false);
        lastSelected = btn;
        keyClickFun = _callback;
        char[] keys;
        if (full_japanDic.TryGetValue(btn.key, out keys))
        {
            onH = true;
            onV = true;
            Vector3 offset = btn.transform.localToWorldMatrix * new Vector3(btn.rectTransform.sizeDelta.x * 0.5f, btn.rectTransform.sizeDelta.y * 0.5f, 0);
            //Vector3 target_pos = btn.transform.parent.localToWorldMatrix * (btn.transform.localPosition + );
            transform.position = btn.transform.position + offset;
            originIdx = lastSelected.transform.GetSiblingIndex ();
            //lastSelected.transform.SetSiblingIndex(goMask_japan.transform.GetSiblingIndex() + 1);
            VisableMask(true);
            //IVRTouchPad.AddKeyEvent(back, IVRTouchPad.KeyLayout.Layout_7);
            gameObject.SetActive(true);
            int lenth = keys.Length;
            if (lenth == 3)
            {
                //Show half mask
                m_halfMask.SetActive(true);
                m_fullMask.SetActive(false);
                m_EventMask.sizeDelta = HALFMASK_SIZE;
            }
            else
            {
                //show full mask
                m_halfMask.SetActive(false);
                m_fullMask.SetActive(true);
                m_EventMask.sizeDelta = FULLMASK_SIZE;
            }

            for (int i = 0; i < 4; ++i)
            {
                if (i < lenth)
                {
                    btns[i].key = keys[i];
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
            if (keyClickFun != null)
            {
                keyClickFun.Invoke(btn.key);
            }
        }
    }

    public void Hide()
    {
        if(lastSelected != null)
        {
            lastSelected.transform.SetSiblingIndex(originIdx);
        }
        VisableMask(false);
        SpecialCharacterHandle(lastSelected, true);
        gameObject.SetActive(false);
        lastSelected = null;
    }

    public void Reset(List<KeyCodeButton> btns)
    {
        foreach (KeyCodeButton btn in btns)
        {
            SpecialCharacterHandle(btn, true);
        }
        Hide();
    }

    /// <summary>
    /// 这个方法是处理弹出的stot keys,不包括最中心的那个
    /// </summary>
    /// <param name="button"></param>
    private void ButtonClickHandler(KeyCodeButton button)
    { 
        if (button.key.Equals('。') || button.key.Equals('!') || button.key.Equals('?'))
        {
            if (keyClickFun != null)
            {
                ComplexBoardAction.Instance.associateKeyBoard.ClearKeys();
                keyClickFun.Invoke(button.key);
            }
        }
        else
        {
            RunAssociate(button);
        }
        button.isSelected = false;
        Hide();
    }

    private void SpecialCharacterHandle(KeyCodeButton btn, bool normalState)
    {
        if (btn == null)
            return;

//        switch (btn.key)
//        {
//            case '、':
//                Text text = btn.transform.Find("key_text").GetComponent<Text>();
//                Image normal = btn.transform.Find("key_icon_normal").GetComponent<Image>();
//                if (normalState)
//                {
//                    normal.enabled = true;
//                    text.enabled = false;
//                }
//                else
//                {
//                    normal.enabled = false;
//                    text.enabled = true;
//                }
//                break;
//        }
    }

    public void OnTriggerExit_H(GameObject go, bool state)
    {
        onH = state;
        if(!onV)
        {
            Hide();
        }
    }

    public void OnTriggerExit_V(GameObject go, bool state)
    {
        onV = state;
        if(!onH)
        {
            Hide();
        }
    }

    private void VisableMask(bool visable)
    {
        //goMask_japan.SetActive(visable);
        //goMask_L.SetActive(visable);
        //goMask_R.SetActive(visable);
        //goMask_B.SetActive(visable);
    }

    void RunAssociate(KeyCodeButton btn)
    {
        List<char> result = new List<char>();
        result.Add(btn.key);
        char sorted;
        if (full_hiragana_sonantDic.TryGetValue(btn.key, out sorted))
        {
            result.Add(sorted);
        }
        if (full_hiragana_half_sonantDic.TryGetValue(btn.key, out sorted))
        {
            result.Add(sorted);
        }
        if (half_japanDic.TryGetValue(btn.key, out sorted))
        {
            result.Add(sorted);
        }

        if(result.Count == 1)
        {
            if (keyClickFun != null)
            {
                ComplexBoardAction.Instance.associateKeyBoard.ClearKeys();
                keyClickFun.Invoke(btn.key);
            }
        }
        else
        {
            ComplexBoardAction.Instance.associateKeyBoard.Visable(true, result, keyClickFun);
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        Hide();
    }
}
