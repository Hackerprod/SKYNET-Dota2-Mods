using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

internal class FlatCheckBox : Control
{
    public delegate void CheckedChangedEventHandler(object sender);

    [Flags]
    public enum _Options
    {
        Style1 = 0x0,
    }

    private int W;

    private int H;

    private MouseState State;

    private _Options O;

    private bool _Checked;

    [CompilerGenerated]
    private CheckedChangedEventHandler CheckedChangedEvent;

    public bool Checked
    {
        get
        {
            return _Checked;
        }
        set
        {
            _Checked = value;
            Invalidate();
        }
    }
    private Color _BoxColor { get; set; } = Color.FromArgb(7, 164, 245);
    public Color BoxColor
    {
        get
        {
            return _BoxColor;
        }
        set
        {
            _BoxColor = value;
        }
    }
    private Color _BoxBackColor { get; set; } = Color.Transparent;
    public Color BoxBackColor
    {
        get
        {
            return _BoxBackColor;
        }
        set
        {
            _BoxBackColor = value;
        }
    }
    [Category("Options")]
    public _Options Options
    {
        get
        {
            return O;
        }
        set
        {
            O = value;
        }
    }

    public event CheckedChangedEventHandler CheckedChanged
    {
        [CompilerGenerated]
        add
        {
            CheckedChangedEventHandler checkedChangedEventHandler = CheckedChangedEvent;
            CheckedChangedEventHandler checkedChangedEventHandler2;
            do
            {
                checkedChangedEventHandler2 = checkedChangedEventHandler;
                CheckedChangedEventHandler value2 = (CheckedChangedEventHandler)Delegate.Combine(checkedChangedEventHandler2, value);
                checkedChangedEventHandler = Interlocked.CompareExchange(ref CheckedChangedEvent, value2, checkedChangedEventHandler2);
            }
            while ((object)checkedChangedEventHandler != checkedChangedEventHandler2);
        }
        [CompilerGenerated]
        remove
        {
            CheckedChangedEventHandler checkedChangedEventHandler = CheckedChangedEvent;
            CheckedChangedEventHandler checkedChangedEventHandler2;
            do
            {
                checkedChangedEventHandler2 = checkedChangedEventHandler;
                CheckedChangedEventHandler value2 = (CheckedChangedEventHandler)Delegate.Remove(checkedChangedEventHandler2, value);
                checkedChangedEventHandler = Interlocked.CompareExchange(ref CheckedChangedEvent, value2, checkedChangedEventHandler2);
            }
            while ((object)checkedChangedEventHandler != checkedChangedEventHandler2);
        }
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        Invalidate();
    }

    protected override void OnClick(EventArgs e)
    {
        _Checked = !_Checked;
        CheckedChangedEvent?.Invoke(this);
        base.OnClick(e);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        base.Height = 22;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        State = MouseState.Down;
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        State = MouseState.Over;
        Invalidate();
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        State = MouseState.Over;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        State = MouseState.None;
        Invalidate();
    }

    public FlatCheckBox()
    {
        State = MouseState.None;
        SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
        DoubleBuffered = true;
        Cursor = Cursors.Hand;
        Font = new Font("Segoe UI", 10f);
        base.Size = new Size(112, 22);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Helpers.B = new Bitmap(base.Width, base.Height);
        Helpers.G = Graphics.FromImage(Helpers.B);
        checked
        {
            W = base.Width - 1;
            H = base.Height - 1;
            Rectangle rect = new Rectangle(0, 2, base.Height - 5, base.Height - 5);
            Graphics g = Helpers.G;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.Clear(Color.Transparent);

            g.FillRectangle(new SolidBrush(_BoxBackColor), rect);
            switch (State)
            {
                case MouseState.Over:
                    g.DrawRectangle(new Pen(BoxColor), rect);
                    break;
                case MouseState.Down:
                    g.DrawRectangle(new Pen(BoxColor), rect);
                    break;
            }
            if (Checked)
            {
                g.DrawString("ü", new Font("Wingdings", 18f), new SolidBrush(BoxColor), new Rectangle(5, 7, H - 9, H - 9), Helpers.CenterSF);
            }
            if (!base.Enabled)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(54, 58, 61)), rect);
                g.DrawString(Text, Font, new SolidBrush(ForeColor), new Rectangle(20, 2, W, H), Helpers.NearSF);
            }
            g.DrawString(Text, Font, new SolidBrush(ForeColor), new Rectangle(20, 2, W, H), Helpers.NearSF);

            g = null;
            base.OnPaint(e);
            Helpers.G.Dispose();
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.DrawImageUnscaled(Helpers.B, 0, 0);
            Helpers.B.Dispose();
        }
    }
}
