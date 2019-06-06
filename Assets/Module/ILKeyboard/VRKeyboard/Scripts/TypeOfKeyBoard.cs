//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace IVRCommon.Keyboard
//{
//    /// <summary>
//    /// 键盘处理类
//    /// </summary>
//    public class TypeOfKeyBoard : MonoBehaviour
//    {
//        public KeyCodeButton[] mKeycodeButtons;
//        public KeyboardType keyboardType = KeyboardType.None;
//        // Use this for initialization
//        void Start()
//        {
//            for (int i = 0; i < mKeycodeButtons.Length; i++)
//            {
//                mKeycodeButtons[i].AddListener(Button_Event);
//            }
//        }

//        // Update is called once per frame
//        void Update()
//        {

//        }
//#if UNITY_EDITOR
//        [ContextMenu("FindButtons")]
//        public void FindButtons()
//        {
//            mKeycodeButtons = GetComponentsInChildren<KeyCodeButton>(false);
            
//            char[] code = VRKeyboardProxy.GetParticularByType(keyboardType);
//            Debug.Assert(code.Length == mKeycodeButtons.Length,string.Format("gameobject size:{0} not match code size:{1}", mKeycodeButtons.Length, code.Length));
//            for (int i = 0; i < mKeycodeButtons.Length; i++)
//            {
//                mKeycodeButtons[i].AddListener(Button_Event);
//                mKeycodeButtons[i].label.text = code[i].ToString();
//            }
//        }
//#endif
//        public void Button_Event(KeyCodeButton button)
//        {
//            Debug.Log("Button_Event");
//        }
//    }
//}
