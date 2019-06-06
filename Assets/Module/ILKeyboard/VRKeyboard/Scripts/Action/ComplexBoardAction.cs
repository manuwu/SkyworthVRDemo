using IVRCommon.Keyboard.Enum;
using IVRCommon.Keyboard.Widet;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

namespace IVRCommon.Keyboard.Action
{
    public class ComplexBoardAction : VRKeyboardAction<ComplexBoardAction>
    {
        
        public MaskableGraphic hotArea;
        public GameObject japanBoard;
        public GameObject digitBoard;
        public GameObject letterBoard;
        public GameObject symbolBoard;
        public GameObject m_rightbar;

        //public KeyCodeButton japanButton;
        //public KeyCodeButton letterButton;
        //public KeyCodeButton digitButton;
        public KeyCodeButton symbolButton;
        public KeyCodeButton sureButton;
        public KeyCodeButton deleteButton;
        public KeyCodeButton hidebutton;

        public RectTransform boardMask;

        private Vector2 boardMinSize = new Vector2(410f, 402.2f);
        private Vector2 boardMaxSize = new Vector2(1000f, 402.2f);
        private Vector2 maskMinSize = new Vector2(733, 402.2f);
        private Vector2 maskMaxSize = new Vector2(1000f, 402.2f);

        public Vector2 defaultPosition = new Vector2(110f, 68f);
        private Vector3 DIGIT_LEFT_LETTER_POSITION = new Vector3(-400.5f,0,0);
        private Vector3 DIGIT_LEFT_JAPAN_POSITION = new Vector3(-260.4f, 0,0);
        private Vector3 RIGHTBAR_DIGIT_AND_JAPAN_POSITION = new Vector3(169.4f,0,0);
        private Vector3 RIGHTBAR_LETTER_POSITION = new Vector3(363.18f,0,0);
        private Vector3 RIGHTBAR_JAPAN_POSITION = new Vector3(223, 0,0);
        public ShotKeyBoard shotKeyBoard;
        public AssociateKeyBoard associateKeyBoard;
        private KeyCodeButton selectedBtn = null;
        private List<KeyboardType> keyboardGroup = new List<KeyboardType>();
        //private Vector2 pressPosition = Vector2.zero;
        private bool locked = false;
        private bool japanBoardAvailable = true;
#if UNITY_EDITOR
        [ContextMenu("ResetBarPosition")]
        public void ResetBarPosition()
        {
            if (digitBoard) digitBoard.transform.localPosition = DIGIT_LEFT_LETTER_POSITION;
            if (m_rightbar) m_rightbar.transform.localPosition = RIGHTBAR_LETTER_POSITION;
            if(boardMask) boardMask.sizeDelta = maskMaxSize;
        }
#endif
        public bool JapanBoardAvailable
        {
            get
            {
                return japanBoardAvailable;
            }

            set
            {
                japanBoardAvailable = value;
                Init();
            }
        }

        private void Init()
        {
            proxy.ClearKeyBoard();
            if (JapanBoardAvailable && PlatformManager.mInstance.platform == PlatformManager.PLATFORM.JAPAN)
            {
                proxy.RegisteKeyboard(KeyboardType.Japan, japanBoard);
                keyboardGroup.Add(KeyboardType.Japan | KeyboardType.Digit);
            }
            proxy.RegisteKeyboard(KeyboardType.Letter, letterBoard);
            keyboardGroup.Add(KeyboardType.Letter | KeyboardType.Digit);
            proxy.RegisteKeyboard(KeyboardType.Digit, digitBoard);
            proxy.RegisteKeyboard(KeyboardType.Symbol, symbolBoard);

            //123 动态调整
            Vector3 pos = this.transform.localPosition;
            this.transform.localPosition = new Vector3(0, pos.y, pos.z);
        }

        protected override void Start()
        {
            base.Start();
            Init();
            //proxy.AddCommonButton(japanButton);
            //proxy.AddCommonButton(letterButton);
            //proxy.AddCommonButton(digitButton);
            //proxy.AddCommonButton(symbolButton);
            proxy.AddCommonButton(sureButton);
            proxy.AddCommonButton(deleteButton);
            proxy.AddCommonButton(hidebutton);
            //hidebutton.onClick.AddListener(HideKeyboard_Event);
            //japanButton.onClick.AddListener(JapanClickHandler);
            //letterButton.onClick.AddListener(LetterClickHandler);
            //symbolButton.onClick.AddListener(SymbolClickHandler);
            //digitButton.onClick.AddListener(DigitClickHandler);

            ResetDefault(KeyboardType.Unknow);
            hotArea.enabled = false;
            isStarted = true;
            proxy.m_CommonFunctionButton = Commone_Event_Keyboard;

        }

        private void Commone_Event_Keyboard(KeyCodeType arg0)
        {
            switch (arg0)
            {
               
                case KeyCodeType.SwitchSymbol:
                    SwitchBoard(proxy.GetKeyboard(KeyboardType.Symbol | KeyboardType.Digit));
                    break;
                case KeyCodeType.SwitchLetter:
                    SwitchBoard(proxy.GetKeyboard(KeyboardType.Letter | KeyboardType.Digit));
                    break;
                
                case KeyCodeType.SwitchKeyboardButton:
                 
                    KeyboardInfo info = GetNextBoardType();
                    SwitchBoard(info);
                    break;
                case KeyCodeType.CloseKeyboard:
                    HideBoards();
                    break;
                default:
                    break;
            }
        }
        public KeyboardInfo GetNextBoardType()
        {
            int currentindex = 0;
            for (int i = 0; i < keyboardGroup.Count; i++)
            {
                if ((keyboardGroup[i] ^ VRKeyboardProxy.MASKKEYBOAD) == proxy.GetCurrentBoard().type)
                {
                    currentindex = i;
                    break;
                }
            }
            int index = (currentindex + 1) % keyboardGroup.Count;
            return proxy.GetKeyboard(keyboardGroup[index]);
        }
        private void HideKeyboard_Event()
        {
            HideBoards();
            
        }

        private void ResetDefault(KeyboardType type)
        {
            //if (type == KeyboardType.Digit)
            //{
            //    proxy.SwitchKeyboard(KeyboardType.Digit_Symbol);
            //}
            //else
            //{
            //    proxy.SwitchKeyboard(type);
            //}
            proxy.SwitchKeyboard(type);
            isTween = false;
            //boardMask.localScale = new Vector3(0.2f, 0.2f, 1f);
            if ((type == KeyboardType.Digit) || (type & KeyboardType.Japan) != 0)
            {
                if ((type & KeyboardType.Japan) != 0)
                {
                    digitBoard.transform.localPosition = DIGIT_LEFT_JAPAN_POSITION;
                    m_rightbar.transform.localPosition = RIGHTBAR_JAPAN_POSITION;
                }
                else
                {
                    digitBoard.transform.localPosition = Vector3.zero;
                    m_rightbar.transform.localPosition = RIGHTBAR_DIGIT_AND_JAPAN_POSITION;
                }
                
                boardMask.sizeDelta = maskMinSize;
                rectTransform.sizeDelta = boardMinSize;
                //KeyboardInfo info = proxy.GetKeyboard(KeyboardType.Digit_Symbol);
                //if (info != null)
                //{
                //    if (info.canvasGroup != null)
                //        info.canvasGroup.alpha = 1f;
                //}
                CheckBoard(type);
                //sureButton.rectTransform.sizeDelta = new Vector2(boardMinSize.x, sureButton.rectTransform.sizeDelta.y);
            }
            else
            {
                digitBoard.transform.localPosition = DIGIT_LEFT_LETTER_POSITION;
                m_rightbar.transform.localPosition = RIGHTBAR_LETTER_POSITION;
                KeyboardInfo info = proxy.GetKeyboard(type);
                if (info != null && info.canvasGroup != null)
                {
                    info.canvasGroup.alpha = 1f;
                }
                boardMask.sizeDelta = maskMaxSize;
                rectTransform.sizeDelta = boardMaxSize;
                //sureButton.rectTransform.sizeDelta = new Vector2(maskMaxSize.x, sureButton.rectTransform.sizeDelta.y);
            }
            //japanButton.rectTransform.localPosition = new Vector3(defaultPosition.x, japanButton.rectTransform.localPosition.y, 0f);
            //letterButton.rectTransform.localPosition = new Vector3(defaultPosition.x, letterButton.rectTransform.localPosition.y, 0f);
            //digitButton.rectTransform.localPosition = new Vector3(defaultPosition.x, digitButton.rectTransform.localPosition.y, 0f);
            //symbolButton.rectTransform.localPosition = new Vector3(defaultPosition.x, symbolButton.rectTransform.localPosition.y, 0f);
            //sureButton.rectTransform.localPosition = new Vector3(sureButton.rectTransform.localPosition.x, defaultPosition.y, 0f);
            //deleteButton.rectTransform.localPosition = new Vector3(-defaultPosition.x, deleteButton.rectTransform.localPosition.y, 0f);
            HideButtons();
            boardMask.gameObject.SetActive(false);
        }

        private void CheckBoard(KeyboardType type)
        {
            //KeyboardInfo info = proxy.GetKeyboard(KeyboardType.Digit_Symbol);
            //if (info != null)
            //{
            //    List<KeyCodeButton> buttons = info.GetButtons(new List<char>(proxy.digitSymKeys));
            //    bool show = (type == KeyboardType.Digit_Symbol || type == KeyboardType.Japan) ? true : false;
            //    for (int i = 0; i < buttons.Count; i++)
            //    {
            //        if (show)
            //            buttons[i].Show();
            //        else
            //            buttons[i].Hide();
            //    }
            //}
        }

#if UNITY_EDITOR
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
//                KeyboardInfo info = proxy.GetNextBoard();
//                SwitchBoard(info);
            }
            //else if (Input.GetKeyDown(KeyCode.DownArrow))
            //{
            //    KeyboardInfo info = proxy.GetPreBoard();
            //    SwitchBoard(info);
            //}
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ShowBoard(KeyboardType.Letter | KeyboardType.Digit);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                HideBoards();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ShowLockedBoard(KeyboardType.Digit);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ShowLockedBoard(KeyboardType.Digit | KeyboardType.Symbol);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                ShowBoard(KeyboardType.Japan | KeyboardType.Digit);
            }
                
        }
#endif

#region Custom Event 
        /// <summary>
        /// 点击日文按钮，显示日文键盘
        /// </summary>
        private void JapanClickHandler()
        {
            if (proxy.currentKeyboard == KeyboardType.Japan)
                return;
            KeyboardInfo to = proxy.GetKeyboard(KeyboardType.Japan);
            SwitchBoard(to);
            shotKeyBoard.Reset(to.GetButtons(KeyCodeType.JaPan));
        }

        /// <summary>
        /// 点击字母按钮，显示字母键盘
        /// </summary>
        private void LetterClickHandler()
        {
            if (proxy.currentKeyboard == KeyboardType.Letter)
                return;
            KeyboardInfo to = proxy.GetKeyboard(KeyboardType.Letter);
            SwitchBoard(to);
        }

        /// <summary>
        /// 点击符号按钮，显示符号键盘
        /// </summary>
        //private void SymbolClickHandler()
        //{
        //    if ((proxy.currentKeyboard & KeyboardType.Symbol) != 0)
        //        return;
        //    KeyboardInfo to = proxy.GetKeyboard(KeyboardType.Symbol | KeyboardType.Digit);
        //    SwitchBoard(to);
        //}

        /// <summary>
        /// 点击数字按钮，显示数字键盘
        /// </summary>
        //private void DigitClickHandler()
        //{
        //    if (proxy.currentKeyboard == KeyboardType.Digit_Symbol)
        //        return;
        //    KeyboardInfo to = proxy.GetKeyboard(KeyboardType.Digit_Symbol);
        //    SwitchBoard(to);
        //}
#endregion

        public void ShowKeyButton(KeyCodeType type)
        {
            //KeyCodeButton button = proxy.GetCommonButton(type);
            //if (button == null)
            //    return;
            //button.Show(false);
        }

        public void HideKeyButton(KeyCodeType type)
        {
            //KeyCodeButton button = proxy.GetCommonButton(type);
            //if (button == null)
            //    return;
            //button.Hide(true);
        }

        /// <summary>
        /// 只显示锁定的键盘
        /// </summary>
        /// <param name="type"></param>
        public void ShowLockedBoard(KeyboardType type)
        {
            locked = true;
            if (isTween || isOpen)
                return;
            StartCoroutine(ShowInspector(type));
        }
        private KeyboardType currentType = KeyboardType.Unknow;
        /// <summary>
        /// 切换键盘
        /// </summary>
        /// <param name="type"></param>
        public void ShowBoard(KeyboardType type,bool isOnlyEnglish = false)
        {
            bool needchange = (type | VRKeyboardProxy.MASKKEYBOAD) != currentType;
            currentType = type | VRKeyboardProxy.MASKKEYBOAD;
            locked = false;
            if (isTween || isOpen)
                return;
            StartCoroutine(ShowInspector(currentType));
        }

        private IEnumerator ShowInspector(KeyboardType type)
        {
            isOpen = true;
            isTween = true;
            hotArea.enabled = true;
            //IVRTouchPad.TouchEvent_onSwipe += TouchSwipeHandler;
            while (isStarted == false)
            {
                yield return null;
            }
            KillAllTween(true);
            ResetDefault(type);
            yield return null;
            if (boardMask.gameObject.activeSelf == false)
                boardMask.gameObject.SetActive(true);
            yield return null;
            //boardMask.DOScale(Vector3.one, tweenTime * 0.5f).OnComplete(ShowTweenComplete);
        }

        //private void ShowTweenComplete()
        //{
        //    ShowButtons();
        //    float time = tweenTime * 0.5f;
        //    //japanButton.rectTransform.DOLocalMoveX(0f, time);
        //    //letterButton.rectTransform.DOLocalMoveX(0f, time);
        //    //digitButton.rectTransform.DOLocalMoveX(0f, time).SetDelay(0.1f);
        //    //symbolButton.rectTransform.DOLocalMoveX(0f, time).SetDelay(0.2f);
        //    deleteButton.rectTransform.DOLocalMove(Vector3.zero, time);
        //    sureButton.rectTransform.DOLocalMove(Vector3.zero, time);
        //    isTween = false;
        //    KeyboardType type = proxy.currentKeyboard;
        //    //if (type == KeyboardType.Japan)
        //    //    selectedBtn = japanButton;
        //    //else if (type == KeyboardType.Letter)
        //    //    selectedBtn = letterButton;
        //    //else if (type == KeyboardType.Symbol)
        //    //    selectedBtn = symbolButton;
        //    //else
        //    //    selectedBtn = digitButton;
        //    selectedBtn.isSelected = true;
        //}

        private void HideButtons()
        {
            //japanButton.Hide();
            //letterButton.Hide();
            //digitButton.Hide();
            //symbolButton.Hide();
            //sureButton.Hide();
            //deleteButton.Hide();
        }

        private void ShowButtons()
        {
            if (locked == false)
            {
                //if (JapanBoardAvailable && PlatformManager.mInstance.platform == PlatformManager.PLATFORM.JAPAN)
                //{
                //    japanButton.Show();
                //    japanButton.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(50f, 0f);
                //}
                //else
                //{
                //    japanButton.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(50f, 32f);
                //}

                //letterButton.Show();
                //digitButton.Show();
                //symbolButton.Show();
                sureButton.Show();
            }
            deleteButton.Show();
        }

        private void KillAllTween(bool complete)
        {
            DOTween.Kill(sureButton, complete);
            DOTween.Kill(deleteButton, complete);
            //DOTween.Kill(japanButton, complete);
            //DOTween.Kill(letterButton, complete);
            //DOTween.Kill(digitButton, complete);
            //DOTween.Kill(symbolButton, complete);
            DOTween.Kill(boardMask, complete);
            DOTween.Kill(rectTransform, complete);
        }

        /// <summary>
        /// 隐藏全部键盘
        /// </summary>
        public void HideBoards()
        {
            shotKeyBoard.Hide();
            associateKeyBoard.gameObject.SetActive(false);
            //IVRTouchPad.TouchEvent_onSwipe -= TouchSwipeHandler;
            //if (isTween || isOpen == false)
            //    return;
            //isTween = true;
            hotArea.enabled = false;
            isOpen = false;
            StopAllCoroutines();
            KillAllTween(false);
            //float time = tweenTime * 0.3f;
            //sureButton.rectTransform.DOLocalMove(new Vector3(0f, defaultPosition.y, 0f), time);
            //sureButton.rectTransform.DOSizeDelta(new Vector2(boardMask.sizeDelta.x, sureButton.rectTransform.sizeDelta.y), time);
            //deleteButton.rectTransform.DOLocalMove(new Vector3(-defaultPosition.x, 0f, 0f), time);
            //letterButton.rectTransform.DOLocalMoveX(defaultPosition.x, time);
            //digitButton.rectTransform.DOLocalMoveX(defaultPosition.x, time).SetDelay(0.05f);
            //symbolButton.rectTransform.DOLocalMoveX(defaultPosition.x, time).SetDelay(0.1f).OnComplete(() =>
            //{
            //    boardMask.DOScale(new Vector3(0.2f, 0.2f, 1f), time).OnComplete(HideTweenComplete);
            //    HideButtons();
            //});
            HideButtons();
            HideTweenComplete();
        }

        private void HideTweenComplete()
        {
            boardMask.gameObject.SetActive(false);
            isTween = false;
        }
        private void SwitchBoard(KeyboardInfo to)
        {
            if (to == null || isTween)
                return;
            isTween = true;
            if (selectedBtn != null)
                selectedBtn.isSelected = false;

            KeyboardInfo from = proxy.GetCurrentBoard();
            KeyboardType fromType = from.type | VRKeyboardProxy.MASKKEYBOAD;
            KeyboardType toType = to.type | VRKeyboardProxy.MASKKEYBOAD;
            if (toType == KeyboardType.Digit || (toType & KeyboardType.Japan)!=0)
            {
                //if(toType == KeyboardType.Digit_Symbol)
                //{
                //    selectedBtn = digitButton;
                //}
                //else
                //{
                //    selectedBtn = japanButton;
                //}
                //CheckBoard(toType);
                //sureButton.rectTransform.DOSizeDelta(new Vector2(boardMinSize.x,sureButton.rectTransform.sizeDelta.y), tweenTime);
                //boardMask.DOSizeDelta(maskMinSize, tweenTime).OnComplete(() => MaskZoomComplete(to, false));
                //rectTransform.DOSizeDelta(boardMinSize, tweenTime);

                if ((toType & KeyboardType.Japan) != 0)
                {
                    //digitBoard.transform.localPosition = DIGIT_LEFT_JAPAN_POSITION;
                    //m_rightbar.transform.localPosition = RIGHTBAR_JAPAN_POSITION;

                    digitBoard.transform.DOLocalMove(DIGIT_LEFT_JAPAN_POSITION, tweenTime * 0.5f);
                    m_rightbar.transform.DOLocalMove(RIGHTBAR_JAPAN_POSITION, tweenTime * 0.5f);
                }
                else
                {
                    digitBoard.transform.localPosition = Vector3.zero;
                    m_rightbar.transform.localPosition = RIGHTBAR_DIGIT_AND_JAPAN_POSITION;

                    digitBoard.transform.DOLocalMove(DIGIT_LEFT_JAPAN_POSITION, tweenTime * 0.5f);
                    m_rightbar.transform.DOLocalMove(RIGHTBAR_JAPAN_POSITION, tweenTime * 0.5f);
                }

                
                
                //boardMask.sizeDelta = maskMinSize;
                //rectTransform.sizeDelta = boardMinSize;
                //boardMask.DOSizeDelta(maskMinSize, tweenTime).OnComplete(() => MaskZoomComplete(to, false));
                rectTransform.DOSizeDelta(boardMinSize, tweenTime * 0.5f).OnComplete(()=> 
                {
                    isTween = false;
                    proxy.SwitchKeyboard(toType);
                });
            }
            else
            {
                if (fromType == KeyboardType.Digit || ((fromType & KeyboardType.Japan)!=0))
                {
                    //rectTransform.DOSizeDelta(boardMaxSize, tweenTime);
                    if (from.canvasGroup != null)
                        from.canvasGroup.DOFade(0f, tweenTime).OnComplete(() =>
                        {
                            proxy.Unshow(fromType);
                        });
                    else
                        proxy.Unshow(fromType);

                    proxy.SwitchKeyboard(toType);
                    //digitBoard.transform.localPosition = DIGIT_LEFT_LETTER_POSITION;
                    //m_rightbar.transform.localPosition = RIGHTBAR_LETTER_POSITION;
                    digitBoard.transform.DOLocalMove(DIGIT_LEFT_LETTER_POSITION, tweenTime * 0.5f);
                    m_rightbar.transform.DOLocalMove(RIGHTBAR_LETTER_POSITION, tweenTime * 0.5f);
                    //sureButton.rectTransform.DOSizeDelta(new Vector2(maskMaxSize.x, sureButton.rectTransform.sizeDelta.y), tweenTime);
                    boardMask.DOSizeDelta(maskMaxSize, tweenTime).OnComplete(() => 
                    {
                        isTween = false;
                    });
                }
                else
                {
                    

                    from.rectTransform.DOLocalRotate(new Vector3(90f, 0f, 0f), tweenTime * 0.5f, RotateMode.Fast).OnComplete(() =>
                      {
                          proxy.SwitchKeyboard(toType);
                          to.rectTransform.localEulerAngles = new Vector3(-90f, 0f, 0f);
                          to.rectTransform.DOLocalRotate(Vector3.zero, tweenTime * 0.5f, RotateMode.Fast).OnComplete(RotateComplete);
                      });
                }
                //if(toType == KeyboardType.Letter)
                //    selectedBtn = letterButton;
                //else
                //    selectedBtn = symbolButton;
            }

            //selectedBtn.isSelected = true;
        }

        private void RotateComplete()
        {
            isTween = false;
        }

        private void MaskZoomComplete(KeyboardInfo toBoard, bool dirty)
        {
            if (dirty)
            {
                List<KeyCodeButton> list = toBoard.keys;
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].SetCull(false);
                }
                isTween = false;
            }
            else
            {
                if (toBoard.canvasGroup != null)
                {
                    proxy.Show(toBoard.type);
                    toBoard.canvasGroup.alpha = 0f;
                    toBoard.canvasGroup.DOFade(1f, tweenTime).OnComplete(() =>
                    {
                        proxy.SwitchKeyboard(toBoard.type);
                        isTween = false;
                    });
                }
                else
                {
                    proxy.SwitchKeyboard(toBoard.type);
                    isTween = false;
                }
            }
        }

        /// <summary>
        /// swip事件
        /// </summary>
        /// <param name="swip"></param>
        //private void TouchSwipeHandler(SwipEnum swip)
        //{
        //    if (locked || isOpen == false)
        //        return;
        //    KeyboardInfo info = null;
        //    //VLog.Warning(name +"---"+ "OnPointerUp....." + dirction);
        //    if (swip == SwipEnum.MOVE_DOWN)
        //    {
        //        info = proxy.GetPreBoard();
        //        if (!ComplexBoardAction.Instance.JapanBoardAvailable && info.type == KeyboardType.Japan)
        //        {
        //            info = proxy.GetPreBoard();
        //        }
        //    }
        //    else if (swip == SwipEnum.MOVE_UP)
        //    {
        //        info = proxy.GetNextBoard();
        //        if (!ComplexBoardAction.Instance.JapanBoardAvailable && info.type == KeyboardType.Japan)
        //        {
        //            info = proxy.GetNextBoard();
        //        }
        //    }
        //    shotKeyBoard.Hide();
        //    SwitchBoard(info);
        //}

        /*
        public void OnPointerUp(PointerEventData eventData)
        {
            if (locked || isOpen == false)
                return;
            IVRRayPointerEventData data = eventData as IVRRayPointerEventData;
            Vector2 upPos = data.TouchPadPosition;
            MoveDirection dirction = ConvertUtil.CalculateSwipe(pressPosition, upPos);
            if (dirction == MoveDirection.None)
                return;
            KeyboardInfo info = null;
            //VLog.Warning(name +"---"+ "OnPointerUp....." + dirction);
            if (dirction == MoveDirection.Up)
            {
                info = proxy.GetPreBoard();
               
            }
            else if (dirction == MoveDirection.Down)
            {
                info = proxy.GetNextBoard();
            }
          
            SwitchBoard(info);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IVRRayPointerEventData data = eventData as IVRRayPointerEventData;
            pressPosition = data.TouchPadPosition;
        }*/
    }
}
