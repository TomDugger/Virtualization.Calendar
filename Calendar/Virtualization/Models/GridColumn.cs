using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virtualization.Models
{
    public class GridColumn:GridItem
    {
        public GridColumn(int column, int columnSpan) 
            : base(0, column, 1, columnSpan, new System.Windows.Thickness(), null) { this.IsOdd = column % 2 == 1; }

        public bool IsOdd { get; private set; }
    }
}
