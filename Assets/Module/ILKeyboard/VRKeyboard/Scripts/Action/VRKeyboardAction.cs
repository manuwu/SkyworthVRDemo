using IVRCommon.Keyboard.Enum;
using System;
using UnityEngine;
using UnityEngine.Events;
namespace IVRCommon.Keyboard.Action
{
    public abstract class VRKeyboardAction<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T mInstance = null;

        public static T Instance
        {
            get
            {
                return mInstance;
            }
        }

        protected bool isOpen = false;
        public bool opend
        {
            get
            {
                return isOpen;
            }
        }
        private RectTransform mRectTrans = null;
        public RectTransform rectTransform
        {
            get
            {
                if (mRectTrans == null)
                    mRectTrans = GetComponent<RectTransform>();
                return mRectTrans;
            }
        }
        public float tweenTime = 0.3f;
        public bool isTween = false;
        protected bool isStarted = false;
        protected VRKeyboardProxy proxy;

        public class AddEvent : UnityEvent { };
        public static AddEvent OnAddEvent = new AddEvent();
        void Awake()
        {
            mInstance = (MonoBehaviour)this as T;
            proxy = new VRKeyboardProxy();
        }

        protected virtual void Start()
        {
           
        }

        void OnDisable()
        {
            isTween = false;
        }

        public void AddListeners(UnityAction<char> keyClick, UnityAction deleteFun, UnityAction sureFun = null, UnityAction<KeyboardType> switchFun = null,UnityAction closeFun = null)
        {
            proxy.AddListeners(keyClick, deleteFun, sureFun, switchFun, closeFun);
            if (OnAddEvent != null) OnAddEvent.Invoke();
        }

        public void RemoveListeners()
        {
            proxy.RemoveListeners();
        }
    }
}
