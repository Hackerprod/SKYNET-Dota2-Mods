using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKYNET
{
    public class Keyboard : KeyboardHook
    {
        public event EventHandler<Keys> OnKeyPress;
        protected override IntPtr OnKeyHooked(int nCode, UIntPtr wParam, IntPtr lParam)
        {
            if (nCode == HC.ACTION)
            {
                const uint REDUNDANT_KEY_EVENT_MASK = 0xC0000000;
                if (((uint)lParam & REDUNDANT_KEY_EVENT_MASK) == 0)
                {
                    // extract the keyCode and combine with modifier keys
                    Keys keyCombo = ((Keys)(int)wParam & Keys.KeyCode) | KeyboardHelper.GetModifierKeys();

                    OnKeyPress?.Invoke(this, keyCombo); 
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
