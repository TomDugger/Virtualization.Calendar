using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Virtualization.Commands;
using Virtualization.Delegate;
using Virtualization.Helpers;
using Virtualization.Models;

namespace Virtualization.Controls
{
    public abstract class GridControl : ScrollableControl, INotifyPropertyChanged
    {
        #region Events
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(SelectionValueEventHandler), typeof(GridControl));
        public static readonly RoutedEvent SelectionDoubleClickChangedEvent = EventManager.RegisterRoutedEvent("SelectionDoubleClick", RoutingStrategy.Bubble, typeof(SelectionValueEventHandler), typeof(GridControl));
        public static readonly RoutedEvent SelectionRighctClickChangedEvent = EventManager.RegisterRoutedEvent("SelectionRighctClick", RoutingStrategy.Bubble, typeof(SelectionValueEventHandler), typeof(GridControl));
        public static readonly RoutedEvent SelectionMiddleClickChangedEvent = EventManager.RegisterRoutedEvent("SelectionMiddleClick", RoutingStrategy.Bubble, typeof(SelectionValueEventHandler), typeof(GridControl));

        public event SelectionValueEventHandler SelectionChanged {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        public event SelectionValueEventHandler SelectionDoubleClickChanged {
            add { AddHandler(SelectionDoubleClickChangedEvent, value); }
            remove { RemoveHandler(SelectionDoubleClickChangedEvent, value); }
        }

        public event SelectionValueEventHandler SelectionRightClickChanged {
            add { AddHandler(SelectionRighctClickChangedEvent, value); }
            remove { RemoveHandler(SelectionRighctClickChangedEvent, value); }
        }

        public event SelectionValueEventHandler SelectionMiddleClickChanged {
            add { AddHandler(SelectionMiddleClickChangedEvent, value); }
            remove { RemoveHandler(SelectionMiddleClickChangedEvent, value); }
        }

        protected void RaiseSelectionChanged(GridItem value, TypeValue type, TypeSelection selection = TypeSelection.Left, bool MetricGroup = false) {

            SelectInfo row = new SelectInfo();
            SelectInfo column = new SelectInfo();
            bool isGroup = false;
            IEnumerable<object> items = null;

            if (MetricGroup) {

                var rrow = this.GridRowHeaders.FirstOrDefault(x => x.Row <= value.Row && value.Row < x.Row + x.RowSpan);
                var rcolumn = this.GridColumnHeaders.FirstOrDefault(x => x.Column <= value.Column && value.Column < x.Column + x.ColumnSpan);

                if (rrow != null)
                    row = new SelectInfo(rrow.Content, (rrow.Row - value.Row) * -1);

                if (rcolumn != null)
                    column = new SelectInfo(rcolumn.Content, (rcolumn.Column - value.Column) * -1);
            }

            switch (type)
            {
                case TypeValue.Column:
                    isGroup = true;
                    var tempList = new List<object>();
                    
                    int count = this.itemSourceCache.GetLength(1);

                    for (int i = 0; i < count; i++) {
                        for (int j = this.displayedColScrollOffset + value.Column - this.ColumnHeaderCount; j < this.displayedColScrollOffset + value.Column + value.ColumnSpan - this.ColumnHeaderCount; j++)
                        {
                            var temp = itemSourceCache[j, i];
                            if (itemSourceCache[j, i] != null) 
                                if (!tempList.Contains(itemSourceCache[j, i]))
                                    tempList.Add(itemSourceCache[j, i]);
                        }
                    }
                    items = tempList.ToArray();
                    SelectedDataInGroup = items;
                    SelectedDataInGroup = stateChange?.Invoke(items);
                    break;
                case TypeValue.Row:
                    isGroup = true;
                    var tList = new List<object>();

                    int c = this.itemSourceCache.GetLength(0);

                    for (int i = 0; i < c; i++) {
                        for (int j = this.displayedRowScrollOffset + value.Row - this.RowHeaderCount; j < this.displayedRowScrollOffset + value.Row + value.RowSpan - this.RowHeaderCount; j++) {
                            var temp = itemSourceCache[i, j];

                            if(itemSourceCache[i, j] != null)
                                if (!tList.Contains(itemSourceCache[i, j]))
                                    tList.Add(itemSourceCache[i, j]);
                        }
                    }
                    items = tList.ToArray();
                    SelectedDataInGroup = items;
                    SelectedDataInGroup = stateChange?.Invoke(items);
                    break;
                case TypeValue.Item:
                    SelectedDataInGroup = null;
                    break;
            }

            SelectionValueEventArgs args = null;
            switch (selection)
            {
                case TypeSelection.Left:
                    args = new SelectionValueEventArgs(GridControl.SelectionChangedEvent, value.Content, type, row, column, isGroup, items);
                    break;
                case TypeSelection.Double:
                    args = new SelectionValueEventArgs(GridControl.SelectionDoubleClickChangedEvent, value.Content, type, row, column, isGroup, items);
                    break;
                case TypeSelection.Right:
                    args = new SelectionValueEventArgs(GridControl.SelectionRighctClickChangedEvent, value.Content, type, row, column, isGroup, items);
                    break;
                case TypeSelection.Middle:
                    args = new SelectionValueEventArgs(GridControl.SelectionMiddleClickChangedEvent, value.Content, type, row, column, isGroup, items);
                    break;
            }
            RaiseEvent(args);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void SendPropertyChanged(string propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Commands
        private Command _mouseSelectCommand;
        public Command MouseSelectCommand {
            get {
                return _mouseSelectCommand ?? (_mouseSelectCommand = new Command(obj =>
                {
                    if (SelectedItem != null)
                        ((GridItem)SelectedItem).IsSelected = false;

                    if (SelectedRow != null)
                        ((GridItem)SelectedRow).IsSelected = false;

                    if (SelectedColumn != null)
                        ((GridItem)SelectedColumn).IsSelected = false;

                    if (obj is GridDataItem item)
                    {
                        var index = this.DataItems.ToList().IndexOf(item);
                        if (index != SelectedIndex) {
                            SelectedIndex = this.DataItems.ToList().IndexOf(item);
                            item.IsSelected = true;
                            SelectedItem = item;
                            SelectedData = item.Content;
                        }
                        else {
                            SelectedIndex = -1;
                            (SelectedItem as GridDataItem).IsSelected = false;
                            SelectedItem = null;
                            SelectedData = null;
                        }

                        SelectedRowIndex = -1;
                        SelectedColumnIndex = -1;

                        var tmpr = this.GridRowHeaders.FirstOrDefault(x => SelectedRow != null && x.Content == (SelectedRow as GridRowHeader).Content);
                        if (tmpr != null)
                        {
                            tmpr.IsSelected = false;
                            SelectedRow = null;
                        }
                        var tmpc = this.GridColumnHeaders.FirstOrDefault(x => SelectedColumn != null && x.Content == (SelectedColumn as GridColumnHeader).Content);
                        if (tmpc != null)
                        {
                            tmpc.IsSelected = false;
                            SelectedColumn = null;
                        }

                        RaiseSelectionChanged(item, TypeValue.Item, MetricGroup: item.Content == null);
                    }
                    else if (obj is GridRowHeader row)
                    {
                        var index = this.GridRowHeaders.ToList().IndexOf(row);
                        if (index != SelectedRowIndex)
                        {
                            SelectedRowIndex = this.GridRowHeaders.ToList().IndexOf(row);
                            row.IsSelected = true;
                            var tmp = this.GridRowHeaders.FirstOrDefault(x => SelectedRow != null && x.Content == (SelectedRow as GridRowHeader).Content);
                            if (tmp != null)
                            {
                                tmp.IsSelected = false;
                                SelectedRow = null;
                            }
                            SelectedRow = row.Copy();
                        }
                        else {
                            SelectedRowIndex = -1;
                            var tmp = this.GridRowHeaders.FirstOrDefault(x => SelectedRow != null && x.Content == (SelectedRow as GridRowHeader).Content);
                            if (tmp != null)
                            {
                                tmp.IsSelected = false;
                                SelectedRow = null;
                            }
                        }

                        SelectedIndex = -1;
                        SelectedColumnIndex = -1;

                        SelectedItem = null;
                        var tmpc = this.GridColumnHeaders.FirstOrDefault(x => SelectedColumn != null && x.Content == (SelectedColumn as GridColumnHeader).Content);
                        if (tmpc != null)
                        {
                            tmpc.IsSelected = false;
                            SelectedColumn = null;
                        }
                        SelectedData = null;

                        RaiseSelectionChanged(row, TypeValue.Row);
                    }
                    else if (obj is GridColumnHeader column) {

                        var index = this.GridColumnHeaders.ToList().IndexOf(column);
                        if (index != SelectedColumnIndex) {
                            SelectedColumnIndex = this.GridColumnHeaders.ToList().IndexOf(column);
                            column.IsSelected = true;
                            var tmp = this.GridColumnHeaders.FirstOrDefault(x => SelectedColumn != null && x.Content == (SelectedColumn as GridColumnHeader).Content);
                            if (tmp != null)
                            {
                                tmp.IsSelected = false;
                                SelectedColumn = null;
                            }
                            SelectedColumn = column.Copy();
                        } else {
                            SelectedColumnIndex = -1;

                            var tmp = this.GridColumnHeaders.FirstOrDefault(x => SelectedColumn != null && x.Content == (SelectedColumn as GridColumnHeader).Content);
                            if (tmp != null) {
                                tmp.IsSelected = false;
                                SelectedColumn = null;
                            }
                        }

                        SelectedIndex = -1;
                        SelectedRowIndex = -1;

                        SelectedItem = null;
                        var tmpr = this.GridRowHeaders.FirstOrDefault(x => SelectedRow != null && x.Content == (SelectedRow as GridRowHeader).Content);
                        if (tmpr != null)
                        {
                            tmpr.IsSelected = false;
                            SelectedRow = null;
                        }
                        SelectedData = null;

                        RaiseSelectionChanged(column, TypeValue.Column);
                    }
                }, obj => obj != null));
            }
        }

        private Command _mouseDoubleClickCommand;
        public Command MouseDoubleClickCommand {
            get {
                return _mouseDoubleClickCommand ?? (_mouseDoubleClickCommand = new Command(obj =>
                {
                    if (obj is GridDataItem item)
                    {
                        RaiseSelectionChanged(item, TypeValue.Item, TypeSelection.Double, item.Content == null);
                    }
                    else if (obj is GridRowHeader row)
                    {
                        RaiseSelectionChanged(row, TypeValue.Row, TypeSelection.Double);
                    }
                    else if (obj is GridColumnHeader column)
                    {
                        RaiseSelectionChanged(column, TypeValue.Column, TypeSelection.Double);
                    }
                }, obj => obj != null));
            }
        }

        private Command _mouseRightClickCommand;
        public Command MouseRightClickCommand {
            get {
                return _mouseRightClickCommand ?? (_mouseRightClickCommand = new Command(obj =>
                {
                    if (SelectedItem != null)
                        ((GridItem)SelectedItem).IsSelected = false;

                    if (SelectedRow != null)
                        ((GridItem)SelectedRow).IsSelected = false;

                    if (SelectedColumn != null)
                        ((GridItem)SelectedColumn).IsSelected = false;

                    if (obj is GridDataItem item)
                    {
                        var index = this.DataItems.ToList().IndexOf(item);
                        if (index != SelectedIndex)
                        {
                            SelectedIndex = this.DataItems.ToList().IndexOf(item);
                            item.IsSelected = true;
                            SelectedItem = item;
                            SelectedData = item.Content;
                        }
                        else
                        {
                            SelectedIndex = -1;
                            (SelectedItem as GridDataItem).IsSelected = false;
                            SelectedItem = null;
                            SelectedData = null;
                        }

                        SelectedRowIndex = -1;
                        SelectedColumnIndex = -1;

                        var tmpr = this.GridRowHeaders.FirstOrDefault(x => SelectedRow != null && x.Content == (SelectedRow as GridRowHeader).Content);
                        if (tmpr != null)
                        {
                            tmpr.IsSelected = false;
                            SelectedRow = null;
                        }
                        var tmpc = this.GridColumnHeaders.FirstOrDefault(x => SelectedColumn != null && x.Content == (SelectedColumn as GridColumnHeader).Content);
                        if (tmpc != null)
                        {
                            tmpc.IsSelected = false;
                            SelectedColumn = null;
                        }

                        RaiseSelectionChanged(item, TypeValue.Item, TypeSelection.Right, item.Content == null);
                    }
                    else if (obj is GridRowHeader row)
                    {
                        var index = this.GridRowHeaders.ToList().IndexOf(row);
                        if (index != SelectedRowIndex)
                        {
                            SelectedRowIndex = this.GridRowHeaders.ToList().IndexOf(row);
                            row.IsSelected = true;
                            var tmp = this.GridRowHeaders.FirstOrDefault(x => SelectedRow != null && x.Content == (SelectedRow as GridRowHeader).Content);
                            if (tmp != null)
                            {
                                tmp.IsSelected = false;
                                SelectedRow = null;
                            }
                            SelectedRow = row.Copy();
                        }
                        else
                        {
                            SelectedRowIndex = -1;
                            var tmp = this.GridRowHeaders.FirstOrDefault(x => SelectedRow != null && x.Content == (SelectedRow as GridRowHeader).Content);
                            if (tmp != null)
                            {
                                tmp.IsSelected = false;
                                SelectedRow = null;
                            }
                        }

                        SelectedIndex = -1;
                        SelectedColumnIndex = -1;

                        SelectedItem = null;
                        var tmpc = this.GridColumnHeaders.FirstOrDefault(x => SelectedColumn != null && x.Content == (SelectedColumn as GridColumnHeader).Content);
                        if (tmpc != null)
                        {
                            tmpc.IsSelected = false;
                            SelectedColumn = null;
                        }
                        SelectedData = null;

                        RaiseSelectionChanged(row, TypeValue.Row, TypeSelection.Right);
                    }
                    else if (obj is GridColumnHeader column)
                    {

                        var index = this.GridColumnHeaders.ToList().IndexOf(column);
                        if (index != SelectedColumnIndex)
                        {
                            SelectedColumnIndex = this.GridColumnHeaders.ToList().IndexOf(column);
                            column.IsSelected = true;
                            var tmp = this.GridColumnHeaders.FirstOrDefault(x => SelectedColumn != null && x.Content == (SelectedColumn as GridColumnHeader).Content);
                            if (tmp != null)
                            {
                                tmp.IsSelected = false;
                                SelectedColumn = null;
                            }
                            SelectedColumn = column.Copy();
                        }
                        else
                        {
                            SelectedColumnIndex = -1;

                            var tmp = this.GridColumnHeaders.FirstOrDefault(x => SelectedColumn != null && x.Content == (SelectedColumn as GridColumnHeader).Content);
                            if (tmp != null)
                            {
                                tmp.IsSelected = false;
                                SelectedColumn = null;
                            }
                        }

                        SelectedIndex = -1;
                        SelectedRowIndex = -1;

                        SelectedItem = null;
                        var tmpr = this.GridRowHeaders.FirstOrDefault(x => SelectedRow != null && x.Content == (SelectedRow as GridRowHeader).Content);
                        if (tmpr != null)
                        {
                            tmpr.IsSelected = false;
                            SelectedRow = null;
                        }
                        SelectedData = null;

                        RaiseSelectionChanged(column, TypeValue.Column, TypeSelection.Right);
                    }
                }, obj => obj != null));
            }
        }

        private Command _mouseMiddleClickCommand;
        public Command MouseMiddleClickCommand {
            get {
                return _mouseMiddleClickCommand ?? (_mouseMiddleClickCommand = new Command(obj =>
                {
                    if (SelectedItem != null)
                        ((GridItem)SelectedItem).IsSelected = false;

                    if (SelectedRow != null)
                        ((GridItem)SelectedRow).IsSelected = false;

                    if (SelectedColumn != null)
                        ((GridItem)SelectedColumn).IsSelected = false;

                    if (obj is GridDataItem item)
                    {
                        var index = this.DataItems.ToList().IndexOf(item);
                        if (index != SelectedIndex)
                        {
                            SelectedIndex = this.DataItems.ToList().IndexOf(item);
                            item.IsSelected = true;
                            SelectedItem = item;
                            SelectedData = item.Content;
                        }
                        else
                        {
                            SelectedIndex = -1;
                            (SelectedItem as GridDataItem).IsSelected = false;
                            SelectedItem = null;
                            SelectedData = null;
                        }

                        SelectedRowIndex = -1;
                        SelectedColumnIndex = -1;

                        var tmpr = this.GridRowHeaders.FirstOrDefault(x => SelectedRow != null && x.Content == (SelectedRow as GridRowHeader).Content);
                        if (tmpr != null)
                        {
                            tmpr.IsSelected = false;
                            SelectedRow = null;
                        }
                        var tmpc = this.GridColumnHeaders.FirstOrDefault(x => SelectedColumn != null && x.Content == (SelectedColumn as GridColumnHeader).Content);
                        if (tmpc != null)
                        {
                            tmpc.IsSelected = false;
                            SelectedColumn = null;
                        }

                        RaiseSelectionChanged(item, TypeValue.Item, TypeSelection.Middle, item.Content == null);
                    }
                    else if (obj is GridRowHeader row)
                    {
                        var index = this.GridRowHeaders.ToList().IndexOf(row);
                        if (index != SelectedRowIndex)
                        {
                            SelectedRowIndex = this.GridRowHeaders.ToList().IndexOf(row);
                            row.IsSelected = true;
                            var tmp = this.GridRowHeaders.FirstOrDefault(x => SelectedRow != null && x.Content == (SelectedRow as GridRowHeader).Content);
                            if (tmp != null)
                            {
                                tmp.IsSelected = false;
                                SelectedRow = null;
                            }
                            SelectedRow = row.Copy();
                        }
                        else
                        {
                            SelectedRowIndex = -1;
                            var tmp = this.GridRowHeaders.FirstOrDefault(x => SelectedRow != null && x.Content == (SelectedRow as GridRowHeader).Content);
                            if (tmp != null)
                            {
                                tmp.IsSelected = false;
                                SelectedRow = null;
                            }
                        }

                        SelectedIndex = -1;
                        SelectedColumnIndex = -1;

                        SelectedItem = null;
                        var tmpc = this.GridColumnHeaders.FirstOrDefault(x => SelectedColumn != null && x.Content == (SelectedColumn as GridColumnHeader).Content);
                        if (tmpc != null)
                        {
                            tmpc.IsSelected = false;
                            SelectedColumn = null;
                        }
                        SelectedData = null;

                        RaiseSelectionChanged(row, TypeValue.Row, TypeSelection.Middle);
                    }
                    else if (obj is GridColumnHeader column)
                    {

                        var index = this.GridColumnHeaders.ToList().IndexOf(column);
                        if (index != SelectedColumnIndex)
                        {
                            SelectedColumnIndex = this.GridColumnHeaders.ToList().IndexOf(column);
                            column.IsSelected = true;
                            var tmp = this.GridColumnHeaders.FirstOrDefault(x => SelectedColumn != null && x.Content == (SelectedColumn as GridColumnHeader).Content);
                            if (tmp != null)
                            {
                                tmp.IsSelected = false;
                                SelectedColumn = null;
                            }
                            SelectedColumn = column.Copy();
                        }
                        else
                        {
                            SelectedColumnIndex = -1;

                            var tmp = this.GridColumnHeaders.FirstOrDefault(x => SelectedColumn != null && x.Content == (SelectedColumn as GridColumnHeader).Content);
                            if (tmp != null)
                            {
                                tmp.IsSelected = false;
                                SelectedColumn = null;
                            }
                        }

                        SelectedIndex = -1;
                        SelectedRowIndex = -1;

                        SelectedItem = null;
                        var tmpr = this.GridRowHeaders.FirstOrDefault(x => SelectedRow != null && x.Content == (SelectedRow as GridRowHeader).Content);
                        if (tmpr != null)
                        {
                            tmpr.IsSelected = false;
                            SelectedRow = null;
                        }
                        SelectedData = null;

                        RaiseSelectionChanged(column, TypeValue.Column, TypeSelection.Middle);
                    }
                }, obj => obj != null));
            }
        }


        private Command _keyLeftSelectCommand;
        public Command KeyLeftSelectCommand {
            get { return _keyLeftSelectCommand ?? (_keyLeftSelectCommand = new Command(obj => {
                string h = "";
            }, obj => obj != null)); }
        }

        private Command _keyTopSelectCommand;
        public Command KeyTopSelectCommand {
            get {
                return _keyTopSelectCommand ?? (_keyTopSelectCommand = new Command(obj => {
                    string h = "";
                }, obj => obj != null));
            }
        }

        private Command _keyRightSelectCommand;
        public Command KeyRightSelectCommand {
            get {
                return _keyRightSelectCommand ?? (_keyRightSelectCommand = new Command(obj => {
                    string h = "";
                }, obj => obj != null));
            }
        }

        private Command _keyBottomSelectCommand;
        public Command KeyBottomSelectCommand {
            get {
                return _keyBottomSelectCommand ?? (_keyBottomSelectCommand = new Command(obj => {
                    string h = "";
                }, obj => obj != null));
            }
        }
        #endregion

        #region DependenceProperty
        public static readonly DependencyProperty WaitLayerTemplateProperty;

        public static readonly DependencyProperty DataItemTemplateProperty;

        public static readonly DependencyProperty DataItemTemplateSelectorProperty;

        public static readonly DependencyProperty RowHeaderTemplateProperty;

        public static readonly DependencyProperty RowHeaderTemplateSelectorProperty;

        public static readonly DependencyProperty ColumnHeaderTemplateProperty;

        public static readonly DependencyProperty ColumnHeaderTemplateSelectorProperty;

        public static readonly DependencyProperty DataSourceProperty;

        public static readonly DependencyProperty RowCountProperty;

        public static readonly DependencyProperty ColumnCountProperty;

        public static readonly DependencyProperty ItemWidthProperty;

        public static readonly DependencyProperty ItemHeightProperty;

        public static readonly DependencyProperty DataItemsProperty;

        public static readonly DependencyProperty GridRowHeadersProperty;

        public static readonly DependencyProperty GridColumnHeadersProperty;

        public static readonly DependencyProperty IsBusyProperty;

        private static readonly DependencyPropertyKey RowCountKey;

        private static readonly DependencyPropertyKey ColumnCountKey;

        private static readonly DependencyPropertyKey DataItemsKey;

        private static readonly DependencyPropertyKey GridRowHeadersKey;

        private static readonly DependencyPropertyKey GridColumnHeadersKey;

        private static readonly DependencyPropertyKey IsBusyKey;


        private static readonly DependencyProperty GroupHeightProperty;

        private static readonly DependencyProperty GroupWidthProperty;


        private static readonly DependencyProperty RowHeaderCountProperty;

        private static readonly DependencyProperty ColumnHeaderCountProperty;

        private static readonly DependencyPropertyKey RowHeaderCountKey;

        private static readonly DependencyPropertyKey ColumnHeaderCountKey;


        private static readonly DependencyProperty RowLineColorProperty;

        private static readonly DependencyProperty ColumnLineColorProperty;

        private static readonly DependencyProperty SelectColorProperty;

        public static readonly DependencyProperty SelectedItemProperty;
        public static readonly DependencyProperty SelectedColumnProperty;
        public static readonly DependencyProperty SelectedRowProperty;

        public static readonly DependencyProperty SelectedIndexProperty;
        public static readonly DependencyProperty SelectedRowIndexProperty;
        public static readonly DependencyProperty SelectedColumnIndexProperty;

        public static readonly DependencyProperty SelectedDataProperty;

        public static readonly DependencyProperty SelectedDataInGroupProperty;
        #endregion

        #region Fields
        #region Private
        private readonly EventHelper<object> scrollEventHelper;

        private int displayedRows;

        private int displayedCols;

        private int displayedRowScrollOffset = -1;

        private int displayedColScrollOffset = -1;

        private bool updateDataSourceAgain;
        
        // Cache
        private KeyValuePair<object, Position>[] rowHeaderCache;
        private KeyValuePair<object, Position>[] colHeaderCache;

        private object[,] itemSourceCache;
        private Position[,] itemMatrixCache;
        #endregion

        #region Protected
        protected Func<object, IEnumerable<object>> rowSelecter;
        protected Func<object, IEnumerable<object>> colSelector;

        protected Func<IEnumerable<object>> rowsSelector;
        protected Func<IEnumerable<object>> columnsSelector;

        protected Func<object, int, Position> rowPostion;
        protected Func<object, int, Position> columnPosition;

        protected Func<object, object>[] groupRowSelector;
        protected Func<object, object>[] groupColSelector;

        protected Func<object, object, int> itemRowLeft;
        protected Func<object, object, object, int> itemRowRight;

        protected Func<object, object, int> itemColLeft;
        protected Func<object, object, object, int> itemColRight;

        protected Func<IEnumerable<object>, object> stateChange;
        #endregion

        #region Dependency property accessors
        public ControlTemplate WaitLayerTemplate { get { return (ControlTemplate)this.GetValue(WaitLayerTemplateProperty); } set { this.SetValue(WaitLayerTemplateProperty, value); } }

        public DataTemplate DataItemTemplate { get { return (DataTemplate)this.GetValue(DataItemTemplateProperty); } set { this.SetValue(DataItemTemplateProperty, value); } }

        public DataTemplateSelector DataItemTemplateSelector { get { return (DataTemplateSelector)this.GetValue(DataItemTemplateSelectorProperty); } set { this.SetValue(DataItemTemplateSelectorProperty, value); } }

        public DataTemplate RowHeaderTemplate { get { return (DataTemplate)this.GetValue(RowHeaderTemplateProperty); } set { this.SetValue(RowHeaderTemplateProperty, value); } }

        public DataTemplateSelector RowHeaderTemplateSelector { get { return (DataTemplateSelector)this.GetValue(RowHeaderTemplateSelectorProperty); } set { this.SetValue(RowHeaderTemplateSelectorProperty, value); } }

        public DataTemplate ColumnHeaderTemplate { get { return (DataTemplate)this.GetValue(ColumnHeaderTemplateProperty); } set { this.SetValue(ColumnHeaderTemplateProperty, value); } }

        public DataTemplateSelector ColumnHeaderTemplateSelector { get { return (DataTemplateSelector)this.GetValue(ColumnHeaderTemplateSelectorProperty); } set { this.SetValue(ColumnHeaderTemplateSelectorProperty, value); } }

        public IEnumerable<GridDataItem> DataItems { get { return (IEnumerable<GridDataItem>)this.GetValue(DataItemsProperty); } protected set { this.SetValue(DataItemsKey, value); } }
        
        public IEnumerable<GridRowHeader> GridRowHeaders { get { return (IEnumerable<GridRowHeader>)this.GetValue(GridRowHeadersProperty); } private set { this.SetValue(GridRowHeadersKey, value); } }

        public IEnumerable<GridColumnHeader> GridColumnHeaders { get { return (IEnumerable<GridColumnHeader>)this.GetValue(GridColumnHeadersProperty); } private set { this.SetValue(GridColumnHeadersKey, value); } }

        public IEnumerable DataSource { get { return (IEnumerable)this.GetValue(DataSourceProperty); } set { this.SetValue(DataSourceProperty, value); } }

        public int RowCount { get { return (int)this.GetValue(RowCountProperty); } private set { this.SetValue(RowCountKey, value); } }

        public int ColumnCount { get { return (int)this.GetValue(ColumnCountProperty); } private set { this.SetValue(ColumnCountKey, value); } }

        public double ItemWidth { get { return (double)this.GetValue(ItemWidthProperty); } set { this.SetValue(ItemWidthProperty, value); } }

        public double ItemHeight { get { return (double)this.GetValue(ItemHeightProperty); } set { this.SetValue(ItemHeightProperty, value); } }

        public bool IsBusy { get { return (bool)this.GetValue(IsBusyProperty); } set { this.SetValue(IsBusyKey, value); } }

        public double GroupWidth { get { return (double)this.GetValue(GroupWidthProperty); } set { this.SetValue(GroupWidthProperty, value); } }

        public double GroupHeight { get { return (double)this.GetValue(GroupHeightProperty); } set { this.SetValue(GroupHeightProperty, value); } }

        public int RowHeaderCount { get { return (int)this.GetValue(RowHeaderCountProperty); } private set { this.SetValue(RowHeaderCountKey, value); } }

        public int ColumnHeaderCount { get { return (int)this.GetValue(ColumnHeaderCountProperty); } private set { this.SetValue(ColumnHeaderCountKey, value); } }

        public Color RowLineColor { get { return (Color)this.GetValue(RowLineColorProperty); } set { this.SetValue(RowLineColorProperty, value); } }

        public Color ColumnLineColor { get { return (Color)this.GetValue(ColumnLineColorProperty); } set { this.SetValue(ColumnLineColorProperty, value); } }

        public Color SelectColor { get { return (Color)this.GetValue(SelectColorProperty); } set { this.SetValue(SelectColorProperty, value); } }


        public object SelectedItem { get { return this.GetValue(SelectedItemProperty); } set { this.SetValue(SelectedItemProperty, value); } }

        public object SelectedRow { get { return this.GetValue(SelectedRowProperty); } set { this.SetValue(SelectedRowProperty, value); } }

        public object SelectedColumn { get { return this.GetValue(SelectedColumnProperty); } set { this.SetValue(SelectedColumnProperty, value); } }

        public int SelectedIndex { get { return (int)this.GetValue(SelectedIndexProperty); } set { this.SetValue(SelectedIndexProperty, value); } }

        public int SelectedRowIndex { get { return (int)this.GetValue(SelectedRowIndexProperty); } set { this.SetValue(SelectedRowIndexProperty, value); } }

        public int SelectedColumnIndex { get { return (int)this.GetValue(SelectedColumnIndexProperty); } set { this.SetValue(SelectedColumnIndexProperty, value); } }

        public object SelectedData { get { return this.GetValue(SelectedDataProperty); } set { this.SetValue(SelectedDataProperty, value); } }

        public object SelectedDataInGroup { get { return (object)this.GetValue(SelectedDataInGroupProperty); } set { this.SetValue(SelectedDataInGroupProperty, value); } }
        #endregion

        #region Public
        private Tuple<Position, object>[] matrix;
        public Tuple<Position, object>[] Matrix {
            get {
                return matrix;
            }
            private set { matrix = value; this.SendPropertyChanged(nameof(Matrix)); }
        }

        public int RowCountMax {
            get {
                return this.rowHeaderCache == null ? 0 : this.rowHeaderCache.Length;
            }
        }

        public int ColumnCountMax {
            get {
                return this.colHeaderCache == null ? 0 : this.colHeaderCache.Length;
            }
        }

        #endregion
        #endregion

        static GridControl() {
            // Initialize read/write properties
            DataItemTemplateProperty = DependencyProperty.Register("DataItemTemplate", typeof(DataTemplate), typeof(GridControl), new PropertyMetadata(null));
            DataItemTemplateSelectorProperty = DependencyProperty.Register("DataItemTemplateSelector", typeof(DataTemplateSelector), typeof(GridControl), new PropertyMetadata(null));
            RowHeaderTemplateProperty = DependencyProperty.Register("RowHeaderTemplate", typeof(DataTemplate), typeof(GridControl), new PropertyMetadata(null));
            RowHeaderTemplateSelectorProperty = DependencyProperty.Register("RowHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(GridControl), new PropertyMetadata(null));
            ColumnHeaderTemplateProperty = DependencyProperty.Register("ColumnHeaderTemplate", typeof(DataTemplate), typeof(GridControl), new PropertyMetadata(null));
            ColumnHeaderTemplateSelectorProperty = DependencyProperty.Register("ColumnHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(GridControl), new PropertyMetadata(null));

            WaitLayerTemplateProperty = DependencyProperty.Register("WaitLayerTemplate", typeof(ControlTemplate), typeof(GridControl), new PropertyMetadata(null));
            DataSourceProperty = DependencyProperty.Register("DataSource", typeof(IEnumerable), typeof(GridControl), new PropertyMetadata(null, GridControl.DataSourceChanged));
            ItemWidthProperty = DependencyProperty.Register("ItemWidth", typeof(double), typeof(GridControl), new PropertyMetadata(130.0, ItemSizeChangedHandler));
            ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(GridControl), new PropertyMetadata(35.0, ItemSizeChangedHandler));

            GroupWidthProperty = DependencyProperty.Register("GroupWidth", typeof(double), typeof(GridControl), new PropertyMetadata(130.0));
            GroupHeightProperty = DependencyProperty.Register("GroupHeight", typeof(double), typeof(GridControl), new PropertyMetadata(35.0));

            // Initialize readonly property keys
            ColumnCountKey = DependencyProperty.RegisterReadOnly("ColumnCount", typeof(int), typeof(GridControl), new PropertyMetadata(0));
            RowCountKey = DependencyProperty.RegisterReadOnly("RowCount", typeof(int), typeof(GridControl), new PropertyMetadata(0));
            DataItemsKey = DependencyProperty.RegisterReadOnly("DataItems", typeof(IEnumerable<GridDataItem>), typeof(GridControl), new PropertyMetadata(null));
            GridRowHeadersKey = DependencyProperty.RegisterReadOnly("GridRowHeaders", typeof(IEnumerable<GridRowHeader>), typeof(GridControl), new PropertyMetadata(null));
            GridColumnHeadersKey = DependencyProperty.RegisterReadOnly("GridColumnHeaders", typeof(IEnumerable<GridColumnHeader>), typeof(GridControl), new PropertyMetadata(null));
            IsBusyKey = DependencyProperty.RegisterReadOnly("IsBusy", typeof(bool), typeof(GridControl), new PropertyMetadata(false));

            RowHeaderCountKey = DependencyProperty.RegisterReadOnly("RowHeaderCount", typeof(int), typeof(GridControl), new PropertyMetadata(0));
            ColumnHeaderCountKey = DependencyProperty.RegisterReadOnly("ColumnHeaderCount", typeof(int), typeof(GridControl), new PropertyMetadata(0));

            // Initialize readonly properties
            ColumnCountProperty = ColumnCountKey.DependencyProperty;
            RowCountProperty = RowCountKey.DependencyProperty;
            DataItemsProperty = DataItemsKey.DependencyProperty;
            GridRowHeadersProperty = GridRowHeadersKey.DependencyProperty;
            GridColumnHeadersProperty = GridColumnHeadersKey.DependencyProperty;
            IsBusyProperty = IsBusyKey.DependencyProperty;

            RowHeaderCountProperty = RowHeaderCountKey.DependencyProperty;
            ColumnHeaderCountProperty = ColumnHeaderCountKey.DependencyProperty;

            // Visual
            RowLineColorProperty = DependencyProperty.Register("RowLineColor", typeof(Color), typeof(GridControl), new PropertyMetadata(Colors.DarkGray));
            ColumnLineColorProperty = DependencyProperty.Register("ColumnLineColor", typeof(Color), typeof(GridControl), new PropertyMetadata(Colors.DarkGray));

            SelectColorProperty = DependencyProperty.Register("SelectColor", typeof(Color), typeof(GridControl), new PropertyMetadata(Colors.DodgerBlue));

            // Events
            SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(GridControl), new PropertyMetadata(null));
            SelectedRowProperty = DependencyProperty.Register("SelectedRow", typeof(object), typeof(GridControl), new PropertyMetadata(null));
            SelectedColumnProperty = DependencyProperty.Register("SelectedColumn", typeof(object), typeof(GridControl), new PropertyMetadata(null));

            SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(GridControl), new PropertyMetadata(-1));
            SelectedRowIndexProperty = DependencyProperty.Register("SelectedRowIndex", typeof(int), typeof(GridControl), new PropertyMetadata(-1));
            SelectedColumnIndexProperty = DependencyProperty.Register("SelectedColumnIndex", typeof(int), typeof(GridControl), new PropertyMetadata(-1));

            SelectedDataProperty = DependencyProperty.Register("SelectedData", typeof(object), typeof(GridControl), new PropertyMetadata(null));

            SelectedDataInGroupProperty = DependencyProperty.Register("SelectedDataInGroup", typeof(object), typeof(GridControl), new PropertyMetadata(null));
        }

        protected GridControl() {
            this.scrollEventHelper = new EventHelper<object>(this.Dispatcher, DispatcherPriority.Background, 200, arg => this.UpdateDisplayData());
        }

        #region Methods
        #region Override
        protected override void OnScroll()
        {
            this.scrollEventHelper.Trigger(null);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            this.UpdateDisplayData();
        }
        #endregion

        #region Static
        private static void DataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = (GridControl)d;
            grid.DataSourceChanged();
        }

        private static void ItemSizeChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var grid = (GridControl)d;

            var rows = grid.rowHeaderCache == null ? 0 : grid.rowHeaderCache.Length;
            var cols = grid.colHeaderCache == null ? 0 : grid.colHeaderCache.Length;
            grid.SetScrollExtent(
                grid.ItemWidth, grid.ItemHeight,
                grid.ItemWidth * (cols + 1), 
                grid.ItemHeight * (rows + 1)
                ); /* +1 because of the headers */

            grid.UpdateDisplayData();
        }

        #endregion

        #region Metric
        protected void DataSourceChanged()
        {
            if (this.IsBusy)
            {
                this.updateDataSourceAgain = true;
                return;
            }

            this.IsBusy = true;

            bool updateAgain = false;

            // 0. clear select parametry
            SelectedIndex = -1;
            SelectedItem = null;
            SelectedData = null;

            SelectedRowIndex = -1;
            SelectedColumnIndex = -1;
            SelectedRow = null;
            SelectedColumn = null;


            Task.Factory.StartNew(() => {
                do
                {
                    // 1 step. Load data source
                    IEnumerable dataSource = null;
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        dataSource = this.DataSource;
                    }));

                    // 2 step. Update header cache
                    List<object> items = null;
                    if (dataSource != null)
                        items = dataSource.Cast<object>().ToList();
                    else
                        items = new List<object>();

                    this.UpdateHeaderCache(items);

                    // 3 step. Check update data source
                    if (this.updateDataSourceAgain)
                    {
                        updateAgain = true;
                        this.updateDataSourceAgain = false;
                        continue;
                    }

                    // 4 step. build item source cache
                    var itemCache = this.BuildItemSourceCache(items);

                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        this.itemSourceCache = itemCache;
                    }));

                    // 5 step. Set scroll extent
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        this.RowHeaderCount = 1;
                        this.ColumnHeaderCount = 1;

                        if (this.groupColSelector != null)
                            this.RowHeaderCount = this.groupColSelector.Length + 1;

                        if (this.groupRowSelector != null)
                            this.ColumnHeaderCount = this.groupRowSelector.Length + 1;

                        this.SetScrollExtent(
                            this.ItemWidth,
                            this.ItemHeight,
                            this.ItemWidth * (this.colHeaderCache.Length + 1) + (this.GroupWidth * this.ColumnHeaderCount + 1),
                            this.ItemHeight * (this.rowHeaderCache.Length + 1) + (this.GroupHeight * this.RowHeaderCount + 1));
                    }));

                    // 6 step. Should we run again
                    updateAgain = this.updateDataSourceAgain;
                    this.updateDataSourceAgain = false;

                } while (updateAgain);
            }).ContinueWith((task) => {
                this.IsBusy = false;
                // 7 step. Finally, updates display data
                this.UpdateDisplayData(true);

                this.SendPropertyChanged(nameof(RowCountMax));
                this.SendPropertyChanged(nameof(ColumnCountMax));
                this.SendPropertyChanged(nameof(Matrix));
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void UpdateDisplayData(bool forceUpdate = false)
        {
            try
            {
                if (this.IsBusy || this.rowHeaderCache == null || this.colHeaderCache == null) return;

                if (this.DataSource == null || !this.DataSource.Cast<object>().Any())
                {
                    this.DataItems = null;
                }

                List<GridDataItem> dataItems = null;
                List<GridRowHeader> gridRowHeaders = null;
                List<GridColumnHeader> gridColumnHeaders = null;
                Action<object, int, int, int, Position, bool, bool, bool> setRowHeader = (row, index, rowPos, colPos, position, line, isGroup, isSelected) =>
                {
                    gridRowHeaders[index].Set(row, rowPos, colPos, position.RowSpan, position.ColumnSpan, new Thickness(), isSelected);
                };
                Action<object, int, int, int, Position, bool, bool, bool> setColHeader = (col, index, rowPos, colPos, position, line, isGroup, isSelected) =>
                {
                    gridColumnHeaders[index].Set(col, rowPos, colPos, position.RowSpan, position.ColumnSpan, new Thickness(), isSelected);
                };


                var viewPortWidth = this.ActualWidth;
                var viewPortHeight = this.ActualHeight;
                var visibleRowCount = (int)((viewPortHeight - this.RowHeaderCount * this.GroupHeight) / this.ItemHeight);
                var visibleColumnCount = (int)((viewPortWidth - this.ColumnHeaderCount * this.GroupWidth) / this.ItemWidth);

                // Calculate current scroll offsets
                var rowScrollOffset = (int)(this.VerticalOffset / this.ItemHeight);
                var colScrollOffset = (int)(this.HorizontalOffset / this.ItemWidth);
                if (rowScrollOffset > this.rowHeaderCache.Length - visibleRowCount) rowScrollOffset = Math.Max(this.rowHeaderCache.Length - visibleRowCount, 0);
                if (colScrollOffset > this.colHeaderCache.Length - visibleColumnCount) colScrollOffset = Math.Max(this.colHeaderCache.Length - visibleColumnCount, 0);

                // Get visible row / column headers
                var visibleRows = this.rowHeaderCache.Skip(rowScrollOffset).Take(visibleRowCount).ToList();
                var visibleColumns = this.colHeaderCache.Skip(colScrollOffset).Take(visibleColumnCount).ToList();
                var ActualVisibleRowCount = visibleRowCount;
                visibleRowCount = visibleRows.Count();
                var ActualVisibleColumnCount = visibleColumnCount;
                visibleColumnCount = visibleColumns.Count();
                this.ColumnCount = visibleColumnCount + this.ColumnHeaderCount;
                this.RowCount = visibleRowCount + this.RowHeaderCount;


                var regenerateDataItems = this.DataItems == null || this.displayedRows != visibleRowCount || this.displayedCols != visibleColumnCount;

                if (regenerateDataItems)
                {
                    // 6 step. Display data is created, headers must be added
                    var rowie = Enumerable.Range(0, ActualVisibleRowCount * this.ColumnHeaderCount);
                    var colie = Enumerable.Range(0, ActualVisibleColumnCount * this.RowHeaderCount);
                    var itemie = Enumerable.Range(0, ActualVisibleRowCount * ActualVisibleColumnCount);

                    dataItems = itemie.Select(x => new GridDataItem(1, 1, 1, 1, new Thickness(), null)).ToList();

                    gridRowHeaders = rowie.Select(x => new GridRowHeader(1, 1, 1, null)).ToList();
                    gridColumnHeaders = colie.Select(x => new GridColumnHeader(1, 1, 1, null)).ToList();
                }
                else
                {
                    // 7 step. Display data already exists, headers are already present
                    dataItems = (List<GridDataItem>)this.DataItems;
                    gridRowHeaders = (List<GridRowHeader>)this.GridRowHeaders;
                    gridColumnHeaders = (List<GridColumnHeader>)this.GridColumnHeaders;

                    for (int i = 0; i < gridColumnHeaders.Count; i++)
                        setColHeader(null, i, 0, 0, Position.Default(), true, false, false);

                    for (int i = 0; i < gridRowHeaders.Count; i++)
                        setRowHeader(null, i, 0, 0, Position.Default(), true, false, false);
                }

                if (itemSourceCache != null)
                {
                    List<object> temp = new List<object>(visibleColumnCount);
                    for (int y = rowScrollOffset; y < rowScrollOffset + visibleRowCount; y++)
                        for (int x = colScrollOffset; x < colScrollOffset + visibleColumnCount; x++)
                        {
                            var content = this.itemSourceCache[x, y];

                            if (content != null && !temp.Contains(content))
                                temp.Add(content);
                            else if (content != null) content = 1;
                            var cs = this.itemMatrixCache[x, y].ColumnSpan;

                            var isSelected = SelectedData == content && SelectedData != null;

                            dataItems[((y - rowScrollOffset) * visibleColumnCount) + (x - colScrollOffset)].Set(content,
                                (y - rowScrollOffset) + this.RowHeaderCount, (x - colScrollOffset) + this.ColumnHeaderCount,
                                this.itemMatrixCache[x, y].RowSpan, this.itemMatrixCache[x, y].ColumnSpan, new Thickness(), isSelected);
                        }
                    temp.Clear();
                    temp = null;
                }

                var tempRow = new Dictionary<object, int>(visibleRows.Count);
                var ind = 0;
                var nextStep = 1 + this.RowHeaderCount - 1;
                for (int i = 0; i < visibleRows.Count; i++)
                {
                    var row = visibleRows[i];
                    if (row.Key == null || tempRow.ContainsKey(row.Key))
                        continue;

                    tempRow[row.Key] = row.Value.RowSpan;

                    var isSelected = SelectedRow != null && (SelectedRow as GridRowHeader).Content == row.Key;

                    setRowHeader(row.Key, ind, nextStep, this.ColumnHeaderCount - 1, row.Value, true, false, isSelected);
                    nextStep = nextStep + row.Value.RowSpan;
                    ind++;
                }
                //Console.WriteLine("row fine");
                //row grouping
                if (groupRowSelector != null)
                    for (int i = 0; i < groupRowSelector.Length; i++)
                    {
                        var gr = tempRow.GroupBy(x => groupRowSelector[i](x.Key)).Select(x => new KeyValuePair<object, int>(x.Key, x.Sum(y => y.Value)));

                        nextStep = 1 + this.RowHeaderCount - 1;
                        foreach (var g in gr)
                        {
                            var pos = new Position(1, 1, 1, g.Value, 1);

                            var isSelected = SelectedRow != null && (SelectedRow as GridRowHeader).Content == g.Key;
                            if (isSelected)
                            {
                                string h = "";
                            }
                            setRowHeader(g.Key, ind, nextStep, i, pos, false, true, isSelected);
                            nextStep = nextStep + g.Value;
                            ind++;
                        }
                    }
                tempRow = null;
                // Generate column headers
                var tempCol = new Dictionary<object, int>(visibleColumns.Count);
                ind = 0;
                nextStep = 1 + this.ColumnHeaderCount - 1;
                for (int i = 0; i < visibleColumns.Count; i++)
                {
                    var col = visibleColumns[i];

                    if (col.Key == null || tempCol.ContainsKey(col.Key))
                        continue;

                    tempCol[col.Key] = col.Value.ColumnSpan;

                    var isSelected = SelectedColumn != null && (SelectedColumn as GridColumnHeader).Content == col.Key;

                    setColHeader(col.Key, ind, this.RowHeaderCount - 1, nextStep, col.Value, true, false, isSelected);
                    nextStep = nextStep + col.Value.ColumnSpan;
                    ind++;
                }
                // column grouping
                if (groupColSelector != null)
                    for (int i = 0; i < groupColSelector.Length; i++)
                    {
                        var gc = tempCol.GroupBy(x => groupColSelector[i](x.Key)).Select(x => new KeyValuePair<object, int>(x.Key, x.Sum(y => y.Value))).ToArray();

                        nextStep = 1 + this.ColumnHeaderCount - 1;
                        foreach (var g in gc)
                        {
                            var pos = new Position(1, 1, 1, 1, g.Value);

                            var isSelected = SelectedColumn != null && (SelectedColumn as GridColumnHeader).Content == g.Key;

                            setColHeader(g.Key, ind, i, nextStep, pos, false, true, isSelected);
                            nextStep = nextStep + g.Value;
                            ind++;
                        }
                    }
                tempCol = null;
                //Console.WriteLine("col fine");

                if (regenerateDataItems)
                {
                    this.DataItems = dataItems;
                    this.GridColumnHeaders = gridColumnHeaders;
                    this.GridRowHeaders = gridRowHeaders;

                    //Console.WriteLine("regenerateDataItems fine");
                }

                this.displayedRows = visibleRowCount;
                this.displayedCols = visibleColumnCount;
                this.displayedRowScrollOffset = rowScrollOffset;
                this.displayedColScrollOffset = colScrollOffset;

                //Console.WriteLine("fine");
            }
            catch (Exception e)
            {
                string h = e.Message;
            }
        }

        private object[,] BuildItemSourceCache(IEnumerable<object> items)
        {
            var rowc = this.rowHeaderCache.Sum(x => x.Value.RowSpan);
            var colc = this.colHeaderCache.Sum(x => x.Value.ColumnSpan);

            var rowCache = this.rowHeaderCache.GroupBy(x => x.Key).ToDictionary(key => key.Key, value => value.First().Value);
            var colCache = this.colHeaderCache.GroupBy(x => x.Key).ToDictionary(key => key.Key, value => value.First().Value);

            var itemSource = new object[colc, rowc];
            itemMatrixCache = Position.Create(rowc, colc);
            var tempMatrix = new List<Tuple<Position, object>>(items.Count());

            foreach (var item in items)
            {
                var rowv = rowSelecter(item);
                var colv = colSelector(item);

                if (rowv.Count(x => rowCache.ContainsKey(x)) == 0
                    || colv.Count(x => colCache.ContainsKey(x)) == 0)
                    continue;

                foreach (var r in rowv)
                    if (rowCache.ContainsKey(r))
                    {
                        foreach (var c in colv)
                            if (colCache.ContainsKey(c))
                            {
                                var removeRowSpan = 0;
                                
                                var removeColSpan = 0;

                                var rowPosition = rowCache.Take(rowCache[r].Index - 1).Sum(x => x.Value.RowSpan);

                                var colPosition = colCache.Take(colCache[c].Index - 1).Sum(x => x.Value.ColumnSpan);

                                if (itemRowLeft != null)
                                    rowPosition += itemRowLeft(item, r);

                                if (itemRowRight != null)
                                    removeRowSpan = itemRowRight(item, r, rowCache.Keys.Last());

                                if (itemColLeft != null)
                                    colPosition += itemColLeft(item, c);

                                if (itemColRight != null)
                                    removeColSpan = itemColRight(item, c, colCache.Keys.Last());

                                if (itemSource[colPosition, rowPosition] == null && itemMatrixCache[colPosition, rowPosition] == Position.Default())
                                {
                                    var rc = 1; //rowv.Where(x => rowCache.ContainsKey(x)).Sum(x => rowCache[x].RowSpan);
                                    var cc = colv.Where(x => colCache.ContainsKey(x)).Sum(x => colCache[x].ColumnSpan) - removeColSpan;

                                    tempMatrix.Add(new Tuple<Position, object>(new Position(1, rowPosition, colPosition, rc, cc), item));

                                    //for (int y = 0; y < rc; y++)
                                        for (int x = 0; x < cc; x++)
                                        {
                                            if (colPosition + x < colc && rowPosition < rowc)
                                            {
                                                Position itemPosition = new Position(1, rowPosition, colPosition - x < 0 ? 0 : colPosition - x, rc, cc - x);
                                                itemSource[colPosition + x, rowPosition] = item;
                                                itemMatrixCache[colPosition + x, rowPosition] = itemPosition;
                                            }
                                        }
                                }
                                else
                                {
                                    var onMaxRow = Math.Min(rowCache[r].RowSpan, rowc - rowPosition);

                                    //if (itemRowLeft != null)
                                    //    rowPosition += itemRowLeft(item);

                                    //if (itemRowRight != null)
                                    //    removeRowSpan = itemRowRight(item);

                                    //if (itemColLeft != null)
                                    //    colPosition += itemColLeft(item);

                                    //if (itemColRight != null)
                                    //    removeColSpan = itemColRight(item);

                                    for (int i = 0; i < onMaxRow; i++)
                                    {
                                        if (itemSource[colPosition, rowPosition + i] == null && itemMatrixCache[colPosition, rowPosition + i] == Position.Default())
                                        {
                                            var rc = 1;
                                            var cc = colv.Where(x => colCache.ContainsKey(x)).Sum(x => colCache[x].ColumnSpan) - removeColSpan;

                                            tempMatrix.Add(new Tuple<Position, object>(new Position(1, rowPosition + i, colPosition, rc, cc), item));
                                            //for (int y = 0; y < rc; y++)
                                            for (int x = 0; x < cc; x++)
                                                {
                                                    if (colPosition + x < colc && rowPosition + i < rowc)
                                                    {
                                                        Position itemPosition = new Position(1, rowPosition + i, colPosition - x < 0 ? 0 : colPosition - x, rc, cc - x);
                                                        itemSource[colPosition + x, rowPosition + i] = item;
                                                        itemMatrixCache[colPosition + x, rowPosition + i] = itemPosition;
                                                    }
                                                }
                                            break;
                                        }
                                    }
                                }
                                break;
                            }
                        break;
                    }
            }

//#if DEBUG
//            for (int y = 0; y < rowc; y++)
//            {
//                for (int x = 0; x < colc; x++)
//                {
//                    //Console.Write(itemSource[x, y] == null? "[null] ": itemSource[x, y] + " ");
//                    Console.Write("[" + itemMatrixCache[x, y].Row + " | " + itemMatrixCache[x, y].Column + " | " + itemMatrixCache[x, y].RowSpan + " | " + itemMatrixCache[x, y].ColumnSpan + "] ");
//                }
//                Console.WriteLine();
//            }
//            Console.WriteLine();
//            for (int y = 0; y < rowc; y++)
//            {
//                for (int x = 0; x < colc; x++)
//                {
//                    Console.Write(itemSource[x, y] == null ? "[null] " : itemSource[x, y].ToString().Substring(0, 6) + " ");
//                    //Console.Write("[" + itemMatrixCache[x, y].RowSpan + " | " + itemMatrixCache[x, y].ColumnSpan + "] ");
//                }
//                Console.WriteLine();
//            }
//#endif

            this.matrix = tempMatrix.ToArray();

            return itemSource;
        }

        private void UpdateHeaderCache(List<object> items)
        {
            Action updateRowCache = () => { };
            Action updateColCache = () => { };

            if (rowsSelector != null)
            {
                updateRowCache = () => {
                    var rows = this.rowsSelector().ToArray();
                    var count = rows.Count();
                    var temp = new List<KeyValuePair<object, Position>>(count);

                    if (groupRowSelector != null)
                        foreach (var grouping in groupRowSelector)
                            rows = rows.OrderBy(x => grouping(x)).ToArray();

                    for (int i = 0; i < count; i++)
                    {
                        var row = rows[i];
                        var position = rowPostion(row, i + 1);
                        for (int j = 0; j < position.RowSpan; j++)
                        {
                            var actPos = position;
                            actPos.RowSpan -= j;
                            temp.Add(new KeyValuePair<object, Position>(row, actPos));
                        }
                    }

                    this.rowHeaderCache = temp.ToArray();
                };
            }
            else
            {
                var rows = items.SelectMany(dataItem => this.rowSelecter(dataItem)).Distinct().ToArray();
                var count = rows.Count();
                var temp = new List<KeyValuePair<object, Position>>(count);

                if (groupRowSelector != null)
                    foreach (var grouping in groupRowSelector)
                        rows = rows.OrderBy(x => grouping(x)).ToArray();

                for (int i = 0; i < count; i++)
                {
                    var row = rows[i];
                    var position = rowPostion(row, i + 1);
                    for (int j = 0; j < position.RowSpan; j++)
                    {
                        var actPos = position;
                        actPos.RowSpan -= j;
                        temp.Add(new KeyValuePair<object, Position>(row, actPos));
                    }
                }

                this.rowHeaderCache = temp.ToArray();
            }

            if (columnsSelector != null)
            {
                var cols = this.columnsSelector().ToArray();

                var count = cols.Count();
                var temp = new List<KeyValuePair<object, Position>>(count);

                if (groupColSelector != null)
                    foreach (var grouping in groupColSelector)
                        cols = cols.OrderBy(x => grouping(x)).ToArray();

                // step
                for (int i = 0; i < count; i++)
                {
                    var col = cols[i];
                    var position = columnPosition(col, i + 1);
                    for (int j = 0; j < position.ColumnSpan; j++) {
                        var actPos = position;
                        actPos.ColumnSpan -= j;
                        temp.Add(new KeyValuePair<object, Position>(col, actPos));
                    }
                }

                this.colHeaderCache = temp.ToArray();
            }
            else
            {
                var cols = items.SelectMany(dataItem => this.colSelector(dataItem)).Distinct().ToArray();
                var count = cols.Count();
                var temp = new List<KeyValuePair<object, Position>>(count);

                if (groupColSelector != null)
                    foreach (var grouping in groupColSelector)
                        cols = cols.OrderBy(x => grouping(x)).ToArray();

                for (int i = 0; i < count; i++)
                {
                    var col = cols[i];
                    var position = columnPosition(col, i + 1);
                    for (int j = 0; j < position.ColumnSpan; j++)
                    {
                        var actPos = position;
                        actPos.ColumnSpan -= j;
                        temp.Add(new KeyValuePair<object, Position>(col, actPos));
                    }
                }

                this.colHeaderCache = temp.ToArray();
            }

            Parallel.Invoke(updateRowCache, updateColCache);
        }
        #endregion

        #region Helps
        public void AddItem(object item) {
            var rowc = this.rowHeaderCache.Sum(x => x.Value.RowSpan);
            var colc = this.colHeaderCache.Sum(x => x.Value.ColumnSpan);

            var rowCache = this.rowHeaderCache.GroupBy(x => x.Key).ToDictionary(key => key.Key, value => value.First().Value);
            var colCache = this.colHeaderCache.GroupBy(x => x.Key).ToDictionary(key => key.Key, value => value.First().Value);

            var rowv = rowSelecter(item);
            var colv = colSelector(item);

            if (rowv.Count(x => rowCache.ContainsKey(x)) == 0
                || colv.Count(x => colCache.ContainsKey(x)) == 0)
                return;
            var t = this.Matrix.ToList();
            foreach (var r in rowv)
                if (rowCache.ContainsKey(r))
                {
                    foreach (var c in colv)
                        if (colCache.ContainsKey(c))
                        {
                            var removeRowSpan = 0;

                            var removeColSpan = 0;

                            var rowPosition = rowCache.Take(rowCache[r].Index - 1).Sum(x => x.Value.RowSpan);

                            var colPosition = colCache.Take(colCache[c].Index - 1).Sum(x => x.Value.ColumnSpan);

                            if (itemRowLeft != null)
                                rowPosition += itemRowLeft(item, r);

                            if (itemRowRight != null)
                                removeRowSpan = itemRowRight(item, r, rowCache.Keys.Last());

                            if (itemColLeft != null)
                                colPosition += itemColLeft(item, c);

                            if (itemColRight != null)
                                removeColSpan = itemColRight(item, c, colCache.Keys.Last());

                            if (this.itemSourceCache[colPosition, rowPosition] == null && this.itemMatrixCache[colPosition, rowPosition] == Position.Default())
                            {
                                var rc = 1; //rowv.Where(x => rowCache.ContainsKey(x)).Sum(x => rowCache[x].RowSpan);
                                var cc = colv.Where(x => colCache.ContainsKey(x)).Sum(x => colCache[x].ColumnSpan) - removeColSpan;

                                t.Add(new Tuple<Position, object>(new Position(1, rowPosition, colPosition, rc, cc), item));

                                //for (int y = 0; y < rc; y++)
                                for (int x = 0; x < cc; x++)
                                {
                                    if (colPosition + x < colc && rowPosition < rowc)
                                    {
                                        Position itemPosition = new Position(1, rowPosition, colPosition - x < 0 ? 0 : colPosition - x, rc, cc - x);
                                        this.itemSourceCache[colPosition + x, rowPosition] = item;
                                        itemMatrixCache[colPosition + x, rowPosition] = itemPosition;
                                    }
                                }
                            }
                            else
                            {
                                var onMaxRow = Math.Min(rowCache[r].RowSpan, rowc - rowPosition);

                                for (int i = 0; i < onMaxRow; i++)
                                {
                                    if (this.itemSourceCache[colPosition, rowPosition + i] == null && this.itemMatrixCache[colPosition, rowPosition + i] == Position.Default())
                                    {
                                        var rc = 1;
                                        var cc = colv.Where(x => colCache.ContainsKey(x)).Sum(x => colCache[x].ColumnSpan) - removeColSpan;

                                        t.Add(new Tuple<Position, object>(new Position(1, rowPosition + i, colPosition, rc, cc), item));

                                        //for (int y = 0; y < rc; y++)
                                        for (int x = 0; x < cc; x++)
                                        {
                                            if (colPosition + x < colc && rowPosition + i < rowc)
                                            {
                                                Position itemPosition = new Position(1, rowPosition + i, colPosition - x < 0 ? 0 : colPosition - x, rc, cc - x);
                                                this.itemSourceCache[colPosition + x, rowPosition + i] = item;
                                                this.itemMatrixCache[colPosition + x, rowPosition + i] = itemPosition;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    break;
                }

            this.matrix = t.ToArray();
            var rt = t.Last();
            int xx = rt.Item1.Column;
            int yy = rt.Item1.Row;
            //ручное обнавление вне кеша
            ((List<GridDataItem>)this.DataItems)[((yy - this.displayedRowScrollOffset) * this.displayedCols) + (xx - this.displayedColScrollOffset)].Set(rt.Item2,
                (yy - this.displayedRowScrollOffset) + this.RowHeaderCount, (xx - this.displayedColScrollOffset) + this.ColumnHeaderCount,
                this.itemMatrixCache[xx, yy].RowSpan, this.itemMatrixCache[xx, yy].ColumnSpan, new Thickness(), false);

            for (int i = 1; i < rt.Item1.ColumnSpan; i++)
            {
                object content = 1;
                xx = xx + 1;
                ((List<GridDataItem>)this.DataItems)[((yy - this.displayedRowScrollOffset) * this.displayedCols) + (xx - this.displayedColScrollOffset)].Set(content,
                (yy - this.displayedRowScrollOffset) + this.RowHeaderCount, (xx - this.displayedColScrollOffset) + this.ColumnHeaderCount,
                this.itemMatrixCache[xx, yy].RowSpan, rt.Item1.ColumnSpan - i, new Thickness(), false);
            }

            this.SendPropertyChanged(nameof(RowCountMax));
            this.SendPropertyChanged(nameof(ColumnCountMax));
            this.SendPropertyChanged(nameof(Matrix));
        }

        public void RemoveItem(object item) {

            for (int i = 0; i < this.itemSourceCache.GetLength(0); i++)
                for (int j = 0; j < this.itemSourceCache.GetLength(1); j++) {
                    if (this.itemSourceCache[i, j] == item)
                        RemoveItem(i, j);
                }

            matrix = matrix.Where(x => x.Item2 != item).ToArray();

            this.UpdateDisplayData();

            this.SendPropertyChanged(nameof(RowCountMax));
            this.SendPropertyChanged(nameof(ColumnCountMax));
            this.SendPropertyChanged(nameof(Matrix));
        }

        protected void RemoveItem(int rowIndex, int ColumnIndex) {
            this.itemSourceCache[rowIndex, ColumnIndex] = null;
            this.itemMatrixCache[rowIndex, ColumnIndex] = Position.Default();
        }
        #endregion
        #endregion
    }
}
