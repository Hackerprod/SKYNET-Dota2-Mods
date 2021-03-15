using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

public class FlatContextMenuStrip : ContextMenuStrip
{
    public class TColorTable : ProfessionalColorTable
    {
        private Color _CheckedColor { get; set; }

        private Color BorderColor;

        [Category("Colors")]
        public Color CheckedColor
        {
            get
            {
                return _CheckedColor;
            }
            set
            {
                _CheckedColor = value;
            }
        }

        [Category("Colors")]
        public Color _BorderColor
        {
            get
            {
                return BorderColor;
            }
            set
            {
                BorderColor = value;
            }
        }

        public override Color ButtonSelectedBorder => ColorSystem.BackDark;

        public override Color CheckBackground => _CheckedColor;

        public override Color CheckPressedBackground => _CheckedColor;

        public override Color CheckSelectedBackground => _CheckedColor;

        public override Color ImageMarginGradientBegin => _CheckedColor;

        public override Color ImageMarginGradientEnd => _CheckedColor;

        public override Color ImageMarginGradientMiddle => _CheckedColor;

        public override Color MenuBorder => BorderColor;

        public override Color MenuItemBorder => BorderColor;

        public override Color MenuItemSelected => _CheckedColor;

        public override Color SeparatorDark => BorderColor;

        public override Color ToolStripDropDownBackground => ColorSystem.BackDark;

        public TColorTable()
        {
            CheckedColor = Color.FromArgb(57, 62, 63);
            BorderColor = Color.FromArgb(53, 58, 60);
        }
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        Invalidate();
    }

    public FlatContextMenuStrip()
    {
        base.Renderer = new ToolStripProfessionalRenderer(new TColorTable());
        base.ShowImageMargin = false;
        base.ForeColor = Color.White;
        Font = new Font("Segoe UI", 8f);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
    }
}
