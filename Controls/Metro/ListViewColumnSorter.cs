using System;
using System.Collections;
using System.Windows.Forms;

public class ListViewColumnSorter : IComparer
{
    public enum SortModifiers
    {
        SortByImage,
        SortByCheckbox,
        SortByText
    }

    public int ColumnToSort;

    public SortOrder OrderOfSort;

    private CaseInsensitiveComparer ObjectCompare;

    private SortModifiers mySortModifier = SortModifiers.SortByText;

    public SortModifiers _SortModifier
    {
        get
        {
            return mySortModifier;
        }
        set
        {
            mySortModifier = value;
        }
    }

    public int SortColumn
    {
        get
        {
            return ColumnToSort;
        }
        set
        {
            ColumnToSort = value;
        }
    }

    public SortOrder Order
    {
        get
        {
            return OrderOfSort;
        }
        set
        {
            OrderOfSort = value;
        }
    }

    public ListViewColumnSorter()
    {
        ColumnToSort = 0;
        ObjectCompare = new CaseInsensitiveComparer();
    }

    public int Compare(object x, object y)
    {
        int num = 0;
        ListViewItem listViewItem = (ListViewItem)x;
        ListViewItem listViewItem2 = (ListViewItem)y;
        num = ((!DateTime.TryParse(listViewItem.SubItems[ColumnToSort].Text, out DateTime result) || !DateTime.TryParse(listViewItem2.SubItems[ColumnToSort].Text, out DateTime result2)) ? ObjectCompare.Compare(listViewItem.SubItems[ColumnToSort].Text, listViewItem2.SubItems[ColumnToSort].Text) : ObjectCompare.Compare(result, result2));
        if (OrderOfSort == SortOrder.Ascending)
        {
            return num;
        }
        if (OrderOfSort == SortOrder.Descending)
        {
            return -num;
        }
        return 0;
    }
}