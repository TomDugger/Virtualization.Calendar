using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Virtualization.Models
{
    public class GridRowHeader : GridItem
    {
        public GridRowHeader(int row, int column, int rowSpan, object content, bool isGroup = false)
            : base(row, column, rowSpan, 1, new Thickness(), content) { this.IsGroup = isGroup; }

        public bool IsGroup { get; private set; }

        public override GridItem Copy()
        {
            var result = new GridRowHeader(this.Row, this.Column, this.RowSpan, this.ColumnSpan, this.IsGroup);
            result.Content = this.Content;
            return result;
        }
    }
}
