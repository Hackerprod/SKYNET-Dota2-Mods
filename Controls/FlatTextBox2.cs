using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

[DefaultEvent("TextChanged")]
class FlatTextBox2 : Control
{
    #region Variables

    public TextBox iTalkTB = new TextBox();
    private GraphicsPath Shape;
    private int _maxchars = 32767;
    private bool _ReadOnly;
    private bool _Multiline;
    private HorizontalAlignment ALNType;
    private bool isPasswordMasked = false;
    private Pen P1;
    private SolidBrush B1;

    #endregion
    #region Properties

    private Color _BackColorControl { get; set; }


    public Color BackColorControl
    {
        get
        {
            return _BackColorControl;
        }
        set
        {
            _BackColorControl = value;
        }
    }
    private bool _isCustomColor { get; set; }


    public bool isCustomColor
    {
        get
        {
            return _isCustomColor;
        }
        set
        {
            _isCustomColor = value;
        }
    }

    public HorizontalAlignment TextAlignment
    {
        get { return ALNType; }
        set
        {
            ALNType = value;
            Invalidate();
        }
    }
    public int MaxLength
    {
        get { return _maxchars; }
        set
        {
            _maxchars = value;
            iTalkTB.MaxLength = MaxLength;
            Invalidate();
        }
    }

    public bool UseSystemPasswordChar
    {
        get { return isPasswordMasked; }
        set
        {
            iTalkTB.UseSystemPasswordChar = UseSystemPasswordChar;
            isPasswordMasked = value;
            Invalidate();
        }
    }
    public bool ReadOnly
    {
        get { return _ReadOnly; }
        set
        {
            _ReadOnly = value;
            if (iTalkTB != null)
            {
                iTalkTB.ReadOnly = value;
            }
        }
    }
    public bool Multiline
    {
        get { return _Multiline; }
        set
        {
            _Multiline = value;
            if (iTalkTB != null)
            {
                iTalkTB.Multiline = value;

                if (value)
                {
                    iTalkTB.Height = Height - 10;
                }
                else
                {
                    Height = iTalkTB.Height + 10;
                }
            }
        }
    }


    #endregion
    #region EventArgs

    protected override void OnTextChanged(System.EventArgs e)
    {
        base.OnTextChanged(e);
        iTalkTB.Text = Text;
        Invalidate();
    }

    private void OnBaseTextChanged(object s, EventArgs e)
    {
        Text = iTalkTB.Text;
    }

    protected override void OnForeColorChanged(System.EventArgs e)
    {
        base.OnForeColorChanged(e);
        iTalkTB.ForeColor = ForeColor;
        Invalidate();
    }

    protected override void OnFontChanged(System.EventArgs e)
    {
        base.OnFontChanged(e);
        iTalkTB.Font = Font;
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        base.OnPaintBackground(e);
    }

    private void _OnKeyDown(object Obj, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.A)
        {
            iTalkTB.SelectAll();
            e.SuppressKeyPress = true;
        }
        if (e.Control && e.KeyCode == Keys.C)
        {
            iTalkTB.Copy();
            e.SuppressKeyPress = true;
        }
    }

    protected override void OnResize(System.EventArgs e)
    {
        base.OnResize(e);
        if (_Multiline)
        {
            iTalkTB.Height = Height - 10;
        }
        else
        {
            Height = iTalkTB.Height + 10;
        }

        Shape = new GraphicsPath();
        var _with1 = Shape;
        _with1.AddArc(0, 0, 10, 10, 180, 90);
        _with1.AddArc(Width - 11, 0, 10, 10, -90, 90);
        _with1.AddArc(Width - 11, Height - 11, 10, 10, 0, 90);
        _with1.AddArc(0, Height - 11, 10, 10, 90, 90);
        _with1.CloseAllFigures();
    }

    protected override void OnGotFocus(System.EventArgs e)
    {
        base.OnGotFocus(e);
        iTalkTB.Focus();
    }

    #endregion
    public void AddTextBox()
    {
        var _TB = iTalkTB;
        _TB.Size = new Size(Width - 10, 33);
        _TB.Location = new Point(7, 5);
        _TB.Text = string.Empty;
        _TB.BorderStyle = BorderStyle.None;
        _TB.TextAlign = HorizontalAlignment.Left;
        _TB.Font = this.Font;
        _TB.UseSystemPasswordChar = UseSystemPasswordChar;
        _TB.Multiline = false;
        iTalkTB.KeyDown += _OnKeyDown;
        iTalkTB.TextChanged += OnBaseTextChanged;

        iTalkTB.BackColor = Color.FromArgb(28, 29, 32); //Color del textBox
    }

    public FlatTextBox2()
    {
        SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        SetStyle(ControlStyles.UserPaint, true);


        P1 = new Pen(_BackColorControl); // P1 = Border color
        B1 = new SolidBrush(_BackColorControl); // B1 = Rect Background color

        AddTextBox();
        Controls.Add(iTalkTB);

        Text = null;
        Font = new Font("Tahoma", 11);
        Size = new Size(135, 30);
        DoubleBuffered = true;
    }
    protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
    {
        base.OnPaint(e);
        Bitmap B = new Bitmap(Width, Height);
        Graphics G = Graphics.FromImage(B);

        G.SmoothingMode = SmoothingMode.AntiAlias;

        var _TB = iTalkTB;
        _TB.Width = Width - 12;
        _TB.TextAlign = TextAlignment;
        _TB.UseSystemPasswordChar = UseSystemPasswordChar;

        G.FillPath(B1, Shape); // Draw background
        G.DrawPath(P1, Shape); // Draw border

        e.Graphics.DrawImage((Image)B.Clone(), 0, 0);
        G.Dispose();
        B.Dispose();
    }

}
