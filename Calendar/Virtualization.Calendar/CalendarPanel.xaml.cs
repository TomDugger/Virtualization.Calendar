using Info.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Virtualization.Controls;
using Virtualization.Delegate;
using Virtualization.Models;

namespace Virtualization.Calendar
{
    /// <summary>
    /// Логика взаимодействия для CalendarPanel.xaml
    /// </summary>
    public partial class CalendarPanel : UserControl
    {
        public CalendarPanel()
        {
            InitializeComponent();

            Random rnd = new Random();

            scrollVH.VerticalMetric = (item) =>
            {
                if (item != null)
                {
                    var p = (Tuple<Position, object>)item;
                    return new Tuple<int, int, int, int, Color>(0, p.Item1.Row, 1, p.Item1.RowSpan, new Color());
                }
                else
                    return null;
            };

            scrollVH.HorizontalMetric = (item) =>
            {
                if (item != null)
                {
                    var p = (Tuple<Position, object>)item;
                    return new Tuple<int, int, int, int, Color>(p.Item1.Column, 0, p.Item1.ColumnSpan, 1, new Color());
                }
                else return null;
            };

            scrollVH.HorizontalSelectItem = (item) =>
            {
                scrollVH.ScrollToHorizontalOffset((scrollVH.ExtentWidth - scrollVH.ActualWidth / 2) / scrollVH.ColumnCount * item.Column);
            };

            scrollVH.VerticalSelectItem = (item) =>
            {
                scrollVH.ScrollToVerticalOffset((scrollVH.ExtentHeight - scrollVH.ActualHeight / 2) / scrollVH.RowCount * item.Row);
            };
        }

        #region Dependency property
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(CalendarPanel), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedColumnProperty = DependencyProperty.Register("SelectedRow", typeof(object), typeof(CalendarPanel), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedRowProperty = DependencyProperty.Register("SelectedColumn", typeof(object), typeof(CalendarPanel), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(CalendarPanel), new PropertyMetadata(-1));
        public static readonly DependencyProperty SelectedRowIndexProperty = DependencyProperty.Register("SelectedRowIndex", typeof(int), typeof(CalendarPanel), new PropertyMetadata(-1));
        public static readonly DependencyProperty SelectedColumnIndexProperty = DependencyProperty.Register("SelectedColumnIndex", typeof(int), typeof(CalendarPanel), new PropertyMetadata(-1));

        public static readonly DependencyProperty SelectedDataProperty = DependencyProperty.Register("SelectedData", typeof(object), typeof(CalendarPanel), new PropertyMetadata(null));

        public static readonly DependencyProperty ShowDatePanelProperty = DependencyProperty.Register("ShowDatePanel", typeof(bool), typeof(CalendarPanel), new PropertyMetadata(true));

        private static readonly DependencyProperty RowLineColorProperty = DependencyProperty.Register("RowLineColor", typeof(Color), typeof(CalendarPanel), new PropertyMetadata(Colors.DarkGray));

        private static readonly DependencyProperty ColumnLineColorProperty = DependencyProperty.Register("ColumnLineColor", typeof(Color), typeof(CalendarPanel), new PropertyMetadata(Colors.DarkGray));

        private static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register("DataSource", typeof(IEnumerable), typeof(CalendarPanel), new PropertyMetadata(null));

        public static readonly DependencyProperty StartDateProperty = DependencyProperty.Register("StartDate", typeof(DateTime), typeof(CalendarPanel), new PropertyMetadata(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), new PropertyChangedCallback((d, a) => {
            (d as CalendarPanel).BeginDateTime = (DateTime)a.NewValue;
            (d as CalendarPanel).crC.SendDateChange();
        })));

        public static readonly DependencyProperty EndDateProperty = DependencyProperty.Register("EndDate", typeof(DateTime), typeof(CalendarPanel), new PropertyMetadata(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), new PropertyChangedCallback((d, a) => {
            (d as CalendarPanel).EndDateTime = (DateTime)a.NewValue;

            (d as CalendarPanel).crC.SendDateChange();
        })));

        public static readonly DependencyProperty HorisontalSizeItemsProperty = DependencyProperty.Register("HorisontalSizeItems", typeof(double), typeof(CalendarPanel), new PropertyMetadata(7.0));

        public static readonly DependencyProperty VerticalSizeItemsProperty = DependencyProperty.Register("VerticalSizeItems", typeof(double), typeof(CalendarPanel), new PropertyMetadata(7.0));

        public static readonly DependencyProperty ScrollColorProperty = DependencyProperty.Register("ScrollColor", typeof(Color), typeof(CalendarPanel), new PropertyMetadata(Colors.DodgerBlue));
        
        public static readonly DependencyProperty LeftTopAreaTemplateProperty = DependencyProperty.Register("LeftTopAreaTemplate", typeof(DataTemplate), typeof(CalendarPanel), new PropertyMetadata(null));

        public static readonly DependencyProperty IsHorizontalShowMoreInfoProperty = DependencyProperty.Register("IsHorizontalShowMoreInfo", typeof(bool), typeof(CalendarPanel), new PropertyMetadata(true));

        public static readonly DependencyProperty IsVerticalShowMoreInfoProperty = DependencyProperty.Register("IsVerticalShowMoreInfo", typeof(bool), typeof(CalendarPanel), new PropertyMetadata(true));

        public static readonly DependencyProperty TopMargingProperty = DependencyProperty.Register("TopMarging", typeof(double), typeof(CalendarPanel), new PropertyMetadata(0.0));

        public static readonly DependencyProperty LeftMargingProperty = DependencyProperty.Register("LeftMarging", typeof(double), typeof(CalendarPanel), new PropertyMetadata(0.0));

        public static readonly DependencyProperty SelectedDataInGroupProperty = DependencyProperty.Register("SelectedDataInGroup", typeof(object), typeof(CalendarPanel), new PropertyMetadata(null));

        public static readonly DependencyProperty FooterTemplateProperty = DependencyProperty.Register("FooterTemplate", typeof(DataTemplate), typeof(CalendarPanel), new PropertyMetadata(null));


        public object SelectedItem { get { return this.GetValue(SelectedItemProperty); } set { this.SetValue(SelectedItemProperty, value); } }

        public object SelectedRow { get { return this.GetValue(SelectedRowProperty); } set { this.SetValue(SelectedRowProperty, value); } }

        public object SelectedColumn { get { return this.GetValue(SelectedColumnProperty); } set { this.SetValue(SelectedColumnProperty, value); } }

        public int SelectedIndex { get { return (int)this.GetValue(SelectedIndexProperty); } set { this.SetValue(SelectedIndexProperty, value); } }

        public int SelectedRowIndex { get { return (int)this.GetValue(SelectedRowIndexProperty); } set { this.SetValue(SelectedRowIndexProperty, value); } }

        public int SelectedColumnIndex { get { return (int)this.GetValue(SelectedColumnIndexProperty); } set { this.SetValue(SelectedColumnIndexProperty, value); } }

        public object SelectedData { get { return this.GetValue(SelectedDataProperty); } set { this.SetValue(SelectedDataProperty, value); } }

        public bool ShowDatePanel { get { return (bool)this.GetValue(ShowDatePanelProperty); } set { this.SetValue(ShowDatePanelProperty, value); } }

        public DateTime StartDate {
            get { return (DateTime)GetValue(StartDateProperty); }
            set { SetValue(StartDateProperty, value); }
        }

        public DateTime EndDate {
            get { return (DateTime)GetValue(EndDateProperty); }
            set { SetValue(EndDateProperty, value); }
        }
        
        public Color RowLineColor { get { return (Color)this.GetValue(RowLineColorProperty); } set { this.SetValue(RowLineColorProperty, value); } }

        public Color ColumnLineColor { get { return (Color)this.GetValue(ColumnLineColorProperty); } set { this.SetValue(ColumnLineColorProperty, value); } }

        public double HorisontalSizeItems {
            get { return (double)this.GetValue(HorisontalSizeItemsProperty); }
            set { this.SetValue(HorisontalSizeItemsProperty, value); }
        }

        public double VerticalSizeItems {
            get { return (double)this.GetValue(VerticalSizeItemsProperty); }
            set { this.SetValue(VerticalSizeItemsProperty, value); }
        }

        public Color ScrollColor {
            get { return (Color)this.GetValue(ScrollColorProperty); }
            set { this.SetValue(ScrollColorProperty, value); }
        }

        public DataTemplate LeftTopAreaTemplate {
            get { return (DataTemplate)GetValue(LeftTopAreaTemplateProperty); }
            set { SetValue(LeftTopAreaTemplateProperty, value); }
        }

        public bool IsHorizontalShowMoreInfo {
            get { return (bool)this.GetValue(IsHorizontalShowMoreInfoProperty); }
            set { this.SetValue(IsHorizontalShowMoreInfoProperty, value); }
        }

        public bool IsVerticalShowMoreInfo {
            get { return (bool)this.GetValue(IsVerticalShowMoreInfoProperty); }
            set { this.SetValue(IsVerticalShowMoreInfoProperty, value); }
        }

        public double TopMarging {
            get { return (double)this.GetValue(TopMargingProperty); }
            set { this.SetValue(TopMargingProperty, value); }
        }

        public double LeftMarging {
            get { return (double)this.GetValue(LeftMargingProperty); }
            set { this.SetValue(LeftMargingProperty, value); }
        }

        public object SelectedDataInGroup { get { return (object)this.GetValue(SelectedDataInGroupProperty); } set { this.SetValue(SelectedDataInGroupProperty, value); } }
        
        public DataTemplate FooterTemplate {
            get { return (DataTemplate)this.GetValue(FooterTemplateProperty); }
            set { this.SetValue(FooterTemplateProperty, value); }
        }
        #endregion

        #region Special dependency property
        public static DependencyProperty DataItemTemplateProperty = DependencyProperty.Register("DataItemTemplate",
            typeof(DataTemplate), typeof(CalendarPanel), new PropertyMetadata(null, new PropertyChangedCallback((s, a) => {
                CalendarPanel panel = (CalendarPanel)s;
                if (a.NewValue != null)
                    panel.crC.DataItemTemplate = (DataTemplate)a.NewValue;
            })));

        public static DependencyProperty DataItemTemplateSelectorProperty = DependencyProperty.Register("DataItemTemplateSelector", 
            typeof(DataTemplateSelector), typeof(CalendarPanel), new PropertyMetadata(null, new PropertyChangedCallback((s, a) => {
                CalendarPanel panel = (CalendarPanel)s;
                if (a.NewValue != null)
                    panel.crC.DataItemTemplateSelector = (DataTemplateSelector)a.NewValue;
            })));

        public static DependencyProperty RowHeaderTemplateProperty = DependencyProperty.Register("RowHeaderTemplate",
            typeof(DataTemplate), typeof(CalendarPanel), new PropertyMetadata(null, new PropertyChangedCallback((s, a) => {
                CalendarPanel panel = (CalendarPanel)s;
                if (a.NewValue != null)
                    panel.crC.RowHeaderTemplate = (DataTemplate)a.NewValue;
            })));

        public static DependencyProperty RowHeaderTemplateSelectorProperty = DependencyProperty.Register("RowHeaderTemplateSelector",
            typeof(DataTemplateSelector), typeof(CalendarPanel), new PropertyMetadata(null, new PropertyChangedCallback((s, a) => {
                CalendarPanel panel = (CalendarPanel)s;
                if (a.NewValue != null)
                    panel.crC.RowHeaderTemplateSelector = (DataTemplateSelector)a.NewValue;
            })));

        public static DependencyProperty ColumnHeaderTemplateProperty = DependencyProperty.Register("ColumnHeaderTemplate",
            typeof(DataTemplate), typeof(CalendarPanel), new PropertyMetadata(null, new PropertyChangedCallback((s, a) => {
                CalendarPanel panel = (CalendarPanel)s;
                if (a.NewValue != null)
                    panel.crC.ColumnHeaderTemplate = (DataTemplate)a.NewValue;
            })));

        public static DependencyProperty ColumnHeaderTemplateSelectorProperty = DependencyProperty.Register("ColumnHeaderTemplateSelector",
            typeof(DataTemplateSelector), typeof(CalendarPanel), new PropertyMetadata(null, new PropertyChangedCallback((s, a) => {
                CalendarPanel panel = (CalendarPanel)s;
                if (a.NewValue != null)
                    panel.crC.ColumnHeaderTemplateSelector = (DataTemplateSelector)a.NewValue;
            })));

        public static DependencyProperty WaitLayerTemplateProperty = DependencyProperty.Register("WaitLayerTemplate", 
            typeof(ControlTemplate), typeof(CalendarPanel), new PropertyMetadata(null, new PropertyChangedCallback((s, a) => {
                CalendarPanel panel = (CalendarPanel)s;
                if (a.NewValue != null)
                    panel.crC.WaitLayerTemplate = (ControlTemplate)a.NewValue;
            })));

        public static DependencyProperty ItemWidthProperty = DependencyProperty.Register("ItemWidth", 
            typeof(double), typeof(CalendarPanel), new PropertyMetadata(0.0, new PropertyChangedCallback((s, a) => {
                CalendarPanel panel = (CalendarPanel)s;
                if ((double)a.NewValue != 0.0)
                    panel.crC.ItemWidth = (double)a.NewValue;
            })));

        public static DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight",
            typeof(double), typeof(CalendarPanel), new PropertyMetadata(0.0, new PropertyChangedCallback((s, a) => {
                CalendarPanel panel = (CalendarPanel)s;
                if ((double)a.NewValue != 0.0)
                    panel.crC.ItemHeight = (double)a.NewValue;
            })));

        public static DependencyProperty GroupWidthProperty = DependencyProperty.Register("GroupWidth",
            typeof(double), typeof(CalendarPanel), new PropertyMetadata(0.0, new PropertyChangedCallback((s, a) => {
                CalendarPanel panel = (CalendarPanel)s;
                if ((double)a.NewValue != 0.0)
                    panel.crC.GroupWidth = (double)a.NewValue;
            })));

        public static DependencyProperty GroupHeightProperty = DependencyProperty.Register("GroupHeight",
            typeof(double), typeof(CalendarPanel), new PropertyMetadata(0.0, new PropertyChangedCallback((s, a) => {
                CalendarPanel panel = (CalendarPanel)s;
                if ((double)a.NewValue != 0.0)
                    panel.crC.GroupHeight = (double)a.NewValue;
            })));

        public DataTemplate DataItemTemplate {
            get { return (DataTemplate)this.GetValue(DataItemTemplateProperty); }
            set { this.SetValue(DataItemTemplateProperty, value); }
        }

        public DataTemplateSelector DataItemTemplateSelector {
            get { return (DataTemplateSelector)this.GetValue(DataItemTemplateSelectorProperty); }
            set { this.SetValue(DataItemTemplateSelectorProperty, value); }
        }

        public DataTemplate RowHeaderTemplate {
            get { return (DataTemplate)this.GetValue(RowHeaderTemplateProperty); }
            set { this.SetValue(RowHeaderTemplateProperty, value); }
        }

        public DataTemplateSelector RowHeaderTemplateSelector {
            get { return (DataTemplateSelector)this.GetValue(RowHeaderTemplateSelectorProperty); }
            set { this.SetValue(RowHeaderTemplateSelectorProperty, value); }
        }

        public DataTemplate ColumnHeaderTemplate {
            get { return (DataTemplate)this.GetValue(ColumnHeaderTemplateProperty); }
            set { this.SetValue(ColumnHeaderTemplateProperty, value); }
        }

        public DataTemplateSelector ColumnHeaderTemplateSelector {
            get { return (DataTemplateSelector)this.GetValue(ColumnHeaderTemplateSelectorProperty); }
            set { this.SetValue(ColumnHeaderTemplateSelectorProperty, value); }
        }

        public ControlTemplate WaitLayerTemplate {
            get { return (ControlTemplate)this.GetValue(WaitLayerTemplateProperty); }
            set { this.SetValue(WaitLayerTemplateProperty, value); }
        }

        public double ItemWidth {
            get { return (double)this.GetValue(ItemWidthProperty); }
            set { this.SetValue(ItemWidthProperty, value); }
        }

        public double ItemHeight {
            get { return (double)this.GetValue(ItemHeightProperty); }
            set { this.SetValue(ItemHeightProperty, value); }
        }

        public double GroupWidth {
            get { return (double)this.GetValue(GroupWidthProperty); }
            set { this.SetValue(GroupWidthProperty, value); }
        }

        public double GroupHeight {
            get { return (double)this.GetValue(GroupHeightProperty); }
            set { this.SetValue(GroupHeightProperty, value); }
        }
        #endregion

        #region Events
        public event SelectionValueEventHandler SelectionChanged {
            add { crC.AddHandler(GridControl.SelectionChangedEvent, value); }
            remove { crC.RemoveHandler(GridControl.SelectionChangedEvent, value); }
        }

        public event SelectionValueEventHandler SelectionDoubleClickChanged {
            add { crC.AddHandler(GridControl.SelectionDoubleClickChangedEvent, value); }
            remove { crC.RemoveHandler(GridControl.SelectionDoubleClickChangedEvent, value); }
        }

        public event SelectionValueEventHandler SelectionRighctClickChanged {
            add { crC.AddHandler(GridControl.SelectionRighctClickChangedEvent, value); }
            remove { crC.RemoveHandler(GridControl.SelectionRighctClickChangedEvent, value); }
        }

        public event SelectionValueEventHandler SelectionMiddleClickChanged {
            add { crC.AddHandler(GridControl.SelectionMiddleClickChangedEvent, value); }
            remove { crC.RemoveHandler(GridControl.SelectionMiddleClickChangedEvent, value); }
        }
        #endregion

        #region Property
        public IEnumerable DataSource { get { return (IEnumerable)this.GetValue(DataSourceProperty); } set { this.SetValue(DataSourceProperty, value); } }

        private DateTime _beginDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime BeginDateTime {
            get { return _beginDateTime; }
            private set { _beginDateTime = value; }
        }

        private DateTime _endDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
        public DateTime EndDateTime {
            get { return _endDateTime; }
            private set { _endDateTime = value; }
        }
        #endregion

        #region Helps
        public void SetParametry(Func<object, IEnumerable<object>> rowSelecter = null,
                                 Func<object, IEnumerable<object>> colSelector = null,
                                 Func<IEnumerable<object>> rowsSelector = null, Func<IEnumerable<object>> columnsSelector = null,
                                 Func<object, int, Position> rowPostion = null, Func<object, int, Position> columnPosition = null,
                                 Func<object, object>[] groupRowSelector = null, Func<object, object>[] groupColumnSelector = null,
                                 Func<object, object, int> itemRowLeft = null, Func<object, object, object, int> itemRowRight = null,
                                 Func<object, object, int> itemColLeft = null, Func<object, object, object, int> itemColRight = null,
                                 Func<object, Tuple<int, int, int, int, Color>> verticalMetric = null,
                                 Func<object, Tuple<int, int, int, int, Color>> horizontalMetric = null,
                                 Action<InfoItem> horizontalGrouping = null,
                                 Action<InfoItem> verticalGrouping = null,

                                 Func<IEnumerable<object>, object> stateSelectGroup = null

            )
        {
            crC.SetParametry(rowSelecter,
                             colSelector,
                             rowsSelector,
                             columnsSelector,
                             rowPostion,
                             columnPosition,
                             groupRowSelector,
                             groupColumnSelector,
                             itemRowLeft,
                             itemRowRight,
                             itemColLeft,
                             itemColRight,
                             stateSelectGroup);

            if (horizontalMetric != null)
                scrollVH.HorizontalMetric = horizontalMetric;
            if (verticalMetric != null)
                scrollVH.VerticalMetric = verticalMetric;

            scrollVH.HorizontalGrouping = horizontalGrouping;
            scrollVH.VerticalGrouping = verticalGrouping;
        }

        public void SetEventHandlerDateChange(Action<object, EventArgs> arg)
        {
            crC.SelectedDateChanged += new EventHandler(arg);
        }

        public void SetSelectedItemOnScrollViewInfo(Action<InfoItem> horizontalSelect = null, Action<InfoItem> verticalSelect = null) {
            scrollVH.HorizontalSelectItem = horizontalSelect;
            scrollVH.VerticalSelectItem = verticalSelect;
        }

        public void Add(object item) {
            crC.AddItem(item);
        }

        public void Remove(object item) {
            crC.RemoveItem(item);
        }
        #endregion
    }
}
