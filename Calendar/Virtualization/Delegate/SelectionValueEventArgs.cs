﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Virtualization.Delegate
{
    public class SelectionValueEventArgs : RoutedEventArgs {

        public SelectionValueEventArgs(RoutedEvent id, object value, TypeValue type, SelectInfo row, SelectInfo column, bool isGroup, IEnumerable<object> itemInGroup) {
            RoutedEvent = id ?? throw new ArgumentNullException("id");

            this._selectedValue = value;
            this._selectedRow = row;
            this._selectedColumn = column;
            this._type = type;

            this._isGroupSelection = isGroup;
            this._itemsInGroup = itemInGroup;
        }

        #region Property
        private readonly object _selectedValue;
        public object SelectedValue {
            get { return _selectedValue; }
        }

        private readonly SelectInfo _selectedRow;
        public SelectInfo SelectedRow {
            get { return _selectedRow; }
        }

        private readonly SelectInfo _selectedColumn;
        public SelectInfo SelectedColumn {
            get { return _selectedColumn; }
        }

        private readonly TypeValue _type;
        public TypeValue Type {
            get { return _type; }
        }

        private readonly bool _isGroupSelection;
        public bool IsGroupSelection {
            get { return _isGroupSelection; }
        }

        private readonly IEnumerable<object> _itemsInGroup;
        public IEnumerable<object> ItemsInGroup {
            get { return _itemsInGroup; }
        }
        #endregion

        protected override void InvokeEventHandler(System.Delegate genericHandler, object genericTarget)
        {
            SelectionValueEventHandler handler = (SelectionValueEventHandler)genericHandler;
            handler(genericTarget, this);
        }
    }

    public enum TypeValue {
        Item, Row, Column
    }

    public enum TypeSelection {
        Left, Double, Right, Middle
    }

    public struct SelectInfo {

        public SelectInfo(object content, int step)
        {
            this.Content = content;
            this.Step = step;
        }

        public object Content { get; private set; }

        public int Step { get; private set; }
    }
}
