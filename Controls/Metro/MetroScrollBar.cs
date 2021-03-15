using System;
using System.ComponentModel;
using System.Drawing;
using System.Security;
using System.Windows.Forms;

[DefaultEvent("Scroll")]
[Designer("MetroFramework.Design.Controls.MetroScrollBarDesigner, MetroFramework.Design, Version=1.4.0.0, Culture=neutral, PublicKeyToken=5f91a84759bf584a")]
[DefaultProperty("Value")]
public class MetroScrollBar : Control
{
    public delegate void ScrollValueChangedDelegate(object sender, int newValue);

    private bool useCustomBackColor;

    private bool useCustomForeColor;

    private bool useStyleColors;

    private bool isFirstScrollEventVertical = true;

    private bool isFirstScrollEventHorizontal = true;

    private bool inUpdate;

    private Rectangle clickedBarRectangle;

    private Rectangle thumbRectangle;

    private bool topBarClicked;

    private bool bottomBarClicked;

    private bool thumbClicked;

    private int thumbWidth = 6;

    private int thumbHeight;

    private int thumbBottomLimitBottom;

    private int thumbBottomLimitTop;

    private int thumbTopLimit;

    private int thumbPosition;

    private int trackPosition;

    private readonly Timer progressTimer = new Timer();

    private int mouseWheelBarPartitions = 10;

    private bool isHovered;

    private bool isPressed;

    private bool useBarColor;

    private bool highlightOnWheel;

    private MetroScrollOrientation metroOrientation = MetroScrollOrientation.Vertical;

    private ScrollOrientation scrollOrientation = ScrollOrientation.VerticalScroll;

    private int minimum;

    private int maximum = 100;

    private int smallChange = 1;

    private int largeChange = 10;

    private int curValue;

    private bool dontUpdateColor;

    private Timer autoHoverTimer;


    [DefaultValue(false)]
    [Category("Metro Appearance")]
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

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    [DefaultValue(false)]
    [Category("Metro Appearance")]
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

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [DefaultValue(false)]
    [Category("Metro Appearance")]
    [Browsable(false)]
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

    [Category("Metro Behaviour")]
    [DefaultValue(false)]
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

    public int MouseWheelBarPartitions
    {
        get
        {
            return mouseWheelBarPartitions;
        }
        set
        {
            if (value > 0)
            {
                mouseWheelBarPartitions = value;
                return;
            }
            throw new ArgumentOutOfRangeException("value", "MouseWheelBarPartitions has to be greather than zero");
        }
    }

    [DefaultValue(false)]
    [Category("Metro Appearance")]
    public bool UseBarColor
    {
        get
        {
            return useBarColor;
        }
        set
        {
            useBarColor = value;
        }
    }

    [Category("Metro Appearance")]
    public int ScrollbarSize
    {
        get
        {
            if (Orientation != MetroScrollOrientation.Vertical)
            {
                return base.Height;
            }
            return base.Width;
        }
        set
        {
            if (Orientation == MetroScrollOrientation.Vertical)
            {
                base.Width = value;
            }
            else
            {
                base.Height = value;
            }
        }
    }

    [Category("Metro Appearance")]
    [DefaultValue(false)]
    public bool HighlightOnWheel
    {
        get
        {
            return highlightOnWheel;
        }
        set
        {
            highlightOnWheel = value;
        }
    }

    public MetroScrollOrientation Orientation
    {
        get
        {
            return metroOrientation;
        }
        set
        {
            if (value != metroOrientation)
            {
                metroOrientation = value;
                if (value == MetroScrollOrientation.Vertical)
                {
                    scrollOrientation = ScrollOrientation.VerticalScroll;
                }
                else
                {
                    scrollOrientation = ScrollOrientation.HorizontalScroll;
                }
                base.Size = new Size(base.Height, base.Width);
                SetupScrollBar();
            }
        }
    }

    public int Minimum
    {
        get
        {
            return minimum;
        }
        set
        {
            if (minimum != value && value >= 0 && value < maximum)
            {
                minimum = value;
                if (curValue < value)
                {
                    curValue = value;
                }
                if (largeChange > maximum - minimum)
                {
                    largeChange = maximum - minimum;
                }
                SetupScrollBar();
                if (curValue < value)
                {
                    dontUpdateColor = true;
                    Value = value;
                }
                else
                {
                    ChangeThumbPosition(GetThumbPosition());
                    Refresh();
                }
            }
        }
    }

    public int Maximum
    {
        get
        {
            return maximum;
        }
        set
        {
            if (value != maximum && value >= 1 && value > minimum)
            {
                maximum = value;
                if (largeChange > maximum - minimum)
                {
                    largeChange = maximum - minimum;
                }
                SetupScrollBar();
                if (curValue > value)
                {
                    dontUpdateColor = true;
                    Value = maximum;
                }
                else
                {
                    ChangeThumbPosition(GetThumbPosition());
                    Refresh();
                }
            }
        }
    }

    [DefaultValue(1)]
    public int SmallChange
    {
        get
        {
            return smallChange;
        }
        set
        {
            if (value != smallChange && value >= 1 && value < largeChange)
            {
                smallChange = value;
                SetupScrollBar();
            }
        }
    }

    [DefaultValue(5)]
    public int LargeChange
    {
        get
        {
            return largeChange;
        }
        set
        {
            if (value != largeChange && value >= smallChange && value >= 2)
            {
                if (value > maximum - minimum)
                {
                    largeChange = maximum - minimum;
                }
                else
                {
                    largeChange = value;
                }
                SetupScrollBar();
            }
        }
    }

    [Browsable(false)]
    [DefaultValue(0)]
    public int Value
    {
        get
        {
            return curValue;
        }
        set
        {
            if (curValue != value && value >= minimum && value <= maximum)
            {
                curValue = value;
                ChangeThumbPosition(GetThumbPosition());
                OnScroll(ScrollEventType.ThumbPosition, -1, value, scrollOrientation);
                if (!dontUpdateColor && highlightOnWheel)
                {
                    if (!isHovered)
                    {
                        isHovered = true;
                    }
                    if (autoHoverTimer == null)
                    {
                        autoHoverTimer = new Timer();
                        autoHoverTimer.Interval = 1000;
                        autoHoverTimer.Tick += autoHoverTimer_Tick;
                        autoHoverTimer.Start();
                    }
                    else
                    {
                        autoHoverTimer.Stop();
                        autoHoverTimer.Start();
                    }
                }
                else
                {
                    dontUpdateColor = false;
                }
                Refresh();
            }
        }
    }

    [Category("Metro Appearance")]
    public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;

    [Category("Metro Appearance")]
    public event EventHandler<MetroPaintEventArgs> CustomPaint;

    [Category("Metro Appearance")]
    public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;

    public event ScrollEventHandler Scroll;

    public event ScrollValueChangedDelegate ValueChanged;

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

    private void OnScroll(ScrollEventType type, int oldValue, int newValue, ScrollOrientation orientation)
    {
        if (oldValue != newValue && this.ValueChanged != null)
        {
            this.ValueChanged(this, curValue);
        }
        if (this.Scroll != null)
        {
            if (orientation == ScrollOrientation.HorizontalScroll)
            {
                if (type != ScrollEventType.EndScroll && isFirstScrollEventHorizontal)
                {
                    type = ScrollEventType.First;
                }
                else if (!isFirstScrollEventHorizontal && type == ScrollEventType.EndScroll)
                {
                    isFirstScrollEventHorizontal = true;
                }
            }
            else if (type != ScrollEventType.EndScroll && isFirstScrollEventVertical)
            {
                type = ScrollEventType.First;
            }
            else if (!isFirstScrollEventHorizontal && type == ScrollEventType.EndScroll)
            {
                isFirstScrollEventVertical = true;
            }
            this.Scroll(this, new ScrollEventArgs(type, oldValue, newValue, orientation));
        }
    }

    private void autoHoverTimer_Tick(object sender, EventArgs e)
    {
        isHovered = false;
        Invalidate();
        autoHoverTimer.Stop();
    }

    public MetroScrollBar()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, value: true);
        base.Width = 10;
        base.Height = 200;
        SetupScrollBar();
        progressTimer.Interval = 20;
        progressTimer.Tick += ProgressTimerTick;
    }

    public MetroScrollBar(MetroScrollOrientation orientation)
        : this()
    {
        Orientation = orientation;
    }

    public MetroScrollBar(MetroScrollOrientation orientation, int width)
        : this(orientation)
    {
        base.Width = width;
    }

    public bool HitTest(Point point)
    {
        return thumbRectangle.Contains(point);
    }

    [SecuritySafeCritical]
    public void BeginUpdate()
    {
        WinApi.SendMessage(base.Handle, 11, param: false, 0);
        inUpdate = true;
    }

    [SecuritySafeCritical]
    public void EndUpdate()
    {
        WinApi.SendMessage(base.Handle, 11, param: true, 0);
        inUpdate = false;
        SetupScrollBar();
        Refresh();
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        try
        {
            Color color = BackColor;
            if (!useCustomBackColor)
            {
                color = Color.FromArgb(31, 32, 35);
            }
            if (color.A == 255)
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
        Color backColor = Color.FromArgb(108, 108, 108);
        Color color;
        Color barColor;
        if (isHovered && !isPressed && base.Enabled)
        {
            color = Color.FromArgb(151, 151, 151);
            barColor = Color.FromArgb(31, 32, 35);
        }
        else if (isHovered && isPressed && base.Enabled)
        {
            color = Color.Gray;
            barColor = Color.FromArgb(31, 32, 35);
        }
        else if (!base.Enabled)
        {
            color = Color.Gray;
            barColor = Color.FromArgb(31, 32, 35);
        }
        else
        {
            color = Color.Gray;
            barColor = Color.FromArgb(31, 32, 35);
        }
        DrawScrollBar(e.Graphics, backColor, color, barColor);
        OnCustomPaintForeground(new MetroPaintEventArgs(backColor, color, e.Graphics));
    }

    private void DrawScrollBar(Graphics g, Color backColor, Color thumbColor, Color barColor)
    {
        if (useBarColor)
        {
            using (SolidBrush brush = new SolidBrush(barColor))
            {
                g.FillRectangle(brush, base.ClientRectangle);
            }
        }
        using (SolidBrush brush2 = new SolidBrush(backColor))
        {
            Rectangle rect = new Rectangle(thumbRectangle.X - 1, thumbRectangle.Y - 1, thumbRectangle.Width + 2, thumbRectangle.Height + 2);
            g.FillRectangle(brush2, rect);
        }
        using (SolidBrush brush3 = new SolidBrush(thumbColor))
        {
            g.FillRectangle(brush3, thumbRectangle);
        }
    }

    protected override void OnGotFocus(EventArgs e)
    {
        Invalidate();
        base.OnGotFocus(e);
    }

    protected override void OnLostFocus(EventArgs e)
    {
        isHovered = false;
        isPressed = false;
        Invalidate();
        base.OnLostFocus(e);
    }

    protected override void OnEnter(EventArgs e)
    {
        Invalidate();
        base.OnEnter(e);
    }

    protected override void OnLeave(EventArgs e)
    {
        isHovered = false;
        isPressed = false;
        Invalidate();
        base.OnLeave(e);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        int num = e.Delta / 120 * (maximum - minimum) / mouseWheelBarPartitions;
        if (Orientation == MetroScrollOrientation.Vertical)
        {
            Value -= num;
        }
        else
        {
            Value += num;
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            isPressed = true;
            Invalidate();
        }
        base.OnMouseDown(e);
        Focus();
        if (e.Button == MouseButtons.Left)
        {
            Point location = e.Location;
            if (thumbRectangle.Contains(location))
            {
                thumbClicked = true;
                thumbPosition = ((metroOrientation == MetroScrollOrientation.Vertical) ? (location.Y - thumbRectangle.Y) : (location.X - thumbRectangle.X));
                Invalidate(thumbRectangle);
            }
            else
            {
                trackPosition = ((metroOrientation == MetroScrollOrientation.Vertical) ? location.Y : location.X);
                if (trackPosition < ((metroOrientation == MetroScrollOrientation.Vertical) ? thumbRectangle.Y : thumbRectangle.X))
                {
                    topBarClicked = true;
                }
                else
                {
                    bottomBarClicked = true;
                }
                ProgressThumb(enableTimer: true);
            }
        }
        else if (e.Button == MouseButtons.Right)
        {
            trackPosition = ((metroOrientation == MetroScrollOrientation.Vertical) ? e.Y : e.X);
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        isPressed = false;
        base.OnMouseUp(e);
        if (e.Button == MouseButtons.Left)
        {
            if (thumbClicked)
            {
                thumbClicked = false;
                OnScroll(ScrollEventType.EndScroll, -1, curValue, scrollOrientation);
            }
            else if (topBarClicked)
            {
                topBarClicked = false;
                StopTimer();
            }
            else if (bottomBarClicked)
            {
                bottomBarClicked = false;
                StopTimer();
            }
            Invalidate();
        }
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        isHovered = true;
        Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        isHovered = false;
        Invalidate();
        base.OnMouseLeave(e);
        ResetScrollStatus();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (e.Button == MouseButtons.Left)
        {
            if (thumbClicked)
            {
                int num = curValue;
                int num2 = (metroOrientation == MetroScrollOrientation.Vertical) ? e.Location.Y : e.Location.X;
                int num3 = (metroOrientation == MetroScrollOrientation.Vertical) ? (num2 / base.Height / thumbHeight) : (num2 / base.Width / thumbWidth);
                if (num2 <= thumbTopLimit + thumbPosition)
                {
                    ChangeThumbPosition(thumbTopLimit);
                    curValue = minimum;
                    Invalidate();
                }
                else if (num2 >= thumbBottomLimitTop + thumbPosition)
                {
                    ChangeThumbPosition(thumbBottomLimitTop);
                    curValue = maximum;
                    Invalidate();
                }
                else
                {
                    ChangeThumbPosition(num2 - thumbPosition);
                    int num4;
                    int num5;
                    if (Orientation == MetroScrollOrientation.Vertical)
                    {
                        num4 = base.Height - num3;
                        num5 = thumbRectangle.Y;
                    }
                    else
                    {
                        num4 = base.Width - num3;
                        num5 = thumbRectangle.X;
                    }
                    float num6 = 0f;
                    if (num4 != 0)
                    {
                        num6 = (float)num5 / (float)num4;
                    }
                    curValue = Convert.ToInt32(num6 * (float)(maximum - minimum) + (float)minimum);
                }
                if (num != curValue)
                {
                    OnScroll(ScrollEventType.ThumbTrack, num, curValue, scrollOrientation);
                    Refresh();
                }
            }
        }
        else if (!base.ClientRectangle.Contains(e.Location))
        {
            ResetScrollStatus();
        }
        else if (e.Button == MouseButtons.None)
        {
            if (thumbRectangle.Contains(e.Location))
            {
                Invalidate(thumbRectangle);
            }
            else if (base.ClientRectangle.Contains(e.Location))
            {
                Invalidate();
            }
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        isHovered = true;
        isPressed = true;
        Invalidate();
        base.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        isHovered = false;
        isPressed = false;
        Invalidate();
        base.OnKeyUp(e);
    }

    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        base.SetBoundsCore(x, y, width, height, specified);
        if (base.DesignMode)
        {
            SetupScrollBar();
        }
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        SetupScrollBar();
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
        Keys keys = Keys.Up;
        Keys keys2 = Keys.Down;
        if (Orientation == MetroScrollOrientation.Horizontal)
        {
            keys = Keys.Left;
            keys2 = Keys.Right;
        }
        if (keyData == keys)
        {
            Value -= smallChange;
            return true;
        }
        if (keyData != keys2)
        {
            switch (keyData)
            {
                case Keys.Prior:
                    Value = GetValue(smallIncrement: false, up: true);
                    return true;
                case Keys.Next:
                    if (curValue + largeChange > maximum)
                    {
                        Value = maximum;
                    }
                    else
                    {
                        Value += largeChange;
                    }
                    return true;
                case Keys.Home:
                    Value = minimum;
                    return true;
                case Keys.End:
                    Value = maximum;
                    return true;
                default:
                    return base.ProcessDialogKey(keyData);
            }
        }
        Value += smallChange;
        return true;
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        Invalidate();
    }

    private void SetupScrollBar()
    {
        if (!inUpdate)
        {
            if (Orientation == MetroScrollOrientation.Vertical)
            {
                thumbWidth = ((base.Width > 0) ? base.Width : 10);
                thumbHeight = GetThumbSize();
                clickedBarRectangle = base.ClientRectangle;
                clickedBarRectangle.Inflate(-1, -1);
                thumbRectangle = new Rectangle(base.ClientRectangle.X, base.ClientRectangle.Y, thumbWidth, thumbHeight);
                thumbPosition = thumbRectangle.Height / 2;
                thumbBottomLimitBottom = base.ClientRectangle.Bottom;
                thumbBottomLimitTop = thumbBottomLimitBottom - thumbRectangle.Height;
                thumbTopLimit = base.ClientRectangle.Y;
            }
            else
            {
                thumbHeight = ((base.Height > 0) ? base.Height : 10);
                thumbWidth = GetThumbSize();
                clickedBarRectangle = base.ClientRectangle;
                clickedBarRectangle.Inflate(-1, -1);
                thumbRectangle = new Rectangle(base.ClientRectangle.X, base.ClientRectangle.Y, thumbWidth, thumbHeight);
                thumbPosition = thumbRectangle.Width / 2;
                thumbBottomLimitBottom = base.ClientRectangle.Right;
                thumbBottomLimitTop = thumbBottomLimitBottom - thumbRectangle.Width;
                thumbTopLimit = base.ClientRectangle.X;
            }
            ChangeThumbPosition(GetThumbPosition());
            Refresh();
        }
    }

    private void ResetScrollStatus()
    {
        bottomBarClicked = (topBarClicked = false);
        StopTimer();
        Refresh();
    }

    private void ProgressTimerTick(object sender, EventArgs e)
    {
        ProgressThumb(enableTimer: true);
    }

    private int GetValue(bool smallIncrement, bool up)
    {
        int num;
        if (up)
        {
            num = curValue - (smallIncrement ? smallChange : largeChange);
            if (num < minimum)
            {
                num = minimum;
            }
        }
        else
        {
            num = curValue + (smallIncrement ? smallChange : largeChange);
            if (num > maximum)
            {
                num = maximum;
            }
        }
        return num;
    }

    private int GetThumbPosition()
    {
        if (thumbHeight == 0 || thumbWidth == 0)
        {
            return 0;
        }
        int num = (metroOrientation == MetroScrollOrientation.Vertical) ? (thumbPosition / base.Height / thumbHeight) : (thumbPosition / base.Width / thumbWidth);
        int num2 = (Orientation != MetroScrollOrientation.Vertical) ? (base.Width - num) : (base.Height - num);
        int num3 = maximum - minimum;
        float num4 = 0f;
        if (num3 != 0)
        {
            num4 = ((float)curValue - (float)minimum) / (float)num3;
        }
        return Math.Max(thumbTopLimit, Math.Min(thumbBottomLimitTop, Convert.ToInt32(num4 * (float)num2)));
    }

    private int GetThumbSize()
    {
        int num = (metroOrientation == MetroScrollOrientation.Vertical) ? base.Height : base.Width;
        if (maximum == 0 || largeChange == 0)
        {
            return num;
        }
        float val = (float)largeChange * (float)num / (float)maximum;
        return Convert.ToInt32(Math.Min((float)num, Math.Max(val, 10f)));
    }

    private void EnableTimer()
    {
        if (!progressTimer.Enabled)
        {
            progressTimer.Interval = 600;
            progressTimer.Start();
        }
        else
        {
            progressTimer.Interval = 10;
        }
    }

    private void StopTimer()
    {
        progressTimer.Stop();
    }

    private void ChangeThumbPosition(int position)
    {
        if (Orientation == MetroScrollOrientation.Vertical)
        {
            thumbRectangle.Y = position;
        }
        else
        {
            thumbRectangle.X = position;
        }
    }

    private void ProgressThumb(bool enableTimer)
    {
        int num = curValue;
        ScrollEventType type = ScrollEventType.First;
        int num2;
        int num3;
        if (Orientation == MetroScrollOrientation.Vertical)
        {
            num2 = thumbRectangle.Y;
            num3 = thumbRectangle.Height;
        }
        else
        {
            num2 = thumbRectangle.X;
            num3 = thumbRectangle.Width;
        }
        if (bottomBarClicked && num2 + num3 < trackPosition)
        {
            type = ScrollEventType.LargeIncrement;
            curValue = GetValue(smallIncrement: false, up: false);
            if (curValue == maximum)
            {
                ChangeThumbPosition(thumbBottomLimitTop);
                type = ScrollEventType.Last;
            }
            else
            {
                ChangeThumbPosition(Math.Min(thumbBottomLimitTop, GetThumbPosition()));
            }
        }
        else if (topBarClicked && num2 > trackPosition)
        {
            type = ScrollEventType.LargeDecrement;
            curValue = GetValue(smallIncrement: false, up: true);
            if (curValue == minimum)
            {
                ChangeThumbPosition(thumbTopLimit);
                type = ScrollEventType.First;
            }
            else
            {
                ChangeThumbPosition(Math.Max(thumbTopLimit, GetThumbPosition()));
            }
        }
        if (num != curValue)
        {
            OnScroll(type, num, curValue, scrollOrientation);
            Invalidate();
            if (enableTimer)
            {
                EnableTimer();
            }
        }
    }
}