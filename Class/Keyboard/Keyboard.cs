using Skynet.CoreServices;
using SkynetDota2Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Skynet
{
    class Keyboard : KeyboardHook
    {
        protected override IntPtr OnKeyHooked(int nCode, UIntPtr wParam, IntPtr lParam)
        {
            if (nCode == HC.ACTION)
            {
                // We want one key event per key key-press. To do this we need to
                // mask out key-down repeats and key-ups by making sure that bits 30
                // and 31 of the lParam are NOT set. Bit 30 specifies the previous
                // key state. The value is 1 if the key is down before the message is
                // sent; it is 0 if the key is up. Bit 31 specifies the transition
                // state. The value is 0 if the key is being pressed and 1 if it is
                // being released. Therefore, we are only interested in key events
                // where both bits are set to 0. To test for both of these bits being
                // set to 0 we use the constant REDUNDANT_KEY_EVENT_MASK.
                const uint REDUNDANT_KEY_EVENT_MASK = 0xC0000000;
                if (((uint)lParam & REDUNDANT_KEY_EVENT_MASK) == 0)
                {
                    // extract the keyCode and combine with modifier keys
                    Keys keyCombo = ((Keys)(int)wParam & Keys.KeyCode) | KeyboardHelper.GetModifierKeys();

                    frmMain.frm.ProcessKey(keyCombo);
                }
            }

            // key not handled by our hook, continue processing
            return CallNextHook(nCode, wParam, lParam);

        }
        public struct HC
        {
            public const int ACTION = 0;
            public const int GETNEXT = 1;
            public const int SKIP = 2;
            public const int NOREMOVE = 3;
            public const int SYSMODALON = 4;
            public const int SYSMODALOFF = 5;
        }
    }
}
