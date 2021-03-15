using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
public class FlatButton : Control
{
    [Flags]
    public enum _Style
    {
        TextOnly = 0x0,
        ImageIconOnly = 0x1,
        ImageIconWithText = 0x2
    }
    private Color _BackColorMouseOver { get; set; }


    public Color BackColorMouseOver
    {
        get
        {
            return _BackColorMouseOver;
        }
        set
        {
            _BackColorMouseOver = value;
        }
    }
    private Color _ForeColorMouseOver { get; set; }

    public Color ForeColorMouseOver
    {
        get
        {
            return _ForeColorMouseOver;
        }
        set
        {
            _ForeColorMouseOver = value;
        }
    }
    [Flags]
    public enum _ImgAlign
    {
        Left = 0x0,
        Center = 0x1,
        Right = 0x2
    }

    private int W;

    private int H;

    private int _ImageX;

    private int _ImageY;

    private bool _Rounded;

    private _Style _ButtonStyle;

    private Image _ImageIcon;

    public _ImgAlign _ImageAlign;

    private MouseState State;

    [Category("Button Options")]
    public bool Rounded
    {
        get
        {
            return _Rounded;
        }
        set
        {
            _Rounded = value;
        }
    }

    [Category("Button Options")]
    public _Style Style
    {
        get
        {
            return _ButtonStyle;
        }
        set
        {
            _ButtonStyle = value;
        }
    }

    [Category("Button Options")]
    public Image ImageIcon
    {
        get
        {
            return _ImageIcon;
        }
        set
        {
            _ImageIcon = value;
        }
    }

    [Category("Button Options")]
    public _ImgAlign ImageAlignment
    {
        get
        {
            return _ImageAlign;
        }
        set
        {
            _ImageAlign = value;
        }
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

    public FlatButton()
    {
        _Rounded = false;
        _ButtonStyle = _Style.TextOnly;
        _ImageAlign = _ImgAlign.Left;
        State = MouseState.None;
        SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
        DoubleBuffered = true;
        base.Size = new Size(100, 32);
        BackColor = Color.Transparent;
        Font = new Font("Segoe UI", 9f);
        Cursor = Cursors.Hand;
        _ButtonStyle = _Style.TextOnly;
        _ImageAlign = _ImgAlign.Left;
        _ImageX = 0;
        _ImageY = 0;
        _ImageIcon = null;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Helpers.B = new Bitmap(base.Width, base.Height);
        Helpers.G = Graphics.FromImage(Helpers.B);
        checked
        {
            W = base.Width - 1;
            H = base.Height - 1;
            if (_ButtonStyle == _Style.ImageIconOnly)
            {
                if (_ImageAlign == _ImgAlign.Left)
                {
                    _ImageX = 3;
                    _ImageY = (int)Math.Round(unchecked(Math.Round((double)base.Height / 2.0) - Math.Round((double)_ImageIcon.Height / 2.0)));
                }
                else if (_ImageAlign == _ImgAlign.Center)
                {
                    _ImageX = (int)Math.Round(unchecked(Math.Round((double)base.Width / 2.0) - Math.Round((double)_ImageIcon.Width / 2.0)));
                    _ImageY = (int)Math.Round(unchecked(Math.Round((double)base.Height / 2.0) - Math.Round((double)_ImageIcon.Height / 2.0)));
                }
                else if (_ImageAlign == _ImgAlign.Right)
                {
                    _ImageX = base.Width - _ImageIcon.Width - 3;
                    _ImageY = (int)Math.Round(unchecked(Math.Round((double)base.Height / 2.0) - Math.Round((double)_ImageIcon.Height / 2.0)));
                }
            }
            else if (_ButtonStyle == _Style.ImageIconWithText)
            {
                if (_ImageAlign == _ImgAlign.Left)
                {
                    _ImageX = 10;
                    _ImageY = (int)Math.Round(unchecked(Math.Round((double)base.Height / 2.0) - Math.Round((double)_ImageIcon.Height / 2.0)));
                }
                else if (_ImageAlign == _ImgAlign.Center)
                {
                    _ImageX = (int)Math.Round(unchecked(Math.Round((double)base.Width / 2.0) - Math.Round((double)_ImageIcon.Width / 2.0)));
                    _ImageY = (int)Math.Round(unchecked(Math.Round((double)base.Height / 2.0) - Math.Round((double)_ImageIcon.Height / 2.0)));
                }
                else if (_ImageAlign == _ImgAlign.Right)
                {
                    _ImageX = base.Width - _ImageIcon.Width - 3;
                    _ImageY = (int)Math.Round(unchecked(Math.Round((double)base.Height / 2.0) - Math.Round((double)_ImageIcon.Height / 2.0)));
                }
            }
            else if (_ButtonStyle == _Style.TextOnly)
            {
                _ImageX = 0;
                _ImageY = 0;
            }
            GraphicsPath graphicsPath = new GraphicsPath();
            Rectangle rectangle = new Rectangle(0, 0, W, H);
            new Point((int)Math.Round(Math.Round(unchecked((double)W / 2.0))), (int)Math.Round(Math.Round(unchecked((double)H / 2.0))));
            Graphics g = Helpers.G;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.Clear(BackColor);
            switch (State)
            {
                case MouseState.None:
                    if (Rounded)
                    {
                        graphicsPath = Helpers.RoundRec(rectangle, 6);
                        g.FillPath(new SolidBrush(BackColor), graphicsPath);
                    }
                    else
                    {
                        g.FillRectangle(new SolidBrush(BackColor), rectangle);
                    }
                    break;
                case MouseState.Over:
                    if (Rounded)
                    {
                        graphicsPath = Helpers.RoundRec(rectangle, 6);
                        g.FillPath(new SolidBrush(BackColor), graphicsPath);
                        g.FillPath(new SolidBrush(Color.FromArgb(20, Color.White)), graphicsPath);
                    }
                    else
                    {
                        g.FillRectangle(new SolidBrush(BackColor), rectangle);
                        g.FillRectangle(new SolidBrush(Color.FromArgb(20, Color.White)), rectangle);
                    }
                    break;
                case MouseState.Down:
                    if (Rounded)
                    {
                        graphicsPath = Helpers.RoundRec(rectangle, 6);
                        g.FillPath(new SolidBrush(BackColor), graphicsPath);
                        g.FillPath(new SolidBrush(Color.FromArgb(20, Color.Black)), graphicsPath);
                    }
                    else
                    {
                        g.FillRectangle(new SolidBrush(BackColor), rectangle);
                        g.FillRectangle(new SolidBrush(Color.FromArgb(20, Color.Black)), rectangle);
                    }
                    break;
            }
            if (_ButtonStyle == _Style.TextOnly)
            {
                g.DrawString(Text, Font, new SolidBrush(ForeColor), rectangle, Helpers.CenterSF);
            }
            else if (_ButtonStyle == _Style.ImageIconOnly)
            {
                g.DrawImage(_ImageIcon, _ImageX, _ImageY, _ImageIcon.Width, _ImageIcon.Height);
            }
            else if (_ButtonStyle == _Style.ImageIconWithText)
            {
                g.DrawImage(_ImageIcon, _ImageX, _ImageY, _ImageIcon.Width, _ImageIcon.Height);
                g.DrawString(Text, Font, new SolidBrush(ForeColor), rectangle, Helpers.CenterSF);
            }
            g = null;
            base.OnPaint(e);
            Helpers.G.Dispose();
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.DrawImageUnscaled(Helpers.B, 0, 0);
            Helpers.B.Dispose();
        }
    }
}