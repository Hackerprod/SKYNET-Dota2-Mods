using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Drawing;
using System.Windows.Forms;

[StandardModule]
internal sealed class ColorSystem
{
    internal static Color SuperDark = Color.FromArgb(29, 35, 38);

    internal static Color BackDark = Color.FromArgb(41, 41, 41);

    internal static Color BackLight = Color.FromArgb(45, 47, 49);

    internal static Color AllBackColor { get; set; } = Color.FromArgb(31, 32, 35);

    internal static Color FlatColor = Color.FromArgb(35, 168, 109);

    internal static Color FlatColorList = Color.FromArgb(35, 168, 109);

    internal static Color FontColorButtons = Color.FromArgb(255, 255, 255);

    internal static Color FontColorLabels = Color.FromArgb(255, 255, 255);

    public static void RefreshColors(Form TargetForm, string ColorSchemeItems)
    {
        try
        {
            string[] array = ColorSchemeItems.Split(';');
            SuperDark = ToRgb(array[1]);
            BackDark = ToRgb(array[2]);
            BackLight = ToRgb(array[3]);
            AllBackColor = ToRgb(array[4]);
            FlatColor = ToRgb(array[5]);
            FlatColorList = ToRgb(array[6]);
            FontColorButtons = ToRgb(array[7]);
            FontColorLabels = ToRgb(array[8]);
   //         MyProject.Forms.Form1.ModListView.BackgroundColor = FlatColorList;
   //         MyProject.Forms.Form1.ModListView.RowsDefaultCellStyle.BackColor = FlatColorList;
   //         MyProject.Forms.Form1.ModListView.AlternatingRowsDefaultCellStyle.BackColor = FlatColorList;
   //         MyProject.Forms.Form1.ModListView.RowsDefaultCellStyle.SelectionBackColor = FlatColor;
   //         MyProject.Forms.Form1.ModListView.AlternatingRowsDefaultCellStyle.SelectionBackColor = FlatColor;
            TargetForm.BackColor = BackDark;
            TargetForm.Refresh();
            GC.Collect();
        }
        catch (Exception ex)
        {
            ProjectData.SetProjectError(ex);
            Exception ex2 = ex;
            ProjectData.ClearProjectError();
        }
    }

    public static Color ToRgb(string HexColor)
    {
        HexColor = Strings.Replace(HexColor, "#", "");
        string value = Conversions.ToString(Conversion.Val("&H" + Strings.Mid(HexColor, 1, 2)));
        string value2 = Conversions.ToString(Conversion.Val("&H" + Strings.Mid(HexColor, 3, 2)));
        string value3 = Conversions.ToString(Conversion.Val("&H" + Strings.Mid(HexColor, 5, 2)));
        return Color.FromArgb(Conversions.ToInteger(value), Conversions.ToInteger(value2), Conversions.ToInteger(value3));
    }
}