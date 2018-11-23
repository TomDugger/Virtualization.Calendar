using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Virtualization.Controls;
using Virtualization.Models;

namespace Virtualization.Calendar
{
    public class CalendarControl:GridControl
    {
        public event EventHandler SelectedDateChanged;

        #region Dependency property


        public DateTime StartDate {
            get { return (DateTime)GetValue(StartDateProperty); }
            set { SetValue(StartDateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartDateProperty =
            DependencyProperty.Register("StartDate", typeof(DateTime), typeof(CalendarControl), new PropertyMetadata(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), 
                new PropertyChangedCallback((d, a) => { (d as CalendarControl).Refresh(); })));



        public DateTime EndDate {
            get { return (DateTime)GetValue(EndDateProperty); }
            set { SetValue(EndDateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndDateProperty =
            DependencyProperty.Register("EndDate", typeof(DateTime), typeof(CalendarControl), new PropertyMetadata(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), 
                new PropertyChangedCallback((d, a) => { (d as CalendarControl).Refresh(); })));



        public DataTemplate LeftTopAreaTemplate {
            get { return (DataTemplate)GetValue(LeftTopAreaTemplateProperty); }
            set { SetValue(LeftTopAreaTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftTopAreaTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftTopAreaTemplateProperty =
            DependencyProperty.Register("LeftTopAreaTemplate", typeof(DataTemplate), typeof(CalendarControl), new PropertyMetadata(null));


        #endregion

        static CalendarControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarControl), new FrameworkPropertyMetadata(typeof(CalendarControl)));
        }

        public CalendarControl() : base() { }

        #region Helps
        public void SetParametry(Func<object, IEnumerable<object>> rowSelecter = null, 
                                 Func<object, IEnumerable<object>> colSelector = null,
                                 Func<IEnumerable<object>> rowsSelector = null, Func<IEnumerable<object>> columnsSelector = null,
                                 Func<object, int, Position> rowPostion = null, Func<object, int, Position> columnPosition = null,
                                 Func<object, object>[] groupRowSelector = null, Func<object, object>[] groupColumnSelector = null,
                                 Func<object, object, int> itemRowLeft = null, Func<object, object, object, int> itemRowRight = null,
                                 Func<object, object, int> itemColLeft = null, Func<object, object, object, int> itemColRight = null,
                                 
                                 Func<IEnumerable<object>, object> stateChange = null) {

            this.rowSelecter = rowSelecter;
            this.colSelector = colSelector;

            this.rowsSelector = rowsSelector;
            this.columnsSelector = columnsSelector;

            this.rowPostion = rowPostion;
            this.columnPosition = columnPosition;

            this.groupRowSelector = groupRowSelector;
            this.groupColSelector = groupColumnSelector;

            this.itemRowLeft = itemRowLeft;
            this.itemRowRight = itemRowRight;

            this.itemColLeft = itemColLeft;
            this.itemColRight = itemColRight;

            this.stateChange = stateChange;

            Refresh();
        }

        public void Refresh() {
            base.DataSourceChanged();
        }

        public void SendDateChange()
        {
            SelectedDateChanged?.Invoke(this, new EventArgs());
        }
        #endregion
    }
}
