using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Virtualization.Models
{
    public class GridDataItem:GridItem
    {
        public GridDataItem(int row, int  column, int rowSpan, int columnSpan, Thickness margin, object content) 
            : base(row, column, rowSpan, columnSpan, margin, content) { }
    }
}
