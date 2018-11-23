using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Virtualization.Models
{
    public abstract class GridItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected GridItem(int row, int column, int rowSpan, int columnSpan, Thickness margin, object content)
        {
            _row = row;
            _column = column;

            _rowSpan = rowSpan;
            _columnSpan = columnSpan;

            _margin = margin;

            _content = content;
        }

        #region Fields
        private int _row;
        public int Row {
            get => _row;
            set { _row = value; this.SendPropertyChnaged(nameof(Row)); }
        }

        private int _column;
        public int Column {
            get => _column;
            set { _column = value; this.SendPropertyChnaged(nameof(Column)); }
        }

        private int _rowSpan;
        public int RowSpan {
            get => _rowSpan;
            set { _rowSpan = value; this.SendPropertyChnaged(nameof(RowSpan)); }
        }

        private int _columnSpan;
        public int ColumnSpan {
            get => _columnSpan;
            set { _columnSpan = value; this.SendPropertyChnaged(nameof(ColumnSpan)); }
        }

        private Thickness _margin;
        public Thickness Margin {
            get => _margin;
            set { _margin = value; this.SendPropertyChnaged(nameof(Margin)); }
        }

        private object _content;
        public object Content {
            get => _content;
            set { _content = value; this.SendPropertyChnaged(nameof(Content)); }
        }

        private bool _isSelected;
        public bool IsSelected {
            get { return _isSelected; }
            set { _isSelected = value; this.SendPropertyChnaged(nameof(IsSelected)); }
        }
        #endregion

        #region Helps
        protected void SendPropertyChnaged(string propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Set(object content, int row, int column, int rowSpan, int columnSpan, Thickness margin, bool isSelected)
        {
            this.Row = row;
            this.Column = column;
            this.RowSpan = rowSpan;
            this.ColumnSpan = columnSpan;

            this.Margin = margin;

            this.Content = content;

            this.IsSelected = isSelected;
        }

        public virtual GridItem Copy() {
            return null;
        }
        #endregion
    }
}
