using System;

public interface IMetroControl
{
    bool UseCustomBackColor
    {
        get;
        set;
    }

    bool UseCustomForeColor
    {
        get;
        set;
    }

    bool UseStyleColors
    {
        get;
        set;
    }

    bool UseSelectable
    {
        get;
        set;
    }

    event EventHandler<MetroPaintEventArgs> CustomPaintBackground;

    event EventHandler<MetroPaintEventArgs> CustomPaint;

    event EventHandler<MetroPaintEventArgs> CustomPaintForeground;
}