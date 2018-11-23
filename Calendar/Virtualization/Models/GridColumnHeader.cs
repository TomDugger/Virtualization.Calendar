using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Virtualization.Models
{
    public class GridColumnHeader : GridItem
    {
        public GridColumnHeader(int row, int column, int colSpan, object content, bool isGroup = false)
            : base(row, column, 1, colSpan, new Thickness(), content) { this.IsGroup = isGroup; }

        public bool IsGroup { get; private set; }

        public override GridItem Copy()
        {
            var result = new GridColumnHeader(this.Row, this.Column, this.RowSpan, this.ColumnSpan, this.IsGroup);
            result.Content = this.Content;
            return result;
        }
    }
}
