using UnityEngine.UI;

namespace IVRCommon.Keyboard.Widet
{
    public class KeySymbolButton : KeyCodeButton
    {
        public Image lowerIcon;
        public Image upperIcon;

        public override void UpdateIcon(bool lower)
        {
            lowerIcon.enabled = lower;
            upperIcon.enabled = !lower;
        }

        public override void SetCull(bool cull)
        {
            upperIcon.SetAllDirty();
            lowerIcon.SetAllDirty();
            base.SetCull(cull);
        }
    }
}
