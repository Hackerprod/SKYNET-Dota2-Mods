// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SKYNET
{
    /// <summary>
    /// KeyboardHook used for monitoring (and optionally handling) key events for
    /// a window. You must always call the Remove() method to uninstall the hook when
    ///	you no longer need it or when the process or thread is terminating.
    ///	Failing to do this will result in an "orphaned" hook procedure which
    ///	may cause crashes and system instability!
    /// </summary>
    public abstract class KeyboardHook
    {
        /// <summary>
        /// Install a Keyboard hook on the thread of the passed hWnd
        /// </summary>
        /// <param name="hIEFrame">Handle to window to hook</param>
        public void Install(IntPtr hWnd)
        {
            // verify that Install is not called twice
            if (m_hHook != IntPtr.Zero)
            {
                // don't install redundantly in release, but warn in debug
                Debug.Assert(false, "Hook already installed");
                return;
            }

            // get the thread-id for the passed window
            uint dwThreadId = GetWindowThreadProcessId(hWnd, IntPtr.Zero);

            // verify that the hook's thread and the current thread are the same
            Debug.Assert(dwThreadId == GetCurrentThreadId(),
                "Keyboard Hook not running on the same thread as hooked window!");

            Install(dwThreadId);
        }

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        /// <summary>
        /// Install on the specified thread
        /// </summary>
        /// <param name="dwThreadId"></param>
        public void Install(uint dwThreadId)
        {
            // create the hook delegate and save a reference to it so that
            // it isn't garbage collected
            m_hookDelegate = new HookDelegate(this.KeyboardProc);

            // install the hook for this thread
            m_hHook = SetWindowsHookEx(WH.KEYBOARD, m_hookDelegate, IntPtr.Zero, dwThreadId);
            Trace.Assert(m_hHook != IntPtr.Zero, "Failed to install keyboard-hook!");
        }

        /// <summary>
        /// Determines whether the hook is currently installed
        /// </summary>
        public bool IsInstalled { get { return m_hHook != IntPtr.Zero; } }

        /// <summary>
        /// Remove the keyboard hook
        /// </summary>
        public void Remove()
        {
            // release hook if necessary (cleanly handle multiple remove calls)
            if (m_hHook != IntPtr.Zero)
            {
                // remove hook
                bool unhooked = UnhookWindowsHookEx(m_hHook);

                // verify success in debug mode (ignore in release)
                Debug.Assert(unhooked, String.Format(CultureInfo.InvariantCulture,
                    "Win32 error number {0} occurred when attempting to " +
                    "remove keyboard hook.", Marshal.GetLastWin32Error()));

                // set handle to zero to prevent unhooking twice
                m_hHook = IntPtr.Zero;
            }

            // set hook delegate to null (allow it to be garbage collected)
            m_hookDelegate = null;
        }

        /// <summary>
        /// call the next hook in the chain
        /// </summary>
        protected IntPtr CallNextHook(int nCode, UIntPtr wParam, IntPtr lParam)
        {
            return CallNextHookEx(m_hHook, nCode, wParam, lParam);
        }

        /// <summary>
        /// Keyboard hook handler (must be implemented by subclasses)
        /// </summary>
        /// <param name="nCode">HC_ACTION or HC_NOREMOVE</param>
        /// <param name="wParam">Virtual key code</param>
        /// <param name="lParam">Key state flags</param>
        /// <returns>1 to indicate key handled, otherwise next hook</returns>
        protected abstract IntPtr OnKeyHooked(int nCode, UIntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Keyboard hook handler.
        /// </summary>
        /// <param name="nCode">HC_ACTION or HC_NOREMOVE</param>
        /// <param name="wParam">Virtual key code</param>
        /// <param name="lParam">Key state flags</param>
        /// <returns>1 to indicate key handled, otherwise next hook</returns>
        private IntPtr KeyboardProc(int nCode, UIntPtr wParam, IntPtr lParam)
        {
            try
            {
                return OnKeyHooked(nCode, wParam, lParam);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Caught exception while handling keyboard hook: " + ex.ToString());
                return new IntPtr(1); // suppress key from editor
            }
        }

        /// <summary>
        /// handle to keyboard hook
        /// </summary>
        private IntPtr m_hHook = IntPtr.Zero;

        /// <summary>
        /// Delegate for KeyboardProc (hold on to a reference to it so that it
        /// isn't garbage collected)
        /// </summary>
        private HookDelegate m_hookDelegate;


        //PInvoke

        /// <summary>
        /// Delegate used for SetWindowsHookEx
        /// </summary>
        public delegate IntPtr HookDelegate(int nCode, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(WH hookType, HookDelegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);

        public enum WH : int
        {
            MSGFILTER = -1,
            JOURNALRECORD = 0,
            JOURNALPLAYBACK = 1,
            KEYBOARD = 2,
            GETMESSAGE = 3,
            CALLWNDPROC = 4,
            CBT = 5,
            SYSMSGFILTER = 6,
            MOUSE = 7,
            HARDWARE = 8,
            DEBUG = 9,
            SHELL = 10,
            FOREGROUNDIDLE = 11,
            CALLWNDPROCRET = 12,
            KEYBOARD_LL = 13,
            MOUSE_LL = 14
        }
    }
}
