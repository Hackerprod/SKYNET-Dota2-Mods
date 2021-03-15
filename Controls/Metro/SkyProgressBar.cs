using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

[Designer("MetroFramework.Design.Controls.MetroProgressBarDesigner, MetroFramework.Design, Version=1.4.0.0, Culture=neutral, PublicKeyToken=5f91a84759bf584a")]
[ToolboxBitmap(typeof(ProgressBar))]
public class MetroProgressBar : ProgressBar, IMetroControl
{
    private bool useCustomBackColor;

    private bool useCustomForeColor;

    private bool useStyleColors = true;

    private MetroProgressBarSize metroLabelSize = MetroProgressBarSize.Medium;

    private MetroProgressBarWeight metroLabelWeight;

    private ContentAlignment textAlign = ContentAlignment.MiddleRight;

    private bool hideProgressText = true;

    private ProgressBarStyle progressBarStyle = ProgressBarStyle.Continuous;

    private int marqueeX;

    private Timer marqueeTimer;


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

    [Category("Metro Appearance")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    [DefaultValue(true)]
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

    [Browsable(false)]
    [Category("Metro Behaviour")]
    [DefaultValue(false)]
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
    [DefaultValue(MetroProgressBarSize.Medium)]
    public MetroProgressBarSize FontSize
    {
        get
        {
            return metroLabelSize;
        }
        set
        {
            metroLabelSize = value;
        }
    }

    [Category("Metro Appearance")]
    [DefaultValue(MetroProgressBarWeight.Light)]
    public MetroProgressBarWeight FontWeight
    {
        get
        {
            return metroLabelWeight;
        }
        set
        {
            metroLabelWeight = value;
        }
    }

    [Category("Metro Appearance")]
    [DefaultValue(ContentAlignment.MiddleRight)]
    public ContentAlignment TextAlign
    {
        get
        {
            return textAlign;
        }
        set
        {
            textAlign = value;
        }
    }

    [Category("Metro Appearance")]
    [DefaultValue(true)]
    public bool HideProgressText
    {
        get
        {
            return hideProgressText;
        }
        set
        {
            hideProgressText = value;
        }
    }

    [Category("Metro Appearance")]
    [DefaultValue(ProgressBarStyle.Continuous)]
    public ProgressBarStyle ProgressBarStyle
    {
        get
        {
            return progressBarStyle;
        }
        set
        {
            progressBarStyle = value;
        }
    }

    public new int Value
    {
        get
        {
            return base.Value;
        }
        set
        {
            if (value <= base.Maximum)
            {
                base.Value = value;
                Invalidate();
            }
        }
    }

    [Browsable(false)]
    public double ProgressTotalPercent
    {
        get
        {
            return (1.0 - (double)(base.Maximum - Value) / (double)base.Maximum) * 100.0;
        }
    }

    [Browsable(false)]
    public double ProgressTotalValue
    {
        get
        {
            return 1.0 - (double)(base.Maximum - Value) / (double)base.Maximum;
        }
    }

    [Browsable(false)]
    public string ProgressPercentText
    {
        get
        {
            return $"{Math.Round(ProgressTotalPercent)}%";
        }
    }

    private double ProgressBarWidth => (double)Value / (double)base.Maximum * (double)base.ClientRectangle.Width;

    private int ProgressBarMarqueeWidth => base.ClientRectangle.Width / 3;

    private bool marqueeTimerEnabled
    {
        get
        {
            if (marqueeTimer != null)
            {
                return marqueeTimer.Enabled;
            }
            return false;
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

    public MetroProgressBar()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, value: true);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        try
        {
            Color color = BackColor;
            if (!useCustomBackColor)
            {
                color = Color.Red;
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
        if (progressBarStyle == ProgressBarStyle.Continuous)
        {
            if (!base.DesignMode)
            {
                StopTimer();
            }
            DrawProgressContinuous(e.Graphics);
        }
        else if (progressBarStyle == ProgressBarStyle.Blocks)
        {
            if (!base.DesignMode)
            {
                StopTimer();
            }
            DrawProgressContinuous(e.Graphics);
        }
        else if (progressBarStyle == ProgressBarStyle.Marquee)
        {
            if (!base.DesignMode && base.Enabled)
            {
                StartTimer();
            }
            if (!base.Enabled)
            {
                StopTimer();
            }
            if (Value == base.Maximum)
            {
                StopTimer();
                DrawProgressContinuous(e.Graphics);
            }
            else
            {
                DrawProgressMarquee(e.Graphics);
            }
        }
        DrawProgressText(e.Graphics);
        using (Pen pen = new Pen(Color.Blue))
        {
            Rectangle rect = new Rectangle(0, 0, base.Width - 1, base.Height - 1);
            e.Graphics.DrawRectangle(pen, rect);
        }
        OnCustomPaintForeground(new MetroPaintEventArgs(Color.Empty, Color.Empty, e.Graphics));
    }

    private void DrawProgressContinuous(Graphics graphics)
    {
        graphics.FillRectangle(new SolidBrush(Color.FromArgb(47, 48, 151)), 0, 0, (int)ProgressBarWidth, base.ClientRectangle.Height);
    }

    private void DrawProgressMarquee(Graphics graphics)
    {
        graphics.FillRectangle(new SolidBrush(Color.FromArgb(147, 48, 51)), marqueeX, 0, ProgressBarMarqueeWidth, base.ClientRectangle.Height);
    }

    private void DrawProgressText(Graphics graphics)
    {
        if (!HideProgressText)
        {
            TextRenderer.DrawText(foreColor: Color.Yellow, dc: graphics, text: ProgressPercentText, font: Font, bounds: base.ClientRectangle, flags: TextFormatFlags.Default);
        }
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        base.GetPreferredSize(proposedSize);
        using (Graphics dc = CreateGraphics())
        {
            proposedSize = new Size(2147483647, 2147483647);
            return TextRenderer.MeasureText(dc, ProgressPercentText, Font, proposedSize);
        }
    }

    private void StartTimer()
    {
        if (!marqueeTimerEnabled)
        {
            if (marqueeTimer == null)
            {
                marqueeTimer = new Timer
                {
                    Interval = 10
                };
                marqueeTimer.Tick += marqueeTimer_Tick;
            }
            marqueeX = -ProgressBarMarqueeWidth;
            marqueeTimer.Stop();
            marqueeTimer.Start();
            marqueeTimer.Enabled = true;
            Invalidate();
        }
    }

    private void StopTimer()
    {
        if (marqueeTimer != null)
        {
            marqueeTimer.Stop();
            Invalidate();
        }
    }

    private void marqueeTimer_Tick(object sender, EventArgs e)
    {
        marqueeX++;
        if (marqueeX > base.ClientRectangle.Width)
        {
            marqueeX = -ProgressBarMarqueeWidth;
        }
        Invalidate();
    }
}
public enum MetroProgressBarSize
{
    Small,
    Medium,
    Tall
}
public enum MetroProgressBarWeight
{
    Light,
    Regular,
    Bold
}