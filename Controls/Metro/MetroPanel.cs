using System;
using System.ComponentModel;
using System.Drawing;
using System.Security;
using System.Windows.Forms;

[ToolboxBitmap(typeof(Panel))]
public class MetroPanel : Panel
{
    private MetroScrollBar verticalScrollbar = new MetroScrollBar(MetroScrollOrientation.Vertical);

    private MetroScrollBar horizontalScrollbar = new MetroScrollBar(MetroScrollOrientation.Horizontal);
    private bool showHorizontalScrollbar;
    private bool showVerticalScrollbar;

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

    [Category("Metro Appearance")]
    [DefaultValue(false)]
    public bool HorizontalScrollbar
    {
        get
        {
            return showHorizontalScrollbar;
        }
        set
        {
            showHorizontalScrollbar = value;
        }
    }

    [Category("Metro Appearance")]
    public int HorizontalScrollbarSize
    {
        get
        {
            return horizontalScrollbar.ScrollbarSize;
        }
        set
        {
            horizontalScrollbar.ScrollbarSize = value;
        }
    }

    [Category("Metro Appearance")]
    public bool HorizontalScrollbarBarColor
    {
        get
        {
            return horizontalScrollbar.UseBarColor;
        }
        set
        {
            horizontalScrollbar.UseBarColor = value;
        }
    }

    [Category("Metro Appearance")]
    public bool HorizontalScrollbarHighlightOnWheel
    {
        get
        {
            return horizontalScrollbar.HighlightOnWheel;
        }
        set
        {
            horizontalScrollbar.HighlightOnWheel = value;
        }
    }

    [Category("Metro Appearance")]
    [DefaultValue(false)]
    public bool VerticalScrollbar
    {
        get
        {
            return showVerticalScrollbar;
        }
        set
        {
            showVerticalScrollbar = value;
        }
    }

    [Category("Metro Appearance")]
    public int VerticalScrollbarSize
    {
        get
        {
            return verticalScrollbar.ScrollbarSize;
        }
        set
        {
            verticalScrollbar.ScrollbarSize = value;
        }
    }

    [Category("Metro Appearance")]
    public bool VerticalScrollbarBarColor
    {
        get
        {
            return verticalScrollbar.UseBarColor;
        }
        set
        {
            verticalScrollbar.UseBarColor = value;
        }
    }

    [Category("Metro Appearance")]
    public bool VerticalScrollbarHighlightOnWheel
    {
        get
        {
            return verticalScrollbar.HighlightOnWheel;
        }
        set
        {
            verticalScrollbar.HighlightOnWheel = value;
        }
    }

    [Category("Metro Appearance")]
    public new bool AutoScroll
    {
        get
        {
            return base.AutoScroll;
        }
        set
        {
            showHorizontalScrollbar = value;
            showVerticalScrollbar = value;
            base.AutoScroll = value;
        }
    }

    [Category("Metro Appearance")]
    public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;

    [Category("Metro Appearance")]
    public event EventHandler<MetroPaintEventArgs> CustomPaint;

    [Category("Metro Appearance")]
    public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;

    protected virtual void OnCustomPaintBackground(MetroPaintEventArgs e)
    {
        if (GetStyle(ControlStyles.UserPaint) && this.CustomPaintBackground != null)
        {
            this.CustomPaintBackground(this, e);
        }
    }

    protected virtual void OnCustomPaint(MetroPaintEventArgs e)
    {
        if (GetStyle(ControlStyles.UserPaint) && this.CustomPaint != null)
        {
            this.CustomPaint(this, e);
        }
    }

    protected virtual void OnCustomPaintForeground(MetroPaintEventArgs e)
    {
        if (GetStyle(ControlStyles.UserPaint) && this.CustomPaintForeground != null)
        {
            this.CustomPaintForeground(this, e);
        }
    }

    public MetroPanel()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, value: true);
        base.Controls.Add(verticalScrollbar);
        base.Controls.Add(horizontalScrollbar);
        verticalScrollbar.UseBarColor = true;
        horizontalScrollbar.UseBarColor = true;
        verticalScrollbar.Visible = false;
        horizontalScrollbar.Visible = false;
        verticalScrollbar.Scroll += VerticalScrollbarScroll;
        horizontalScrollbar.Scroll += HorizontalScrollbarScroll;
    }

    private void HorizontalScrollbarScroll(object sender, ScrollEventArgs e)
    {
        base.AutoScrollPosition = new Point(e.NewValue, verticalScrollbar.Value);
        UpdateScrollBarPositions();
    }

    private void VerticalScrollbarScroll(object sender, ScrollEventArgs e)
    {
        base.AutoScrollPosition = new Point(horizontalScrollbar.Value, e.NewValue);
        UpdateScrollBarPositions();
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        try
        {
            Color color = BackColor;
            if (color.A == 255 && BackgroundImage == null)
            {
                e.Graphics.Clear(color);
            }
            else
            {
                base.OnPaintBackground(e);
                OnCustomPaintBackground(new MetroPaintEventArgs(color, Color.Empty, e.Graphics));
            }
        }
        catch
        {
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        try
        {
            if (GetStyle(ControlStyles.AllPaintingInWmPaint))
            {
                OnPaintBackground(e);
            }
            OnCustomPaint(new MetroPaintEventArgs(Color.Empty, Color.Empty, e.Graphics));
            OnPaintForeground(e);
        }
        catch
        {
            Invalidate();
        }
    }

    protected virtual void OnPaintForeground(PaintEventArgs e)
    {
        if (base.DesignMode)
        {
            horizontalScrollbar.Visible = false;
            verticalScrollbar.Visible = false;
        }
        else
        {
            UpdateScrollBarPositions();
            if (HorizontalScrollbar)
            {
                horizontalScrollbar.Visible = base.HorizontalScroll.Visible;
            }
            if (base.HorizontalScroll.Visible)
            {
                horizontalScrollbar.Minimum = base.HorizontalScroll.Minimum;
                horizontalScrollbar.Maximum = base.HorizontalScroll.Maximum;
                horizontalScrollbar.SmallChange = base.HorizontalScroll.SmallChange;
                horizontalScrollbar.LargeChange = base.HorizontalScroll.LargeChange;
            }
            if (VerticalScrollbar)
            {
                verticalScrollbar.Visible = base.VerticalScroll.Visible;
            }
            if (base.VerticalScroll.Visible)
            {
                verticalScrollbar.Minimum = base.VerticalScroll.Minimum;
                verticalScrollbar.Maximum = base.VerticalScroll.Maximum;
                verticalScrollbar.SmallChange = base.VerticalScroll.SmallChange;
                verticalScrollbar.LargeChange = base.VerticalScroll.LargeChange;
            }
            OnCustomPaintForeground(new MetroPaintEventArgs(Color.Empty, Color.Empty, e.Graphics));
        }
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        verticalScrollbar.Value = Math.Abs(base.VerticalScroll.Value);
        horizontalScrollbar.Value = Math.Abs(base.HorizontalScroll.Value);
    }

    [SecuritySafeCritical]
    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        if (!base.DesignMode)
        {
            WinApi.ShowScrollBar(base.Handle, 3, 0);
        }
    }

    private void UpdateScrollBarPositions()
    {
        if (!base.DesignMode)
        {
            if (!AutoScroll)
            {
                verticalScrollbar.Visible = false;
                horizontalScrollbar.Visible = false;
            }
            else
            {
                verticalScrollbar.Location = new Point(base.ClientRectangle.Width - verticalScrollbar.Width, base.ClientRectangle.Y);
                verticalScrollbar.Height = base.ClientRectangle.Height - horizontalScrollbar.Height;
                if (!VerticalScrollbar)
                {
                    verticalScrollbar.Visible = false;
                }
                horizontalScrollbar.Location = new Point(base.ClientRectangle.X, base.ClientRectangle.Height - horizontalScrollbar.Height);
                horizontalScrollbar.Width = base.ClientRectangle.Width - verticalScrollbar.Width;
                if (!HorizontalScrollbar)
                {
                    horizontalScrollbar.Visible = false;
                }
            }
        }
    }
}
public enum MetroScrollOrientation
{
    Horizontal,
    Vertical
}
