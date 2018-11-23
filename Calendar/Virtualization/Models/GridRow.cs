using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virtualization.Models
{
    public class GridRow: GridItem
    {
        public GridRow(int row, int rowSpan) 
            : base(row, 0, rowSpan, 1, new System.Windows.Thickness(), null) { this.IsOdd = row % 2 == 1; }

        public bool IsOdd { get; private set; }
    }
}
