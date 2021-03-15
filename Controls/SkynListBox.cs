using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

class SkynListBox : ListBox
{
    public SkynListBox()
    {
        this.SetStyle((System.Windows.Forms.ControlStyles)(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint), true);
        this.DrawMode = DrawMode.OwnerDrawFixed;
        IntegralHeight = false;
        ItemHeight = 18;
        Font = new Font("Seoge UI", 11, FontStyle.Regular);
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        try
        {
            base.OnDrawItem(e);
            e.DrawBackground();
            LinearGradientBrush LGB = new LinearGradientBrush(e.Bounds, Color.FromArgb(246, 132, 85), Color.FromArgb(231, 108, 57), 90.0F);
            if (System.Convert.ToInt32((e.State & DrawItemState.Selected)) == (int)DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(LGB, e.Bounds);
            }
            using (SolidBrush b = new SolidBrush(e.ForeColor))
            {
                if (base.Items.Count == 0)
                {
                    return;
                }
                else
                {
                    e.Graphics.DrawString(base.GetItemText(base.Items[e.Index]), e.Font, b, e.Bounds);
                }
            }

            LGB.Dispose();

        }
        catch { }
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Region MyRegion = new Region(e.ClipRectangle);
        e.Graphics.FillRegion(new SolidBrush(this.BackColor), MyRegion);

        if (this.Items.Count > 0)
        {
            for (int i = 0; i <= this.Items.Count - 1; i++)
            {
                System.Drawing.Rectangle RegionRect = this.GetItemRectangle(i);
                if (e.ClipRectangle.IntersectsWith(RegionRect))
                {
                    if ((this.SelectionMode == SelectionMode.One && this.SelectedIndex == i) || (this.SelectionMode == SelectionMode.MultiSimple && this.SelectedIndices.Contains(i)) || (this.SelectionMode == SelectionMode.MultiExtended && this.SelectedIndices.Contains(i)))
                    {
                        OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font, RegionRect, i, DrawItemState.Selected, this.ForeColor, this.BackColor));
                    }
                    else
                    {
                        OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font, RegionRect, i, DrawItemState.Default, Color.FromArgb(60, 60, 60), this.BackColor));
                    }
                    MyRegion.Complement(RegionRect);
                }
            }
        }
    }







    /*
    public SkynListBox()
    {
        Font = new Font("Segoe UI", 9F);
        DrawItem += MetroListView_DrawItem;
        base.SelectedIndexChanged += MetroListView_SelectedIndexChanged;
        base.Controls.Add(_vScrollbar);
        _vScrollbar.Visible = false;
        _vScrollbar.Width = 15;
        _vScrollbar.Dock = DockStyle.Right;
        _vScrollbar.ValueChanged += _vScrollbar_ValueChanged;

        _vScrollbar.UseBarColor = true;

    }
    private void MetroListView_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateScrollbar();
    }
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
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);


    private int _disableChangeEvents;

    private MetroScrollBar _vScrollbar = new MetroScrollBar();

    protected override void OnMouseWheel(MouseEventArgs e)
    {

        base.OnMouseWheel(e);
        if (_vScrollbar != null)
        {
            _vScrollbar.Value -= 1 * Math.Sign(e.Delta);
        }
    }
    public delegate void ScrollPositionChangedDelegate(SkynListBox listview, int pos);
    public event ScrollPositionChangedDelegate ScrollPositionChanged;

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        if (!base.DesignMode)
        {
            WinApi.ShowScrollBar(base.Handle, 3, 0);
        }

    }
    public static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong)
    {
        if (IntPtr.Size == 4)
        {
            return (int)SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
        }
        return (int)(long)SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
    }
    public static int GetWindowLong(IntPtr hWnd, int nIndex)
    {
        if (IntPtr.Size == 4)
        {
            return (int)GetWindowLong32(hWnd, nIndex);
        }
        return (int)(long)GetWindowLongPtr64(hWnd, nIndex);
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
            //for (int i = 0; i < 10; i++)
            //{
            //    if (base.TopItem != null && base.TopItem.Index != pos)
            //    {
            //        base.TopItem = base.Items[pos];
            //    }
            //}
            ResumeLayout();
        }
    }
    private void MetroListView_DrawItem(object sender, DrawItemEventArgs e)
    {
        Rectangle bounds2 = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
        Color color = Color.Red;
        TextFormatFlags textFormatFlags2 = TextFormatFlags.Default;
        TextRenderer.DrawText(e.Graphics, e.ToString(), new Font("Segoe UI", 9f), bounds2, color, textFormatFlags2 | TextFormatFlags.SingleLine | TextFormatFlags.WordEllipsis);

    }
    [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetWindowLong")]
    public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetWindowLongPtr")]
    public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
    public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
    public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, int dwNewLong);
    */
}
