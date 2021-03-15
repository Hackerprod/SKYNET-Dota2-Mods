using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class SkynListView : ListView
{
    private struct SCROLLINFO
    {
        public uint cbSize;

        public uint fMask;

        public int nMin;

        public int nMax;

        public uint nPage;

        public int nPos;

        public int nTrackPos;
    }

    private enum ScrollBarDirection
    {
        SB_HORZ,
        SB_VERT,
        SB_CTL,
        SB_BOTH
    }

    private enum ScrollInfoMask
    {
        SIF_RANGE = 1,
        SIF_PAGE = 2,
        SIF_POS = 4,
        SIF_DISABLENOSCROLL = 8,
        SIF_TRACKPOS = 0x10,
        SIF_ALL = 23
    }

    private enum SBTYPES
    {
        SB_HORZ,
        SB_VERT,
        SB_CTL,
        SB_BOTH
    }

    private enum LPCSCROLLINFO
    {
        SIF_RANGE = 1,
        SIF_PAGE = 2,
        SIF_POS = 4,
        SIF_DISABLENOSCROLL = 8,
        SIF_TRACKPOS = 0x10,
        SIF_ALL = 23
    }

    private struct LVITEM
    {
        public uint mask;

        public int iItem;

        public int iSubItem;

        public uint state;

        public uint stateMask;

        public IntPtr pszText;

        public int cchTextMax;

        public int iImage;

        public IntPtr lParam;
    }

    public enum ScrollBarCommands
    {
        SB_LINEUP = 0,
        SB_LINELEFT = 0,
        SB_LINEDOWN = 1,
        SB_LINERIGHT = 1,
        SB_PAGEUP = 2,
        SB_PAGELEFT = 2,
        SB_PAGEDOWN = 3,
        SB_PAGERIGHT = 3,
        SB_THUMBPOSITION = 4,
        SB_THUMBTRACK = 5,
        SB_TOP = 6,
        SB_LEFT = 6,
        SB_BOTTOM = 7,
        SB_RIGHT = 7,
        SB_ENDSCROLL = 8
    }

    public delegate void ScrollPositionChangedDelegate(SkynListView listview, int pos);

    private const uint WM_VSCROLL = 277u;

    private const uint WM_NCCALCSIZE = 131u;

    private const uint LVM_FIRST = 4096u;

    private const uint LVM_INSERTITEMA = 4103u;

    private const uint LVM_INSERTITEMW = 4173u;

    private const uint LVM_DELETEITEM = 4104u;

    private const uint LVM_DELETEALLITEMS = 4105u;

    private const int GWL_STYLE = -16;

    private const int WS_VSCROLL = 2097152;

    private ListViewColumnSorter lvwColumnSorter;

    private Font stdFont = new System.Drawing.Font("Segoe UI", 9F);

    private float _offset = 0.2f;


    private bool useCustomBackColor;

    private bool useCustomForeColor;

    private bool useStyleColors;

    private int _disableChangeEvents;

    private MetroScrollBar _vScrollbar = new MetroScrollBar();

    private bool allowSorting;


    [Category("Metro Appearance")]
    [DefaultValue(false)]
    public bool UseCustomBackColor
    {
        get
        {
            return useCustomBackColor;
        }
        set
        {
            useCustomBackColor = value;
        }
    }

    [Category("Metro Appearance")]
    [DefaultValue(false)]
    public bool UseCustomForeColor
    {
        get
        {
            return useCustomForeColor;
        }
        set
        {
            useCustomForeColor = value;
        }
    }

    [Category("Metro Appearance")]
    [DefaultValue(false)]
    public bool UseStyleColors
    {
        get
        {
            return useStyleColors;
        }
        set
        {
            useStyleColors = value;
        }
    }

    [DefaultValue(false)]
    [Category("Metro Behaviour")]
    [Browsable(false)]
    public bool UseSelectable
    {
        get
        {
            return GetStyle(ControlStyles.Selectable);
        }
        set
        {
            SetStyle(ControlStyles.Selectable, value);
        }
    }

    [Category("Metro Behaviour")]
    [DefaultValue(false)]
    public bool AllowSorting
    {
        get
        {
            return allowSorting;
        }
        set
        {
            allowSorting = value;
            if (!value)
            {
                lvwColumnSorter = null;
                base.ListViewItemSorter = null;
            }
            else
            {
                lvwColumnSorter = new ListViewColumnSorter();
                base.ListViewItemSorter = lvwColumnSorter;
            }
        }
    }

    [Browsable(false)]
    [Description("Set the font of the button caption")]
    public override Font Font
    {
        get
        {
            return base.Font;
        }
        set
        {
            base.Font = value;
        }
    }

    [Category("Metro Appearance")]
    public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;

    [Category("Metro Appearance")]
    public event EventHandler<MetroPaintEventArgs> CustomPaint;

    [Category("Metro Appearance")]
    public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;

    public event ScrollPositionChangedDelegate ScrollPositionChanged;

    public event Action<SkynListView> ItemAdded;

    public event Action<SkynListView> ItemsRemoved;

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);

    private void BeginDisableChangeEvents()
    {
        _disableChangeEvents++;
    }

    private void EndDisableChangeEvents()
    {
        if (_disableChangeEvents > 0)
        {
            _disableChangeEvents--;
        }
    }

    private void _vScrollbar_ValueChanged(object sender, int newValue)
    {
        if (_disableChangeEvents <= 0)
        {
            SetScrollPosition(_vScrollbar.Value);
        }
    }

    public void GetScrollPosition(out int min, out int max, out int pos, out int smallchange, out int largechange)
    {
        SCROLLINFO lpsi = default(SCROLLINFO);
        lpsi.cbSize = (uint)Marshal.SizeOf(typeof(SCROLLINFO));
        lpsi.fMask = 23u;
        if (GetScrollInfo(base.Handle, 1, ref lpsi))
        {
            min = lpsi.nMin;
            max = lpsi.nMax;
            pos = lpsi.nPos + 1;
            smallchange = 1;
            largechange = (int)lpsi.nPage;
        }
        else
        {
            min = 0;
            max = 0;
            pos = 0;
            smallchange = 0;
            largechange = 0;
        }
    }

    public void UpdateScrollbar()
    {
        if (_vScrollbar != null)
        {
            GetScrollPosition(out int min, out int max, out int pos, out int smallchange, out int largechange);
            BeginDisableChangeEvents();
            _vScrollbar.Value = pos;
            _vScrollbar.Maximum = max - largechange + 1;
            _vScrollbar.Minimum = min;
            _vScrollbar.SmallChange = smallchange;
            _vScrollbar.LargeChange = largechange;
            _vScrollbar.Visible = (_vScrollbar.Maximum != 101);
            EndDisableChangeEvents();
        }
    }

    public void SetScrollPosition(int pos)
    {
        pos = Math.Min(base.Items.Count - 1, pos);
        if (pos >= 0 && pos < base.Items.Count)
        {
            SuspendLayout();
            EnsureVisible(pos);
            for (int i = 0; i < 10; i++)
            {
                if (base.TopItem != null && base.TopItem.Index != pos)
                {
                    base.TopItem = base.Items[pos];
                }
            }
            ResumeLayout();
        }
    }

    protected void OnItemAdded()
    {
        if (_disableChangeEvents <= 0)
        {
            UpdateScrollbar();
            if (this.ItemAdded != null)
            {
                this.ItemAdded(this);
            }
        }
    }

    protected void OnItemsRemoved()
    {
        if (_disableChangeEvents <= 0)
        {
            UpdateScrollbar();
            if (this.ItemsRemoved != null)
            {
                this.ItemsRemoved(this);
            }
        }
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        if (_vScrollbar != null)
        {
            _vScrollbar.Value -= 3 * Math.Sign(e.Delta);
        }
    }

    protected override void WndProc(ref Message m)
    {
        if ((long)m.Msg == 277)
        {
            GetScrollPosition(out int _, out int _, out int pos, out int _, out int _);
            if (this.ScrollPositionChanged != null)
            {
                this.ScrollPositionChanged(this, pos);
            }
            if (_vScrollbar != null)
            {
                _vScrollbar.Value = pos;
            }
        }
        else if ((long)m.Msg == 131)
        {
            int windowLong = GetWindowLong(base.Handle, -16);
            if ((windowLong & 0x200000) == 2097152)
            {
                SetWindowLong(base.Handle, -16, windowLong & -2097153);
            }
        }
        else if ((long)m.Msg == 4103 || (long)m.Msg == 4173)
        {
            OnItemAdded();
        }
        else if ((long)m.Msg == 4104 || (long)m.Msg == 4105)
        {
            OnItemsRemoved();
        }
        base.WndProc(ref m);
    }

    public static int GetWindowLong(IntPtr hWnd, int nIndex)
    {
        if (IntPtr.Size == 4)
        {
            return (int)GetWindowLong32(hWnd, nIndex);
        }
        return (int)(long)GetWindowLongPtr64(hWnd, nIndex);
    }

    public static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong)
    {
        if (IntPtr.Size == 4)
        {
            return (int)SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
        }
        return (int)(long)SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetWindowLong")]
    public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetWindowLongPtr")]
    public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
    public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
    public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, int dwNewLong);

    public SkynListView()
    {
        Font = new Font("Segoe UI", 9F);
        base.HideSelection = true;
        base.OwnerDraw = true;
        base.DrawColumnHeader += MetroListView_DrawColumnHeader;
        base.DrawItem += MetroListView_DrawItem;
        base.DrawSubItem += MetroListView_DrawSubItem;
        base.Resize += MetroListView_Resize;
        base.ColumnClick += MetroListView_ColumnClick;
        base.SelectedIndexChanged += MetroListView_SelectedIndexChanged;
        base.FullRowSelect = true;
        base.Controls.Add(_vScrollbar);
        _vScrollbar.Visible = false;
        _vScrollbar.Width = 15;
        _vScrollbar.Dock = DockStyle.Right;
        _vScrollbar.ValueChanged += _vScrollbar_ValueChanged;
    }

    private void MetroListView_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateScrollbar();
    }

    private void MetroListView_ColumnClick(object sender, ColumnClickEventArgs e)
    {
        if (lvwColumnSorter != null)
        {
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }
            Sort();
        }
    }

    private void MetroListView_Resize(object sender, EventArgs e)
    {
        int count = base.Columns.Count;
    }

    private void MetroListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
    {
        Color color = Color.FromArgb(147, 157, 160);

        if (base.View == View.Details)
        {
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(47, 48, 51)), e.Bounds);
                color = Color.White;
            }
            TextFormatFlags textFormatFlags = TextFormatFlags.Default;
            int num = 0;
            int num2 = 0;
            if (base.CheckBoxes && e.ColumnIndex == 0)
            {
                num = 12;
                num2 = 14;
                int num3 = e.Bounds.Height / 2 - 6;
                using (Pen pen = new Pen(color))
                {
                    Rectangle rect = new Rectangle(e.Bounds.X + 2, e.Bounds.Y + num3, 12, 12);
                    e.Graphics.DrawRectangle(pen, rect);
                }
                if (e.Item.Checked)
                {
                    Color color2 = Color.Blue;
                    if (e.Item.Selected)
                    {
                        color2 = Color.White;
                    }
                    using (SolidBrush brush = new SolidBrush(color2))
                    {
                        num3 = e.Bounds.Height / 2 - 4;
                        Rectangle rect2 = new Rectangle(e.Bounds.X + 4, e.Bounds.Y + num3, 9, 9);
                        e.Graphics.FillRectangle(brush, rect2);
                    }
                }
            }
            if (base.SmallImageList != null)
            {
                int num4 = 0;
                Image image = null;
                if (e.Item.ImageIndex > -1)
                {
                    image = base.SmallImageList.Images[e.Item.ImageIndex];
                }
                if (e.Item.ImageKey != "")
                {
                    image = base.SmallImageList.Images[e.Item.ImageKey];
                }
                if (image != null)
                {
                    num2 += ((num2 > 0) ? 4 : 2);
                    num4 = (e.Item.Bounds.Height - image.Height) / 2;
                    e.Graphics.DrawImage(image, new Rectangle(e.Item.Bounds.Left + num2, e.Item.Bounds.Top + num4, image.Width, image.Height));
                    num2 += base.SmallImageList.ImageSize.Width;
                    num += base.SmallImageList.ImageSize.Width;
                }
            }
            int width = e.Item.Bounds.Width;
            if (base.View == View.Details)
            {
                width = base.Columns[0].Width;
            }
            using (StringFormat stringFormat = new StringFormat())
            {
                switch (e.Header.TextAlign)
                {
                    case HorizontalAlignment.Center:
                        stringFormat.Alignment = StringAlignment.Center;
                        break;
                    case HorizontalAlignment.Right:
                        stringFormat.Alignment = StringAlignment.Far;
                        break;
                }
                if (e.ColumnIndex > 0 && double.TryParse(e.SubItem.Text, NumberStyles.Currency, NumberFormatInfo.CurrentInfo, out double _))
                {
                    stringFormat.Alignment = StringAlignment.Far;
                }
                TextRenderer.DrawText(bounds: new Rectangle(e.Bounds.X + num2, e.Bounds.Y, 300, e.Item.Bounds.Height), dc: e.Graphics, text: e.SubItem.Text, font: stdFont, foreColor: color, flags: textFormatFlags | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis);
            }
        }
        else
        {
            e.DrawDefault = true;
        }
    }

    private void MetroListView_DrawItem(object sender, DrawListViewItemEventArgs e)
    {
        Color color = Color.Red; 
        if ((base.View == View.Details) | (base.View == View.List) | (base.View == View.SmallIcon))
        {
            Color color2 = Color.Gray;
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.Green), e.Bounds);
                color = Color.White;
                color2 = Color.White;
            }
            TextFormatFlags textFormatFlags = TextFormatFlags.Default;
            int num = 0;
            int num2 = 0;
            if (base.CheckBoxes)
            {
                num = 12;
                num2 = 14;
                int num3 = e.Bounds.Height / 2 - 6;
                using (Pen pen = new Pen(color))
                {
                    Rectangle rect = new Rectangle(e.Bounds.X + 2, e.Bounds.Y + num3, 12, 12);
                    e.Graphics.DrawRectangle(pen, rect);
                }
                if (e.Item.Checked)
                {
                    using (SolidBrush brush = new SolidBrush(color2))
                    {
                        num3 = e.Bounds.Height / 2 - 4;
                        Rectangle rect2 = new Rectangle(e.Bounds.X + 4, e.Bounds.Y + num3, 9, 9);
                        e.Graphics.FillRectangle(brush, rect2);
                    }
                }
            }
            if (base.SmallImageList != null)
            {
                int num4 = 0;
                Image image = null;
                if (e.Item.ImageIndex > -1)
                {
                    image = base.SmallImageList.Images[e.Item.ImageIndex];
                }
                if (e.Item.ImageKey != "")
                {
                    image = base.SmallImageList.Images[e.Item.ImageKey];
                }
                if (image != null)
                {
                    num2 += ((num2 > 0) ? 4 : 2);
                    num4 = (e.Item.Bounds.Height - image.Height) / 2;
                    e.Graphics.DrawImage(image, new Rectangle(e.Item.Bounds.Left + num2, e.Item.Bounds.Top + num4, image.Width, image.Height));
                    num2 += base.SmallImageList.ImageSize.Width;
                    num += base.SmallImageList.ImageSize.Width;
                }
            }
            if (base.View != View.Details)
            {
                int width = e.Item.Bounds.Width;
                if (base.View == View.Details)
                {
                    width = base.Columns[0].Width;
                }
                TextRenderer.DrawText(bounds: new Rectangle(e.Bounds.X + num2, e.Bounds.Y, width - num, e.Item.Bounds.Height), dc: e.Graphics, text: e.Item.Text, font: stdFont, foreColor: color, flags: textFormatFlags | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis);
            }
        }
        else if (base.View == View.Tile)
        {
            int num5 = 0;
            if (base.LargeImageList != null)
            {
                int num6 = 0;
                num5 = base.LargeImageList.ImageSize.Width + 2;
                Image image2 = null;
                if (e.Item.ImageIndex > -1)
                {
                    image2 = base.LargeImageList.Images[e.Item.ImageIndex];
                }
                if (e.Item.ImageKey != "")
                {
                    image2 = base.LargeImageList.Images[e.Item.ImageKey];
                }
                if (image2 != null)
                {
                    num6 = (e.Item.Bounds.Height - image2.Height) / 2;
                    e.Graphics.DrawImage(image2, new Rectangle(e.Item.Bounds.Left + num5, e.Item.Bounds.Top + num6, image2.Width, image2.Height));
                }
            }
            if (e.Item.Selected)
            {
                Rectangle rect3 = new Rectangle(e.Item.Bounds.X + num5, e.Item.Bounds.Y, e.Item.Bounds.Width, e.Item.Bounds.Height);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(250, 194, 87)), rect3);
            }
            int num7 = 0;
            foreach (ListViewItem.ListViewSubItem subItem in e.Item.SubItems)
            {
                if (num7 > 0 && !e.Item.Selected)
                {
                    color = Color.Silver;
                }
                int y = e.Item.Bounds.Y;
                int num9 = (e.Item.Bounds.Height - e.Item.SubItems.Count * 15) / 2;
                Rectangle bounds2 = new Rectangle(e.Item.Bounds.X + num5, e.Item.Bounds.Y + num7, e.Item.Bounds.Width, e.Item.Bounds.Height);
                TextFormatFlags textFormatFlags2 = TextFormatFlags.Default;
                TextRenderer.DrawText(e.Graphics, subItem.Text, new Font("Segoe UI", 9f), bounds2, color, textFormatFlags2 | TextFormatFlags.SingleLine | TextFormatFlags.WordEllipsis);
                num7 += 15;
            }
        }
        else
        {
            if (base.CheckBoxes)
            {
                int num8 = e.Bounds.Height / 2 - 6;
                using (Pen pen2 = new Pen(Color.Black))
                {
                    Rectangle rect4 = new Rectangle(e.Bounds.X + 6, e.Bounds.Y + num8, 12, 12);
                    e.Graphics.DrawRectangle(pen2, rect4);
                }
                if (e.Item.Checked)
                {
                    Color color3 = Color.Gold;
                    if (e.Item.Selected)
                    {
                        color3 = Color.White;
                    }
                    using (SolidBrush brush2 = new SolidBrush(color3))
                    {
                        num8 = e.Bounds.Height / 2 - 4;
                        Rectangle rect5 = new Rectangle(e.Bounds.X + 8, e.Bounds.Y + num8, 9, 9);
                        e.Graphics.FillRectangle(brush2, rect5);
                    }
                }
                Rectangle r = new Rectangle(e.Bounds.X + 23, e.Bounds.Y + 1, e.Bounds.Width, e.Bounds.Height);
                e.Graphics.DrawString(e.Item.Text, stdFont, new SolidBrush(color), r);
            }
            Font = stdFont;
            e.DrawDefault = true;
        }
    }

    private void MetroListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
    {
        Color color = Color.FromArgb(147, 157, 160);
        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(47, 48, 51)), e.Bounds);
        using (StringFormat stringFormat = new StringFormat())
        {
            stringFormat.Alignment = StringAlignment.Near;
            e.Graphics.DrawString(e.Header.Text, stdFont, new SolidBrush(color), e.Bounds, stringFormat);
        }
    }
}
