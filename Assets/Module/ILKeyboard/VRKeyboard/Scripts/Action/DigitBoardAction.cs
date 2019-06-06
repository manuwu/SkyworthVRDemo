using DG.Tweening;
using IVRCommon.Keyboard.Enum;
using IVRCommon.Keyboard.Widet;
using System.Collections;
using UnityEngine;
namespace IVRCommon.Keyboard.Action
{
    public class DigitBoardAction : VRKeyboardAction<DigitBoardAction>
    {
        public GameObject digitBoard;
        public RectTransform boardMask;
        public KeyCodeButton deleteButton;
        public Vector2 defaultPosition = new Vector2(-110f, 68f);
        protected override void Start()
        {
            base.Start();
            proxy.RegisteKeyboard(KeyboardType.Digit, digitBoard);

            proxy.AddCommonButton(deleteButton);

            proxy.SwitchKeyboard(KeyboardType.Digit);
            isTween = false;
            isOpen = true;
            deleteButton.rectTransform.localPosition = new Vector3(defaultPosition.x, deleteButton.rectTransform.localPosition.y, 0f);
            isStarted = true;
        }

#if UNITY_EDITOR
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ShowBoard();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                HideBoard();
            }
        }
#endif

        public void ShowBoard()
        {
            if (isTween || isOpen)
                return;
            StartCoroutine(ShowInspector());
        }

        private IEnumerator ShowInspector()
        {
            isOpen = true;
            isTween = true;
            while (isStarted == false)
            {
                yield return null;
            }

            DOTween.Kill(boardMask,false);
            if (boardMask.gameObject.activeSelf == false)
                boardMask.gameObject.SetActive(true);
            deleteButton.rectTransform.localPosition = new Vector3(defaultPosition.x, deleteButton.rectTransform.localPosition.y, 0f);
           
            yield return null;
            boardMask.DOScale(Vector3.one, tweenTime * 0.5f).OnComplete(ShowTweenComplete);
        }

        private void ShowTweenComplete()
        {
            deleteButton.Show();
            deleteButton.rectTransform.DOLocalMove(Vector3.zero, tweenTime * 0.5f);
            isTween = false;
        }

        public void HideBoardNow()
        {
            if (isStarted)
            {
                isOpen = false;
                isTween = false;
                deleteButton.Hide();
                HideTweenComplete();
            }else
                StartCoroutine(HideInspector());
        }

        private IEnumerator HideInspector()
        {
            while (isStarted == false)
            {
                yield return null;
            }
            isOpen = false;
            isTween = false;
            deleteButton.Hide();
            HideTweenComplete();
        }

        public void HideBoard()
        {
            if (isTween || isOpen == false || boardMask.gameObject.activeSelf == false)
                return;
            isTween = true;
            isOpen = false;
            float time = tweenTime * 0.3f;

            deleteButton.rectTransform.DOLocalMove(new Vector3(defaultPosition.x, 0f, 0f), time).OnComplete(() =>
            {
                deleteButton.Hide();
                boardMask.DOScale(new Vector3(0.2f, 0.2f, 1f), time).OnComplete(HideTweenComplete);
            });
        }

        private void HideTweenComplete()
        {
            boardMask.gameObject.SetActive(false);
            isTween = false;
        }
    }
}
