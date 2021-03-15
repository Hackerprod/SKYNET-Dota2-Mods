
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
public class FlatTabControl : TabControl
{
    protected override void CreateHandle()
    {
        base.CreateHandle();
    }

    public FlatTabControl()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
        DoubleBuffered = true;
        Font = new Font("Segoe UI", 10f);
        base.SizeMode = TabSizeMode.Fixed;
        base.ItemSize = new Size(120, 40);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Helpers.B = new Bitmap(base.Width, base.Height);
        Helpers.G = Graphics.FromImage(Helpers.B);
        Graphics g = Helpers.G;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        g.Clear(Color.FromArgb(31, 32, 35));
        try
        {
            base.SelectedTab.BackColor = Color.FromArgb(31, 32, 35);
        }
        catch (Exception projectError)
        {
            ProjectData.SetProjectError(projectError);
            ProjectData.ClearProjectError();
        }
        checked
        {
            if (base.Alignment == TabAlignment.Top)
            {
                g.FillRectangle(new SolidBrush(ColorSystem.BackLight), new Rectangle(2, 0, base.Width - 4, base.ItemSize.Height + 4));
            }
            else if (base.Alignment == TabAlignment.Left)
            {
                g.FillRectangle(new SolidBrush(ColorSystem.BackLight), new Rectangle(0, 0, base.ItemSize.Height + 4, base.Height));
            }
            int num = base.TabCount - 1;
            for (int i = 0; i <= num; i++)
            {
                Rectangle rectangle = new Rectangle(new Point(GetTabRect(i).Location.X + 4, GetTabRect(i).Location.Y), new Size(GetTabRect(i).Width - 4, GetTabRect(i).Height));
                Rectangle rectangle2 = new Rectangle(rectangle.Location, new Size(rectangle.Width, rectangle.Height));
                if (i == base.SelectedIndex)
                {
                    g.FillRectangle(new SolidBrush(Color.Transparent), rectangle2);
                    g.FillRectangle(new SolidBrush(ColorSystem.FlatColor), rectangle2);
                    if (base.ImageList != null)
                    {
                        try
                        {
                            if (base.ImageList.Images[base.TabPages[i].ImageIndex] != null)
                            {
                                g.DrawImage(base.ImageList.Images[base.TabPages[i].ImageIndex], new Point(rectangle2.Location.X + 8, rectangle2.Location.Y + 6));
                                g.DrawString("      " + base.TabPages[i].Text, Font, new SolidBrush(ColorSystem.FontColorButtons), rectangle2, Helpers.CenterSF);
                            }
                            else
                            {
                                g.DrawString(base.TabPages[i].Text, Font, new SolidBrush(ColorSystem.FontColorButtons), rectangle2, Helpers.CenterSF);
                            }
                        }
                        catch (Exception ex)
                        {
                            ProjectData.SetProjectError(ex);
                            Exception ex2 = ex;
                            throw new Exception(ex2.Message);
                        }
                    }
                    else
                    {
                        g.DrawString(base.TabPages[i].Text, Font, new SolidBrush(ColorSystem.FontColorButtons), rectangle2, Helpers.CenterSF);
                    }
                }
                else
                {
                    g.FillRectangle(new SolidBrush(Color.Transparent), rectangle2);
                    if (base.ImageList != null)
                    {
                        try
                        {
                            if (base.ImageList.Images[base.TabPages[i].ImageIndex] != null)
                            {
                                g.DrawImage(base.ImageList.Images[base.TabPages[i].ImageIndex], new Point(rectangle2.Location.X + 8, rectangle2.Location.Y + 6));
                                g.DrawString("      " + base.TabPages[i].Text, Font, new SolidBrush(ColorSystem.FontColorButtons), rectangle2, new StringFormat
                                {
                                    LineAlignment = StringAlignment.Center,
                                    Alignment = StringAlignment.Center
                                });
                            }
                            else
                            {
                                g.DrawString(base.TabPages[i].Text, Font, new SolidBrush(ColorSystem.FontColorButtons), rectangle2, new StringFormat
                                {
                                    LineAlignment = StringAlignment.Center,
                                    Alignment = StringAlignment.Center
                                });
                            }
                        }
                        catch (Exception ex3)
                        {
                            ProjectData.SetProjectError(ex3);
                            Exception ex4 = ex3;
                            throw new Exception(ex4.Message);
                        }
                    }
                    else
                    {
                        g.DrawString(base.TabPages[i].Text, Font, new SolidBrush(ColorSystem.FontColorButtons), rectangle2, new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        });
                    }
                }
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
