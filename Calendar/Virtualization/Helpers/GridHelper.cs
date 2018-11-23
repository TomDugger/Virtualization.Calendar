using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Virtualization.Helpers
{
    public class GridHelper
    {
        #region Dependency property
        public static readonly DependencyProperty RowCountProperty = DependencyProperty.RegisterAttached("RowCount", typeof(int), typeof(GridHelper), new PropertyMetadata(-1, RowCountChangedHandler));

        public static readonly DependencyProperty ColumnCountProperty = DependencyProperty.RegisterAttached("ColumnCount", typeof(int), typeof(GridHelper), new PropertyMetadata(-1, ColumnCountChangedHandler));


        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.RegisterAttached("ItemHeight", typeof(double), typeof(GridHelper), new PropertyMetadata(30.0));

        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.RegisterAttached("ItemWidth", typeof(double), typeof(GridHelper), new PropertyMetadata(100.0));


        public static readonly DependencyProperty RowHeaderCountProperty = DependencyProperty.RegisterAttached("RowHeaderCount", typeof(int), typeof(GridHelper), new PropertyMetadata(1));

        public static readonly DependencyProperty ColumnHeaderCountProperty = DependencyProperty.RegisterAttached("ColumnHeaderCount", typeof(int), typeof(GridHelper), new PropertyMetadata(1));


        public static readonly DependencyProperty GroupHeightProperty = DependencyProperty.RegisterAttached("GroupHeight", typeof(double), typeof(GridHelper), new PropertyMetadata(30.0));

        public static readonly DependencyProperty GroupWidthProperty = DependencyProperty.RegisterAttached("GroupWidth", typeof(double), typeof(GridHelper), new PropertyMetadata(100.0));
        #endregion

        #region Accessors
        public static int GetRowCount(DependencyObject obj)
        {
            return (int)obj.GetValue(RowCountProperty);
        }

        public static void SetRowCount(DependencyObject obj, int value)
        {
            obj.SetValue(RowCountProperty, value);
        }

        public static int GetColumnCount(DependencyObject obj)
        {
            return (int)obj.GetValue(ColumnCountProperty);
        }

        public static void SetColumnCount(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnCountProperty, value);
        }


        public static double GetItemHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(ItemHeightProperty);
        }

        public static void SetItemHeight(DependencyObject obj, double value)
        {
            obj.SetValue(ItemHeightProperty, value);
        }

        public static double GetItemWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(ItemWidthProperty);
        }

        public static void SetItemWidth(DependencyObject obj, double value)
        {
            obj.SetValue(ItemWidthProperty, value);
        }


        public static int GetRowHeaderCount(DependencyObject obj) {
            return (int)obj.GetValue(RowHeaderCountProperty);
        }

        public static void SetRowHeaderCount(DependencyObject obj, int value) {
            obj.SetValue(RowHeaderCountProperty, value);
        }

        public static int GetColumnHeaderCount(DependencyObject obj) {
            return (int)obj.GetValue(ColumnHeaderCountProperty);
        }

        public static void SetColumnHeaderCount(DependencyObject obj, int value) {
            obj.SetValue(ColumnHeaderCountProperty, value);
        }


        public static double GetGroupHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(GroupHeightProperty);
        }

        public static void SetGroupHeight(DependencyObject obj, double value)
        {
            obj.SetValue(GroupHeightProperty, value);
        }

        public static double GetGroupWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(GroupWidthProperty);
        }

        public static void SetGroupWidth(DependencyObject obj, double value)
        {
            obj.SetValue(GroupWidthProperty, value);
        }
        #endregion

        #region Property changed helpers
        public static void RowCountChangedHandler(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var grid = (System.Windows.Controls.Grid)obj;
            double height = GridHelper.GetItemHeight(grid);
            int c = GridHelper.GetRowCount(grid);
            int ch = GridHelper.GetRowHeaderCount(grid);
            double gheight = GridHelper.GetGroupHeight(grid);

            if (!(obj is System.Windows.Controls.Grid) || c < 0 || ch < 0)
                return;

            grid.RowDefinitions.Clear();

            for (int i = 0; i < ch; i++)
                grid.RowDefinitions.Add(new RowDefinition { MinHeight = gheight, Height = new GridLength(1, GridUnitType.Auto), SharedSizeGroup = "headerRow" + i });

            for (int i = ch; i < c; i++)
                grid.RowDefinitions.Add(new RowDefinition { MinHeight = height, Height = new GridLength(1, GridUnitType.Star), SharedSizeGroup = "mainRow" + i });
        }

        public static void ColumnCountChangedHandler(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var grid = (System.Windows.Controls.Grid)obj;
            double width = GridHelper.GetItemWidth(grid);
            int c = GridHelper.GetColumnCount(grid);
            int ch = GridHelper.GetColumnHeaderCount(grid);
            double gwidth = GridHelper.GetGroupWidth(grid);

            if (!(obj is System.Windows.Controls.Grid) || c < 0 || ch < 0)
                return;

            grid.ColumnDefinitions.Clear();
            for (int i = 0; i < ch; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition { MinWidth = gwidth,  Width = new GridLength(1, GridUnitType.Auto), SharedSizeGroup = "headerColumn" + i });

            for (int i = ch; i < c; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition { MinWidth = width, Width = new GridLength(1, GridUnitType.Star), SharedSizeGroup = "mainColumn" + i });
        }
        #endregion
    }
}
