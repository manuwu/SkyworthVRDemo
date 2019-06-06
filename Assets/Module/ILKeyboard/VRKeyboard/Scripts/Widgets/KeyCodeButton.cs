using IVRCommon.Keyboard.Enum;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using IVR.Language;
using System;

namespace IVRCommon.Keyboard.Widet
{
    public class KeyCodeButton : Button
    {
        [Serializable]
        public class OnCallFunEvent : UnityEvent<KeyCodeButton> { }
        //按键上显示的字符
        public Text label;
        //按键功能
        public KeyCodeType type = KeyCodeType.Letter;

        private char _key;
        public char key
        {
            set
            {
                _key = value;
                label.text = value.ToString();
            }
            get
            {
                return _key;
            }
        }

        private bool selected = false;
        public bool isSelected
        {
            set
            {
                if (value)
                {
                    DoStateTransition(SelectionState.Highlighted, false);
                    if (targetGraphic) targetGraphic.color = hoverColor;
                }
                else
                {
                    DoStateTransition(SelectionState.Normal, false);
                    if (targetGraphic) targetGraphic.color = normalColor;
                }
                selected = value;
            }
        }

        public bool islocked = false;
        public CanvasRenderer[] renders;

        [SerializeField]
        private OnCallFunEvent callFun;

        public Color normalColor = Color.white;
        public Color hoverColor = Color.white;

        private RectTransform mTransform;
        public RectTransform rectTransform
        {
            get
            {
                if(mTransform == null)
                    mTransform = GetComponent<RectTransform>();
                return mTransform;
            }
        }
        protected override void Start()
        {
            onClick.AddListener(ClickHandler);
            if (label == null)
                label = GetComponentInChildren<Text>();
            if (targetGraphic == null)
                targetGraphic = GetComponentInChildren<Image>();
            base.Start();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            isSelected = false;
        }
        public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            //base.OnSelect(eventData);
        }

        public override void OnDeselect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            //base.OnDeselect(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (selected == false && interactable)
            {
                //if(label != null)
                //    label.rectTransform.localScale = Vector3.one;
                base.OnPointerExit(eventData);
                if (targetGraphic) targetGraphic.color = normalColor;
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            
            if (selected == false && interactable)
            {
                
                //if(type == KeyCodeType.Digit || type == KeyCodeType.Letter || type == KeyCodeType.Symbol)
                //    label.rectTransform.localScale = Vector3.one * 1.5f;
                base.OnPointerEnter(eventData);
                if (targetGraphic) 
                {
                    targetGraphic.color = hoverColor;
                    ((Image)targetGraphic).overrideSprite = spriteState.highlightedSprite;
                }
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (selected == false && interactable)
                base.OnPointerUp(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (selected == false && interactable)
                base.OnPointerDown(eventData);
        }

        public void AddListener(UnityAction<KeyCodeButton> callback)
        {
            callFun.AddListener(callback);
        }

        public virtual void SetCull(bool cull)
        {
            if (label != null)
                label.SetAllDirty();
            if (targetGraphic != null)
                targetGraphic.SetAllDirty();
            if (renders == null || renders.Length < 1)
                return;
            for (int i = 0; i < renders.Length;i++ )
            {
                renders[i].cull = cull;
            }
        }

        public void Show(bool locked)
        {
            islocked = locked;
            gameObject.SetActive(true);
        }

        public void Hide(bool locked)
        {
            islocked = locked;
            gameObject.SetActive(false);
        }

        public void Show()
        {
            if (islocked)
                return;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (islocked)
                return;
            selected = false;
            gameObject.SetActive(false);
        }

        public void UpdateLabel(char val, int size = 0)
        {
            _key = val;
            if (label == null)
                label = GetComponentInChildren<Text>();
            if (label == null)
                return; 
            label.text = val.ToString();
            if(Application.systemLanguage == SystemLanguage.Japanese)
            {
                //label.font = Language.CurrentFont;
            }
            if (size > 0)
                label.fontSize = size;
        }

        public virtual void UpdateIcon(bool lower) { }

        private void ClickHandler()
        {
            if (interactable && callFun != null)
                callFun.Invoke(this);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            renders = GetComponentsInChildren<CanvasRenderer>();
            if (targetGraphic) targetGraphic.color = normalColor;
        }
#endif
    }
}
