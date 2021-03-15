using System;
using System.Drawing;

public class MetroPaintEventArgs : EventArgs
{
    public Color BackColor
    {
        get;
        private set;
    }

    public Color ForeColor
    {
        get;
        private set;
    }

    public Graphics Graphics
    {
        get;
        private set;
    }

    public MetroPaintEventArgs(Color backColor, Color foreColor, Graphics g)
    {
        BackColor = backColor;
        ForeColor = foreColor;
        Graphics = g;
    }
}