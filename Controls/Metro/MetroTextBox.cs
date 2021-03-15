
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

[Designer("MetroFramework.Design.Controls.MetroTextBoxDesigner, MetroFramework.Design, Version=1.4.0.0, Culture=neutral, PublicKeyToken=5f91a84759bf584a")]
public class MetroTextBox : Control, IMetroControl
{
    public delegate void ButClick(object sender, EventArgs e);

    private class PromptedTextBox : TextBox
    {
        private const int OCM_COMMAND = 8465;

        private const int WM_PAINT = 15;

        private bool drawPrompt;

        private string promptText = "";

        private Color _waterMarkColor = Color.Red;

        private Font _waterMarkFont = new System.Drawing.Font("Segoe UI Emoji", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

        [DefaultValue("")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string WaterMark
        {
            get
            {
                return promptText;
            }
            set
            {
                promptText = value.Trim();
                Invalidate();
            }
        }

        public Color WaterMarkColor
        {
            get
            {
                return _waterMarkColor;
            }
            set
            {
                _waterMarkColor = value;
                Invalidate();
            }
        }

        public Font WaterMarkFont
        {
            get
            {
                return _waterMarkFont;
            }
            set
            {
                _waterMarkFont = value;
            }
        }

        public PromptedTextBox()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, value: true);
            drawPrompt = (Text.Trim().Length == 0);
        }

        private void DrawTextPrompt()
        {
            using (Graphics g = CreateGraphics())
            {
                DrawTextPrompt(g);
            }
        }

        private void DrawTextPrompt(Graphics g)
        {
            TextFormatFlags textFormatFlags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding;
            Rectangle clientRectangle = base.ClientRectangle;
            switch (base.TextAlign)
            {
                case HorizontalAlignment.Left:
                    clientRectangle.Offset(1, 0);
                    break;
                case HorizontalAlignment.Right:
                    textFormatFlags |= TextFormatFlags.Right;
                    clientRectangle.Offset(-2, 0);
                    break;
                case HorizontalAlignment.Center:
                    textFormatFlags |= TextFormatFlags.HorizontalCenter;
                    clientRectangle.Offset(1, 0);
                    break;
            }
            new SolidBrush(WaterMarkColor);
            TextRenderer.DrawText(g, promptText, _waterMarkFont, clientRectangle, _waterMarkColor, BackColor, textFormatFlags);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (drawPrompt)
            {
                DrawTextPrompt(e.Graphics);
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
        }

        protected override void OnTextAlignChanged(EventArgs e)
        {
            base.OnTextAlignChanged(e);
            Invalidate();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            drawPrompt = (Text.Trim().Length == 0);
            Invalidate();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if ((m.Msg == 15 || m.Msg == 8465) && drawPrompt && !GetStyle(ControlStyles.UserPaint))
            {
                DrawTextPrompt();
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
        }
    }

    public delegate void LUClear();

    [ToolboxItem(false)]
    public class MetroTextButton : Button, IMetroControl
    {
        private bool useCustomBackColor;

        private bool useCustomForeColor;

        private bool useStyleColors;

        private bool isHovered;

        private bool isPressed;

        private Bitmap _image;


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

        [DefaultValue(false)]
        [Category("Metro Appearance")]
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
        [DefaultValue(false)]
        [Category("Metro Behaviour")]
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

        public new Image Image
        {
            get
            {
                return base.Image;
            }
            set
            {
                base.Image = value;
                if (value != null)
                {
                    _image = ApplyInvert(new Bitmap(value));
                }
            }
        }

        protected Size iconSize
        {
            get
            {
                if (Image != null)
                {
                    Size size = Image.Size;
                    double num = 14.0 / (double)size.Height;
                    new Point(1, 1);
                    return new Size((int)((double)size.Width * num), (int)((double)size.Height * num));
                }
                return new Size(-1, -1);
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

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Color foreColor;
            Color color;
            if (base.Parent != null)
            {
                if (base.Parent is IMetroControl)
                {
                    foreColor = Color.Red;
                    color = Color.Yellow;
                }
                else
                {
                    foreColor = Color.Red;
                    color = Color.Yellow;
                }
            }
            else
            {
                foreColor = Color.Red;
                color = Color.Yellow;
            }
            if (isHovered && !isPressed && base.Enabled)
            {
                byte r = color.R;
                byte g = color.G;
                byte b = color.B;
                color = ControlPaint.Light(color, 0.25f);
            }
            else if (isHovered && isPressed && base.Enabled)
            {
                foreColor = Color.Red;
                color = Color.Yellow;
            }
            else if (!base.Enabled)
            {
                foreColor = Color.Red;
                color = Color.Yellow;
            }
            else
            {
                foreColor = Color.Red;
            }
            e.Graphics.Clear(color);
            Font font = this.Font;
            TextRenderer.DrawText(e.Graphics, Text, font, base.ClientRectangle, foreColor, color, TextFormatFlags.EndEllipsis | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            DrawIcon(e.Graphics);
        }

        public Bitmap ApplyInvert(Bitmap bitmapImage)
        {
            for (int i = 0; i < bitmapImage.Height; i++)
            {
                for (int j = 0; j < bitmapImage.Width; j++)
                {
                    Color pixel = bitmapImage.GetPixel(j, i);
                    byte a = pixel.A;
                    byte red = (byte)(255 - pixel.R);
                    byte green = (byte)(255 - pixel.G);
                    byte blue = (byte)(255 - pixel.B);
                    bitmapImage.SetPixel(j, i, Color.FromArgb(a, red, green, blue));
                }
            }
            return bitmapImage;
        }

        private void DrawIcon(Graphics g)
        {
            if (Image != null)
            {
                Point location = new Point(2, (base.ClientRectangle.Height - iconSize.Height) / 2);
                int num = 5;
                switch (base.ImageAlign)
                {
                    case ContentAlignment.BottomCenter:
                        location = new Point((base.ClientRectangle.Width - iconSize.Width) / 2, base.ClientRectangle.Height - iconSize.Height - num);
                        break;
                    case ContentAlignment.BottomLeft:
                        location = new Point(num, base.ClientRectangle.Height - iconSize.Height - num);
                        break;
                    case ContentAlignment.BottomRight:
                        location = new Point(base.ClientRectangle.Width - iconSize.Width - num, base.ClientRectangle.Height - iconSize.Height - num);
                        break;
                    case ContentAlignment.MiddleCenter:
                        location = new Point((base.ClientRectangle.Width - iconSize.Width) / 2, (base.ClientRectangle.Height - iconSize.Height) / 2);
                        break;
                    case ContentAlignment.MiddleLeft:
                        location = new Point(num, (base.ClientRectangle.Height - iconSize.Height) / 2);
                        break;
                    case ContentAlignment.MiddleRight:
                        location = new Point(base.ClientRectangle.Width - iconSize.Width - num, (base.ClientRectangle.Height - iconSize.Height) / 2);
                        break;
                    case ContentAlignment.TopCenter:
                        location = new Point((base.ClientRectangle.Width - iconSize.Width) / 2, num);
                        break;
                    case ContentAlignment.TopLeft:
                        location = new Point(num, num);
                        break;
                    case ContentAlignment.TopRight:
                        location = new Point(base.ClientRectangle.Width - iconSize.Width - num, num);
                        break;
                }
                g.DrawImage(_image, new Rectangle(location, iconSize));
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isPressed = true;
                Invalidate();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isPressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isHovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }
    }


    private bool useCustomBackColor;

    private bool useCustomForeColor;

    private bool useStyleColors;

    private PromptedTextBox baseTextBox;

    private Image textBoxIcon;

    private bool textBoxIconRight;

    private bool displayIcon;

    private MetroTextButton _button;

    private bool _showbutton;


    private bool _showclear;

    private bool _witherror;

    private bool _cleared;

    private bool _withtext;

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

    [DefaultValue(false)]
    [Category("Metro Appearance")]
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
    [Browsable(false)]
    [Category("Metro Behaviour")]
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


    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    [Obsolete("Use watermark")]
    [DefaultValue("")]
    [Category("Metro Appearance")]
    public string PromptText
    {
        get
        {
            return baseTextBox.WaterMark;
        }
        set
        {
            baseTextBox.WaterMark = value;
        }
    }

    [Category("Metro Appearance")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DefaultValue("")]
    public string WaterMark
    {
        get
        {
            return baseTextBox.WaterMark;
        }
        set
        {
            baseTextBox.WaterMark = value;
        }
    }

    [DefaultValue(null)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category("Metro Appearance")]
    [Browsable(true)]
    public Image Icon
    {
        get
        {
            return textBoxIcon;
        }
        set
        {
            textBoxIcon = value;
            Refresh();
        }
    }

    [EditorBrowsable(EditorBrowsableState.Always)]
    [DefaultValue(false)]
    [Category("Metro Appearance")]
    [Browsable(true)]
    public bool IconRight
    {
        get
        {
            return textBoxIconRight;
        }
        set
        {
            textBoxIconRight = value;
            Refresh();
        }
    }

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DefaultValue(false)]
    [Category("Metro Appearance")]
    public bool DisplayIcon
    {
        get
        {
            return displayIcon;
        }
        set
        {
            displayIcon = value;
            Refresh();
        }
    }

    protected Size iconSize
    {
        get
        {
            if (displayIcon && textBoxIcon != null)
            {
                int num = (textBoxIcon.Height > base.ClientRectangle.Height) ? base.ClientRectangle.Height : textBoxIcon.Height;
                Size size = textBoxIcon.Size;
                double num2 = (double)num / (double)size.Height;
                new Point(1, 1);
                return new Size((int)((double)size.Width * num2), (int)((double)size.Height * num2));
            }
            return new Size(-1, -1);
        }
    }

    protected int ButtonWidth
    {
        get
        {
            int result = 0;
            if (_button != null)
            {
                result = (_showbutton ? _button.Width : 0);
            }
            return result;
        }
    }

    [Browsable(true)]
    [Category("Metro Appearance")]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DefaultValue(false)]
    public bool ShowButton
    {
        get
        {
            return _showbutton;
        }
        set
        {
            _showbutton = value;
            Refresh();
        }
    }

    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    [DefaultValue(false)]
    [Category("Metro Appearance")]
    public bool ShowClearButton
    {
        get
        {
            return _showclear;
        }
        set
        {
            _showclear = value;
            Refresh();
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [DefaultValue(false)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [Category("Metro Appearance")]
    public MetroTextButton CustomButton
    {
        get
        {
            return _button;
        }
        set
        {
            _button = value;
            Refresh();
        }
    }

    [DefaultValue(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool WithError
    {
        get
        {
            return _witherror;
        }
        set
        {
            _witherror = value;
            Invalidate();
        }
    }

    public override ContextMenu ContextMenu
    {
        get
        {
            return baseTextBox.ContextMenu;
        }
        set
        {
            ContextMenu = value;
            baseTextBox.ContextMenu = value;
        }
    }

    public override ContextMenuStrip ContextMenuStrip
    {
        get
        {
            return baseTextBox.ContextMenuStrip;
        }
        set
        {
            ContextMenuStrip = value;
            baseTextBox.ContextMenuStrip = value;
        }
    }

    [DefaultValue(false)]
    public bool Multiline
    {
        get
        {
            return baseTextBox.Multiline;
        }
        set
        {
            baseTextBox.Multiline = value;
        }
    }

    public override string Text
    {
        get
        {
            return baseTextBox.Text;
        }
        set
        {
            baseTextBox.Text = value;
        }
    }

    [Category("Metro Appearance")]
    public Color WaterMarkColor
    {
        get
        {
            return baseTextBox.WaterMarkColor;
        }
        set
        {
            baseTextBox.WaterMarkColor = value;
        }
    }

    [Category("Metro Appearance")]
    public Font WaterMarkFont
    {
        get
        {
            return baseTextBox.WaterMarkFont;
        }
        set
        {
            baseTextBox.WaterMarkFont = value;
        }
    }

    public string[] Lines
    {
        get
        {
            return baseTextBox.Lines;
        }
        set
        {
            baseTextBox.Lines = value;
        }
    }

    [Browsable(false)]
    public string SelectedText
    {
        get
        {
            return baseTextBox.SelectedText;
        }
        set
        {
            baseTextBox.Text = value;
        }
    }

    [DefaultValue(false)]
    public bool ReadOnly
    {
        get
        {
            return baseTextBox.ReadOnly;
        }
        set
        {
            baseTextBox.ReadOnly = value;
        }
    }

    public char PasswordChar
    {
        get
        {
            return baseTextBox.PasswordChar;
        }
        set
        {
            baseTextBox.PasswordChar = value;
        }
    }

    [DefaultValue(false)]
    public bool UseSystemPasswordChar
    {
        get
        {
            return baseTextBox.UseSystemPasswordChar;
        }
        set
        {
            baseTextBox.UseSystemPasswordChar = value;
        }
    }

    [DefaultValue(HorizontalAlignment.Left)]
    public HorizontalAlignment TextAlign
    {
        get
        {
            return baseTextBox.TextAlign;
        }
        set
        {
            baseTextBox.TextAlign = value;
        }
    }

    public int SelectionStart
    {
        get
        {
            return baseTextBox.SelectionStart;
        }
        set
        {
            baseTextBox.SelectionStart = value;
        }
    }

    public int SelectionLength
    {
        get
        {
            return baseTextBox.SelectionLength;
        }
        set
        {
            baseTextBox.SelectionLength = value;
        }
    }

    [DefaultValue(true)]
    public new bool TabStop
    {
        get
        {
            return baseTextBox.TabStop;
        }
        set
        {
            baseTextBox.TabStop = value;
        }
    }

    public int MaxLength
    {
        get
        {
            return baseTextBox.MaxLength;
        }
        set
        {
            baseTextBox.MaxLength = value;
        }
    }

    public ScrollBars ScrollBars
    {
        get
        {
            return baseTextBox.ScrollBars;
        }
        set
        {
            baseTextBox.ScrollBars = value;
        }
    }

    [DefaultValue(AutoCompleteMode.None)]
    public AutoCompleteMode AutoCompleteMode
    {
        get
        {
            return baseTextBox.AutoCompleteMode;
        }
        set
        {
            baseTextBox.AutoCompleteMode = value;
        }
    }

    [DefaultValue(AutoCompleteSource.None)]
    public AutoCompleteSource AutoCompleteSource
    {
        get
        {
            return baseTextBox.AutoCompleteSource;
        }
        set
        {
            baseTextBox.AutoCompleteSource = value;
        }
    }

    public AutoCompleteStringCollection AutoCompleteCustomSource
    {
        get
        {
            return baseTextBox.AutoCompleteCustomSource;
        }
        set
        {
            baseTextBox.AutoCompleteCustomSource = value;
        }
    }

    public bool ShortcutsEnabled
    {
        get
        {
            return baseTextBox.ShortcutsEnabled;
        }
        set
        {
            baseTextBox.ShortcutsEnabled = value;
        }
    }

    [DefaultValue(CharacterCasing.Normal)]
    public CharacterCasing CharacterCasing
    {
        get
        {
            return baseTextBox.CharacterCasing;
        }
        set
        {
            baseTextBox.CharacterCasing = value;
        }
    }

    [Category("Metro Appearance")]
    public event EventHandler<MetroPaintEventArgs> CustomPaintBackground;

    [Category("Metro Appearance")]
    public event EventHandler<MetroPaintEventArgs> CustomPaint;

    [Category("Metro Appearance")]
    public event EventHandler<MetroPaintEventArgs> CustomPaintForeground;

    public event EventHandler AcceptsTabChanged;

    public event ButClick ButtonClick;

    public event LUClear ClearClicked;

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

    public MetroTextBox()
    {
        SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, value: true);
        base.GotFocus += MetroTextBox_GotFocus;
        base.TabStop = false;
        CreateBaseTextBox();
        UpdateBaseTextBox();
        AddEventHandler();
    }

    private void BaseTextBoxAcceptsTabChanged(object sender, EventArgs e)
    {
        if (this.AcceptsTabChanged != null)
        {
            this.AcceptsTabChanged(this, e);
        }
    }

    private void BaseTextBoxSizeChanged(object sender, EventArgs e)
    {
        base.OnSizeChanged(e);
    }

    private void BaseTextBoxCursorChanged(object sender, EventArgs e)
    {
        base.OnCursorChanged(e);
    }

    private void BaseTextBoxContextMenuStripChanged(object sender, EventArgs e)
    {
        base.OnContextMenuStripChanged(e);
    }

    private void BaseTextBoxContextMenuChanged(object sender, EventArgs e)
    {
        base.OnContextMenuChanged(e);
    }

    private void BaseTextBoxClientSizeChanged(object sender, EventArgs e)
    {
        base.OnClientSizeChanged(e);
    }

    private void BaseTextBoxClick(object sender, EventArgs e)
    {
        base.OnClick(e);
    }

    private void BaseTextBoxChangeUiCues(object sender, UICuesEventArgs e)
    {
        base.OnChangeUICues(e);
    }

    private void BaseTextBoxCausesValidationChanged(object sender, EventArgs e)
    {
        base.OnCausesValidationChanged(e);
    }

    private void BaseTextBoxKeyUp(object sender, KeyEventArgs e)
    {
        base.OnKeyUp(e);
    }

    private void BaseTextBoxKeyPress(object sender, KeyPressEventArgs e)
    {
        base.OnKeyPress(e);
    }

    private void BaseTextBoxKeyDown(object sender, KeyEventArgs e)
    {
        base.OnKeyDown(e);
    }

    private void BaseTextBoxTextChanged(object sender, EventArgs e)
    {
        base.OnTextChanged(e);
        if (baseTextBox.Text != "" && !_withtext)
        {
            _withtext = true;
            _cleared = false;
            Invalidate();
        }
        if (baseTextBox.Text == "" && !_cleared)
        {
            _withtext = false;
            _cleared = true;
            Invalidate();
        }
    }

    public void Select(int start, int length)
    {
        baseTextBox.Select(start, length);
    }

    public void SelectAll()
    {
        baseTextBox.SelectAll();
    }

    public void Clear()
    {
        baseTextBox.Clear();
    }

    private void MetroTextBox_GotFocus(object sender, EventArgs e)
    {
        baseTextBox.Focus();
    }

    public void AppendText(string text)
    {
        baseTextBox.AppendText(text);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        try
        {
            Color color = BackColor;
            baseTextBox.BackColor = color;
            if (!useCustomBackColor)
            {
                color = Color.Red;
                baseTextBox.BackColor = color;
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
        if (useCustomForeColor)
        {
            baseTextBox.ForeColor = ForeColor;
        }
        else
        {
            baseTextBox.ForeColor = Color.Blue;
        }
        Color color = Color.Green;
        if (useStyleColors)
        {
            color = Color.Magenta; 
        }
        if (_witherror)
        {
            color = Color.Red;
        }
        using (Pen pen = new Pen(color))
        {
            e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, base.Width - 2, base.Height - 1));
        }
        DrawIcon(e.Graphics);
    }

    private void DrawIcon(Graphics g)
    {
        if (displayIcon && textBoxIcon != null)
        {
            Point location = new Point(5, 5);
            if (textBoxIconRight)
            {
                location = new Point(base.ClientRectangle.Width - iconSize.Width - 1, 1);
            }
            g.DrawImage(textBoxIcon, new Rectangle(location, iconSize));
            UpdateBaseTextBox();
        }
        else
        {
            _button.Visible = _showbutton;
            if (_showbutton && _button != null)
            {
                UpdateBaseTextBox();
            }
        }
        OnCustomPaintForeground(new MetroPaintEventArgs(Color.Empty, baseTextBox.ForeColor, g));
    }

    public override void Refresh()
    {
        base.Refresh();
        UpdateBaseTextBox();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        UpdateBaseTextBox();
    }

    private void CreateBaseTextBox()
    {
        if (baseTextBox == null)
        {
            baseTextBox = new PromptedTextBox();
            baseTextBox.BorderStyle = BorderStyle.None;
            baseTextBox.Font = Font;
            baseTextBox.Location = new Point(3, 3);
            baseTextBox.Size = new Size(base.Width - 6, base.Height - 6);
            base.Size = new Size(baseTextBox.Width + 6, baseTextBox.Height + 6);
            baseTextBox.TabStop = true;
            base.Controls.Add(baseTextBox);
            if (_button == null)
            {
                _button = new MetroTextButton();
                _button.Location = new Point(3, 1);
                _button.Size = new Size(base.Height - 4, base.Height - 4);
                _button.TextChanged += _button_TextChanged;
                _button.MouseEnter += _button_MouseEnter;
                _button.MouseLeave += _button_MouseLeave;
                _button.Click += _button_Click;
                if (!base.Controls.Contains(_button))
                {
                    base.Controls.Add(_button);
                }
            }
        }
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
    }

    private void _button_Click(object sender, EventArgs e)
    {
        if (this.ButtonClick != null)
        {
            this.ButtonClick(this, e);
        }
    }

    private void _button_MouseLeave(object sender, EventArgs e)
    {
        UseStyleColors = baseTextBox.Focused;
        Invalidate();
    }

    private void _button_MouseEnter(object sender, EventArgs e)
    {
        UseStyleColors = true;
        Invalidate();
    }

    private void _button_TextChanged(object sender, EventArgs e)
    {
        _button.Invalidate();
    }

    private void AddEventHandler()
    {
        baseTextBox.AcceptsTabChanged += BaseTextBoxAcceptsTabChanged;
        baseTextBox.CausesValidationChanged += BaseTextBoxCausesValidationChanged;
        baseTextBox.ChangeUICues += BaseTextBoxChangeUiCues;
        baseTextBox.Click += BaseTextBoxClick;
        baseTextBox.ClientSizeChanged += BaseTextBoxClientSizeChanged;
        baseTextBox.ContextMenuChanged += BaseTextBoxContextMenuChanged;
        baseTextBox.ContextMenuStripChanged += BaseTextBoxContextMenuStripChanged;
        baseTextBox.CursorChanged += BaseTextBoxCursorChanged;
        baseTextBox.KeyDown += BaseTextBoxKeyDown;
        baseTextBox.KeyPress += BaseTextBoxKeyPress;
        baseTextBox.KeyUp += BaseTextBoxKeyUp;
        baseTextBox.SizeChanged += BaseTextBoxSizeChanged;
        baseTextBox.TextChanged += BaseTextBoxTextChanged;
        baseTextBox.GotFocus += baseTextBox_GotFocus;
        baseTextBox.LostFocus += baseTextBox_LostFocus;
    }

    private void baseTextBox_LostFocus(object sender, EventArgs e)
    {
        UseStyleColors = false;
        Invalidate();
        InvokeLostFocus(this, e);
    }

    private void baseTextBox_GotFocus(object sender, EventArgs e)
    {
        _witherror = false;
        UseStyleColors = true;
        Invalidate();
        InvokeGotFocus(this, e);
    }

    private void UpdateBaseTextBox()
    {
        if (_button != null)
        {
            if (base.Height % 2 > 0)
            {
                _button.Size = new Size(base.Height - 2, base.Height - 2);
                _button.Location = new Point(base.Width - (_button.Width + 1), 1);
            }
            else
            {
                _button.Size = new Size(base.Height - 5, base.Height - 5);
                _button.Location = new Point(base.Width - _button.Width - 3, 2);
            }
            _button.Visible = _showbutton;
        }
        int num = 0;
        if (baseTextBox != null)
        {
            baseTextBox.Font = this.Font;
            if (displayIcon)
            {
                Point location = new Point(iconSize.Width + 10, 5);
                if (textBoxIconRight)
                {
                    location = new Point(3, 3);
                }
                baseTextBox.Location = location;
                baseTextBox.Size = new Size(base.Width - (20 + ButtonWidth + num) - iconSize.Width, base.Height - 6);
            }
            else
            {
                baseTextBox.Location = new Point(3, 3);
                baseTextBox.Size = new Size(base.Width - (6 + ButtonWidth + num), base.Height - 6);
            }
        }
    }

    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(MetroTextBox));
        SuspendLayout();
        ResumeLayout(performLayout: false);
    }

    private void lnkClear_Click(object sender, EventArgs e)
    {
        Focus();
        Clear();
        baseTextBox.Focus();
        if (this.ClearClicked != null)
        {
            this.ClearClicked();
        }
    }
}
