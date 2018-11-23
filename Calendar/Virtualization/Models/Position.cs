using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Virtualization.Models
{
    public struct Position
    {
        public Position(int index = 0, int row = 0, int col = 0, int rowSpan = 1, int colSpan = 1)
        {
            this.Index = index;

            this.Row = row;
            this.Column = col;
            this.RowSpan = rowSpan;
            this.ColumnSpan = colSpan;
        }

        public int Index { get; set; }

        public int Row { get; set; }
        public int Column { get; set; }

        public int RowSpan { get; set; }

        public int ColumnSpan { get; set; }
        
        public static bool operator ==(Position obj1, Position obj2)
        {
            if ((obj1.Index == obj2.Index) 
                && (obj1.Row == obj2.Row) && (obj1.Column == obj2.Column) 
                && (obj1.RowSpan == obj2.RowSpan) && (obj1.ColumnSpan == obj2.ColumnSpan))
                return true;
            return false;
        }

        public static bool operator !=(Position obj1, Position obj2)
        {
            return (obj1.Index != obj2.Index)
                || (obj1.Row != obj2.Row) || (obj1.Column != obj2.Column)
                || (obj1.RowSpan != obj2.RowSpan) || (obj1.ColumnSpan != obj2.ColumnSpan);
        }

        public static Position[,] Create(int rowCount, int columnCount) {
            var result = new Position[columnCount, rowCount];
            for (int i = 0; i < columnCount; i++)
                for (int j = 0; j < rowCount; j++)
                    result[i, j] = def;
            return result;
        }

        private static Position def = new Position(-1, 0, 0, 1, 1);
        public static Position Default() {
            return def;
        }
    }
}
