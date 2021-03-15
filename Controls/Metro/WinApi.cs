using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
internal static class WinApi
{
    public struct POINT
    {
        public int x;

        public int y;

        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public struct SIZE
    {
        public int cx;

        public int cy;

        public SIZE(int cx, int cy)
        {
            this.cx = cx;
            this.cy = cy;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ARGB
    {
        public byte Blue;

        public byte Green;

        public byte Red;

        public byte Alpha;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BLENDFUNCTION
    {
        public byte BlendOp;

        public byte BlendFlags;

        public byte SourceConstantAlpha;

        public byte AlphaFormat;
    }

    public struct TCHITTESTINFO
    {
        public Point pt;

        public uint flags;
    }

    public struct RECT
    {
        public int Left;

        public int Top;

        public int Right;

        public int Bottom;

        public RECT(Rectangle rc)
        {
            Left = rc.Left;
            Top = rc.Top;
            Right = rc.Right;
            Bottom = rc.Bottom;
        }

        public Rectangle ToRectangle()
        {
            return Rectangle.FromLTRB(Left, Top, Right, Bottom);
        }
    }

    public struct NCCALCSIZE_PARAMS
    {
        public RECT rect0;

        public RECT rect1;

        public RECT rect2;

        public IntPtr lppos;
    }

    public struct MINMAXINFO
    {
        public POINT ptReserved;

        public POINT ptMaxSize;

        public POINT ptMaxPosition;

        public POINT ptMinTrackSize;

        public POINT ptMaxTrackSize;
    }

    public struct APPBARDATA
    {
        public uint cbSize;

        public IntPtr hWnd;

        public uint uCallbackMessage;

        public ABE uEdge;

        public RECT rc;

        public int lParam;
    }

    public struct WindowPos
    {
        public int hwnd;

        public int hWndInsertAfter;

        public int x;

        public int y;

        public int cx;

        public int cy;

        public int flags;
    }

    public enum ABM : uint
    {
        New,
        Remove,
        QueryPos,
        SetPos,
        GetState,
        GetTaskbarPos,
        Activate,
        GetAutoHideBar,
        SetAutoHideBar,
        WindowPosChanged,
        SetState
    }

    public enum ABE : uint
    {
        Left,
        Top,
        Right,
        Bottom
    }

    public enum ScrollBar
    {
        SB_HORZ,
        SB_VERT,
        SB_CTL,
        SB_BOTH
    }

    public enum HitTest
    {
        HTNOWHERE = 0,
        HTCLIENT = 1,
        HTCAPTION = 2,
        HTGROWBOX = 4,
        HTSIZE = 4,
        HTMINBUTTON = 8,
        HTMAXBUTTON = 9,
        HTLEFT = 10,
        HTRIGHT = 11,
        HTTOP = 12,
        HTTOPLEFT = 13,
        HTTOPRIGHT = 14,
        HTBOTTOM = 0xF,
        HTBOTTOMLEFT = 0x10,
        HTBOTTOMRIGHT = 17,
        HTREDUCE = 8,
        HTZOOM = 9,
        HTSIZEFIRST = 10,
        HTSIZELAST = 17,
        HTTRANSPARENT = -1
    }

    public enum TabControlHitTest
    {
        TCHT_NOWHERE = 1
    }

    public enum Messages : uint
    {
        WM_NULL = 0u,
        WM_CREATE = 1u,
        WM_DESTROY = 2u,
        WM_MOVE = 3u,
        WM_SIZE = 5u,
        WM_ACTIVATE = 6u,
        WM_SETFOCUS = 7u,
        WM_KILLFOCUS = 8u,
        WM_ENABLE = 10u,
        WM_SETREDRAW = 11u,
        WM_SETTEXT = 12u,
        WM_GETTEXT = 13u,
        WM_GETTEXTLENGTH = 14u,
        WM_PAINT = 0xF,
        WM_CLOSE = 0x10,
        WM_QUERYENDSESSION = 17u,
        WM_QUERYOPEN = 19u,
        WM_ENDSESSION = 22u,
        WM_QUIT = 18u,
        WM_ERASEBKGND = 20u,
        WM_SYSCOLORCHANGE = 21u,
        WM_SHOWWINDOW = 24u,
        WM_WININICHANGE = 26u,
        WM_SETTINGCHANGE = 26u,
        WM_DEVMODECHANGE = 27u,
        WM_ACTIVATEAPP = 28u,
        WM_FONTCHANGE = 29u,
        WM_TIMECHANGE = 30u,
        WM_CANCELMODE = 0x1F,
        WM_SETCURSOR = 0x20,
        WM_MOUSEACTIVATE = 33u,
        WM_CHILDACTIVATE = 34u,
        WM_QUEUESYNC = 35u,
        WM_GETMINMAXINFO = 36u,
        WM_PAINTICON = 38u,
        WM_ICONERASEBKGND = 39u,
        WM_NEXTDLGCTL = 40u,
        WM_SPOOLERSTATUS = 42u,
        WM_DRAWITEM = 43u,
        WM_MEASUREITEM = 44u,
        WM_DELETEITEM = 45u,
        WM_VKEYTOITEM = 46u,
        WM_CHARTOITEM = 47u,
        WM_SETFONT = 48u,
        WM_GETFONT = 49u,
        WM_SETHOTKEY = 50u,
        WM_GETHOTKEY = 51u,
        WM_QUERYDRAGICON = 55u,
        WM_COMPAREITEM = 57u,
        WM_GETOBJECT = 61u,
        WM_COMPACTING = 65u,
        WM_COMMNOTIFY = 68u,
        WM_WINDOWPOSCHANGING = 70u,
        WM_WINDOWPOSCHANGED = 71u,
        WM_POWER = 72u,
        WM_COPYDATA = 74u,
        WM_CANCELJOURNAL = 75u,
        WM_NOTIFY = 78u,
        WM_INPUTLANGCHANGEREQUEST = 80u,
        WM_INPUTLANGCHANGE = 81u,
        WM_TCARD = 82u,
        WM_HELP = 83u,
        WM_USERCHANGED = 84u,
        WM_NOTIFYFORMAT = 85u,
        WM_CONTEXTMENU = 123u,
        WM_STYLECHANGING = 124u,
        WM_STYLECHANGED = 125u,
        WM_DISPLAYCHANGE = 126u,
        WM_GETICON = 0x7F,
        WM_SETICON = 0x80,
        WM_NCCREATE = 129u,
        WM_NCDESTROY = 130u,
        WM_NCCALCSIZE = 131u,
        WM_NCHITTEST = 132u,
        WM_NCPAINT = 133u,
        WM_NCACTIVATE = 134u,
        WM_GETDLGCODE = 135u,
        WM_SYNCPAINT = 136u,
        WM_NCMOUSEMOVE = 160u,
        WM_NCLBUTTONDOWN = 161u,
        WM_NCLBUTTONUP = 162u,
        WM_NCLBUTTONDBLCLK = 163u,
        WM_NCRBUTTONDOWN = 164u,
        WM_NCRBUTTONUP = 165u,
        WM_NCRBUTTONDBLCLK = 166u,
        WM_NCMBUTTONDOWN = 167u,
        WM_NCMBUTTONUP = 168u,
        WM_NCMBUTTONDBLCLK = 169u,
        WM_NCXBUTTONDOWN = 171u,
        WM_NCXBUTTONUP = 172u,
        WM_NCXBUTTONDBLCLK = 173u,
        WM_INPUT = 0xFF,
        WM_KEYFIRST = 0x100,
        WM_KEYDOWN = 0x100,
        WM_KEYUP = 257u,
        WM_CHAR = 258u,
        WM_DEADCHAR = 259u,
        WM_SYSKEYDOWN = 260u,
        WM_SYSKEYUP = 261u,
        WM_SYSCHAR = 262u,
        WM_SYSDEADCHAR = 263u,
        WM_UNICHAR = 265u,
        WM_KEYLAST = 264u,
        WM_IME_STARTCOMPOSITION = 269u,
        WM_IME_ENDCOMPOSITION = 270u,
        WM_IME_COMPOSITION = 271u,
        WM_IME_KEYLAST = 271u,
        WM_INITDIALOG = 272u,
        WM_COMMAND = 273u,
        WM_SYSCOMMAND = 274u,
        WM_TIMER = 275u,
        WM_HSCROLL = 276u,
        WM_VSCROLL = 277u,
        WM_INITMENU = 278u,
        WM_INITMENUPOPUP = 279u,
        WM_MENUSELECT = 287u,
        WM_MENUCHAR = 288u,
        WM_ENTERIDLE = 289u,
        WM_MENURBUTTONUP = 290u,
        WM_MENUDRAG = 291u,
        WM_MENUGETOBJECT = 292u,
        WM_UNINITMENUPOPUP = 293u,
        WM_MENUCOMMAND = 294u,
        WM_CHANGEUISTATE = 295u,
        WM_UPDATEUISTATE = 296u,
        WM_QUERYUISTATE = 297u,
        WM_CTLCOLOR = 25u,
        WM_CTLCOLORMSGBOX = 306u,
        WM_CTLCOLOREDIT = 307u,
        WM_CTLCOLORLISTBOX = 308u,
        WM_CTLCOLORBTN = 309u,
        WM_CTLCOLORDLG = 310u,
        WM_CTLCOLORSCROLLBAR = 311u,
        WM_CTLCOLORSTATIC = 312u,
        WM_MOUSEFIRST = 0x200,
        WM_MOUSEMOVE = 0x200,
        WM_LBUTTONDOWN = 513u,
        WM_LBUTTONUP = 514u,
        WM_LBUTTONDBLCLK = 515u,
        WM_RBUTTONDOWN = 516u,
        WM_RBUTTONUP = 517u,
        WM_RBUTTONDBLCLK = 518u,
        WM_MBUTTONDOWN = 519u,
        WM_MBUTTONUP = 520u,
        WM_MBUTTONDBLCLK = 521u,
        WM_MOUSEWHEEL = 522u,
        WM_XBUTTONDOWN = 523u,
        WM_XBUTTONUP = 524u,
        WM_XBUTTONDBLCLK = 525u,
        WM_MOUSELAST = 525u,
        WM_PARENTNOTIFY = 528u,
        WM_ENTERMENULOOP = 529u,
        WM_EXITMENULOOP = 530u,
        WM_NEXTMENU = 531u,
        WM_SIZING = 532u,
        WM_CAPTURECHANGED = 533u,
        WM_MOVING = 534u,
        WM_POWERBROADCAST = 536u,
        WM_DEVICECHANGE = 537u,
        WM_MDICREATE = 544u,
        WM_MDIDESTROY = 545u,
        WM_MDIACTIVATE = 546u,
        WM_MDIRESTORE = 547u,
        WM_MDINEXT = 548u,
        WM_MDIMAXIMIZE = 549u,
        WM_MDITILE = 550u,
        WM_MDICASCADE = 551u,
        WM_MDIICONARRANGE = 552u,
        WM_MDIGETACTIVE = 553u,
        WM_MDISETMENU = 560u,
        WM_ENTERSIZEMOVE = 561u,
        WM_EXITSIZEMOVE = 562u,
        WM_DROPFILES = 563u,
        WM_MDIREFRESHMENU = 564u,
        WM_IME_SETCONTEXT = 641u,
        WM_IME_NOTIFY = 642u,
        WM_IME_CONTROL = 643u,
        WM_IME_COMPOSITIONFULL = 644u,
        WM_IME_SELECT = 645u,
        WM_IME_CHAR = 646u,
        WM_IME_REQUEST = 648u,
        WM_IME_KEYDOWN = 656u,
        WM_IME_KEYUP = 657u,
        WM_MOUSEHOVER = 673u,
        WM_MOUSELEAVE = 675u,
        WM_NCMOUSELEAVE = 674u,
        WM_WTSSESSION_CHANGE = 689u,
        WM_TABLET_FIRST = 704u,
        WM_TABLET_LAST = 735u,
        WM_CUT = 768u,
        WM_COPY = 769u,
        WM_PASTE = 770u,
        WM_CLEAR = 771u,
        WM_UNDO = 772u,
        WM_RENDERFORMAT = 773u,
        WM_RENDERALLFORMATS = 774u,
        WM_DESTROYCLIPBOARD = 775u,
        WM_DRAWCLIPBOARD = 776u,
        WM_PAINTCLIPBOARD = 777u,
        WM_VSCROLLCLIPBOARD = 778u,
        WM_SIZECLIPBOARD = 779u,
        WM_ASKCBFORMATNAME = 780u,
        WM_CHANGECBCHAIN = 781u,
        WM_HSCROLLCLIPBOARD = 782u,
        WM_QUERYNEWPALETTE = 783u,
        WM_PALETTEISCHANGING = 784u,
        WM_PALETTECHANGED = 785u,
        WM_HOTKEY = 786u,
        WM_PRINT = 791u,
        WM_PRINTCLIENT = 792u,
        WM_APPCOMMAND = 793u,
        WM_THEMECHANGED = 794u,
        WM_HANDHELDFIRST = 856u,
        WM_HANDHELDLAST = 863u,
        WM_AFXFIRST = 864u,
        WM_AFXLAST = 895u,
        WM_PENWINFIRST = 896u,
        WM_PENWINLAST = 911u,
        WM_USER = 0x400,
        WM_REFLECT = 0x2000,
        WM_APP = 0x8000,
        WM_DWMCOMPOSITIONCHANGED = 798u,
        SC_MOVE = 61456u,
        SC_MINIMIZE = 61472u,
        SC_MAXIMIZE = 61488u,
        SC_RESTORE = 61728u
    }

    public enum Bool
    {
        False,
        True
    }

    public const int Autohide = 1;

    public const int AlwaysOnTop = 2;

    public const int MfByposition = 1024;

    public const int MfRemove = 4096;

    public const int TCM_HITTEST = 4883;

    public const int ULW_COLORKEY = 1;

    public const int ULW_ALPHA = 2;

    public const int ULW_OPAQUE = 4;

    public const byte AC_SRC_OVER = 0;

    public const byte AC_SRC_ALPHA = 1;

    public const int GW_HWNDFIRST = 0;

    public const int GW_HWNDLAST = 1;

    public const int GW_HWNDNEXT = 2;

    public const int GW_HWNDPREV = 3;

    public const int GW_OWNER = 4;

    public const int GW_CHILD = 5;

    public const int HC_ACTION = 0;

    public const int WH_CALLWNDPROC = 4;

    public const int GWL_WNDPROC = -4;

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern Bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pprSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern Bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll", ExactSpelling = true)]
    public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern Bool DeleteObject(IntPtr hObject);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int W, int H, uint uFlags);

    [DllImport("user32.dll")]
    public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport("user32.dll")]
    public static extern int GetMenuItemCount(IntPtr hMenu);

    [DllImport("user32.dll")]
    public static extern bool DrawMenuBar(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    public static extern IntPtr SetCapture(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr wnd, int msg, bool param, int lparam);

    [DllImport("shell32.dll", SetLastError = true)]
    public static extern IntPtr SHAppBarMessage(ABM dwMessage, [In] ref APPBARDATA pData);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr GetDCEx(IntPtr hwnd, IntPtr hrgnclip, uint fdwOptions);

    [DllImport("user32.dll")]
    public static extern bool ShowScrollBar(IntPtr hWnd, int bar, int cmd);

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr GetWindowDC(IntPtr handle);

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr ReleaseDC(IntPtr handle, IntPtr hDC);

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern int GetClassName(IntPtr hwnd, char[] className, int maxCount);

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr GetWindow(IntPtr hwnd, int uCmd);

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern bool IsWindowVisible(IntPtr hwnd);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern int GetClientRect(IntPtr hwnd, ref RECT lpRect);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern int GetClientRect(IntPtr hwnd, [In] [Out] ref Rectangle rect);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern bool MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern bool UpdateWindow(IntPtr hwnd);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern bool InvalidateRect(IntPtr hwnd, ref Rectangle rect, bool bErase);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern bool ValidateRect(IntPtr hwnd, ref Rectangle rect);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    internal static extern bool GetWindowRect(IntPtr hWnd, [In] [Out] ref Rectangle rect);

    public static int LoWord(int dwValue)
    {
        return dwValue & 0xFFFF;
    }

    public static int HiWord(int dwValue)
    {
        return (dwValue >> 16) & 0xFFFF;
    }
}