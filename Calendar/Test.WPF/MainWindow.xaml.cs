using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Virtualization.Models;

namespace Test.WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Color[] crc = new Color[] { Colors.Maroon, Colors.Green, Colors.DimGray };

        public Group[] groups;

        public MainWindow()
        {
            InitializeComponent();

            var random = new Random();

            groups = Enumerable.Range(2, 10).Select(x => new Group { Name = string.Format("Group {0}", x), EntryCount = random.Next(5, 8), GroupName = "Lv " }).ToArray();

            List<SampleGridItem> dataItems = new List<SampleGridItem>();

            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(random.Next(0, 27));
                    dataItems.Add(new SampleGridItem()
                    {
                        StartDate = date.AddDays(i).Date.AddHours(random.Next(0, 24)),
                        EndDate = date.AddDays(i).Date.AddDays(random.Next(1, 5)).AddHours(random.Next(0, 24)),
                        ProductName = string.Format("item {0} {1}", i, j),
                        Position = 0,
                        Group = groups[random.Next(0, 10)],

                        State = random.Next(0, 3)
                    });
                }
            }

            //groups[0].EntryCount = 3;
            //groups[0].GroupName = "Lv 1";

            //var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //dataItems.Add(new SampleGridItem()
            //{
            //    StartDate = new DateTime(2018, 9, 1).AddHours(15),
            //    EndDate = new DateTime(2018, 9, 3).AddHours(0),
            //    ProductName = string.Format("item 1"),
            //    Position = 0,
            //    Group = groups[0],

            //    State = 0
            //});
            //dataItems.Add(new SampleGridItem()
            //{
            //    StartDate = new DateTime(2018, 9, 2).AddHours(23),
            //    EndDate = new DateTime(2018, 9, 6).AddHours(0),
            //    ProductName = string.Format("item 2"),
            //    Position = 0,
            //    Group = groups[0],

            //    State = 1
            //});
            //dataItems.Add(new SampleGridItem()
            //{
            //    StartDate = new DateTime(2018, 9, 4).AddHours(0),
            //    EndDate = new DateTime(2018, 9, 5).AddHours(6),
            //    ProductName = string.Format("item 3"),
            //    Position = 0,
            //    Group = groups[0],

            //    State = 2
            //});

            //dataItems.Add(new SampleGridItem()
            //{
            //    StartDate = date.AddDays(1).AddHours(12),
            //    EndDate = date.AddDays(2).AddHours(20),
            //    ProductName = string.Format("item 4"),
            //    Position = 0,
            //    Group = groups[1],

            //    State = 1
            //});

            //dataItems.Add(new SampleGridItem()
            //{
            //    StartDate = date.AddDays(0),
            //    EndDate = date.AddDays(random.Next(1, 5)),
            //    ProductName = string.Format("item 3"),
            //    Position = 0,
            //    Group = groups[0]
            //});

            crC.DataSource = dataItems.Where(x => !(x.StartDate > crC.EndDate || x.EndDate < crC.StartDate)).OrderBy(x => x.StartDate).ToArray();
            //dataItems.Clear();
            dataItems = null;

            crC.SetParametry(
                item => new object[] { ((SampleGridItem)item).Group },
                item => Enumerable.Range(0, 1 + ((SampleGridItem)item).EndDate.Date.Subtract(((SampleGridItem)item).StartDate.Date).Days).Select(x => (object)((SampleGridItem)item).StartDate.Date.AddDays(x)),
                () => groups,
                () => Enumerable.Range(0, 1 + crC.EndDateTime.Subtract(crC.BeginDateTime).Days).Select(x => (object)crC.BeginDateTime.Date.AddDays(x)).ToArray(),
                //() => Enumerable.Range(0, 5).Select(x => (object)crC.StartDate.Date.AddDays(x)),
                (row, index) => new Virtualization.Models.Position(index, 0, 0, rowSpan: ((Group)row).EntryCount), //((Group)row).EntryCount
                (col, index) => new Virtualization.Models.Position(index, 0, 0, colSpan: 4),
                new Func<object, object>[] { (row) => ((Group)row).GroupName },
                new Func<object, object>[] {
                    //(col) => ((DateTime)col).AddDays(-((DateTime)col).Day + 1).AddMonths(-((DateTime)col).Month + 1),
                    (col) => ((DateTime)col).Year,
                    //(col) => ((DateTime)col).AddDays(-((DateTime)col).Day + 1) },
                    (col) => ((DateTime)col).Month },
                null, null,
                (item, col) => ((SampleGridItem)item).StartDate.Date != ((DateTime)col).Date ? 0 : (((SampleGridItem)item).StartDate.Hour < 6 ? 0 : (((SampleGridItem)item).StartDate.Hour < 12 ? 1 : (((SampleGridItem)item).StartDate.Hour < 18 ? 2 : 3))),
                (item, col, colLast) => (((SampleGridItem)item).StartDate.Date != ((DateTime)col).Date ? 0 : (((SampleGridItem)item).StartDate.Hour < 6 ? 0 : (((SampleGridItem)item).StartDate.Hour < 12 ? 1 : (((SampleGridItem)item).StartDate.Hour < 18 ? 2 : 3))))
                + (((SampleGridItem)item).EndDate.Date > ((DateTime)colLast).Date ? 0 : ((((SampleGridItem)item).EndDate.Hour >= 18 ? 0 : (((SampleGridItem)item).EndDate.Hour >= 12 ? 1 : (((SampleGridItem)item).EndDate.Hour >= 6 ? 2 : 3))))),
                (item) =>
                    {
                        if (item != null)
                        {
                            var p = (Tuple<Position, object>)item;
                            return new Tuple<int, int, int, int, Color>(0, p.Item1.Row, 1, p.Item1.RowSpan, crc[((SampleGridItem)p.Item2).State]);
                        }
                        else
                            return null;
                    },
                (item) =>
                {
                    if (item != null)
                    {
                        var p = (Tuple<Position, object>)item;
                        return new Tuple<int, int, int, int, Color>(p.Item1.Column, 0, p.Item1.ColumnSpan, 1, crc[((SampleGridItem)p.Item2).State]);
                    }
                    else return null;
                }, 
                (info) => { info.Row = crc.ToList().IndexOf((Color)info.Color); }, 
                (info) => { info.Column = crc.ToList().IndexOf((Color)info.Color); },

                (sItems) => {
                    string state = "";
                    // считаем по группам
                    var recan = sItems.Select(x => (SampleGridItem)x);
                    state = string.Format("0 = {0}, 1 = {1}, 2 = {2}", recan.Count(x => x.State == 0), recan.Count(x => x.State == 1), recan.Count(x => x.State == 2));
                    return state;
                    ; }
                );

            crC.SetEventHandlerDateChange((sender, arg) => {
                //List<SampleGridItem> dataItemss = new List<SampleGridItem>();

                //Task.Factory.StartNew(() =>
                //{
                //    for (int i = 0; i < 100; i++)
                //    {
                //        for (int j = 0; j < 10; j++)
                //        {
                //            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(random.Next(0, 27));
                //            dataItemss.Add(new SampleGridItem()
                //            {
                //                StartDate = date.AddDays(i).Date.AddHours(random.Next(0, 24)),
                //                EndDate = date.AddDays(i).Date.AddDays(random.Next(1, 5)).AddHours(random.Next(0, 24)),
                //                ProductName = string.Format("item {0} {1}", i, j),
                //                Position = 0,
                //                Group = groups[random.Next(0, 10)],

                //                State = random.Next(0, 3)
                //            });
                //        }
                //    }

                //}).ContinueWith((task) =>
                //{
                //    crC.DataSource = dataItemss.Where(x => !(x.StartDate > crC.EndDate || x.EndDate < crC.StartDate)).OrderBy(x => x.StartDate).ToArray();
                //    dataItemss = null;
                //}, TaskScheduler.FromCurrentSynchronizationContext());
            });

            //crC.SetSelectedItemOnScrollViewInfo((info) => { MessageBox.Show("Select horizontal item [" + info.Column + ", " + info.ColumnSpan + "]"); },
            //    (info) => { MessageBox.Show("Select vertical item [" + info.Row + ", " + info.RowSpan + "]"); });
        }

        public class SampleGridItem
        {
            public string ProductName { get; set; }

            public Group Group { get; set; }

            public DateTime StartDate { get; set; }

            public DateTime EndDate { get; set; }

            public int Position { get; set; }

            public int State { get; set; }

            public Color StateColor {
                get { return MainWindow.crc[this.State]; }
            }

            public override string ToString()
            {
                return string.Format("{0}, {1}, {2}, {3}, State = {4}", ProductName, Group, StartDate, EndDate, State);
            }
        }

        public class Group
        {
            public string Name { get; set; }

            public int EntryCount { get; set; }

            public string GroupName { get; set; }

            public override string ToString()
            {
                return string.Format("{0}, {1}, {2}", Name, EntryCount, GroupName);
            }
        }

        private void crC_SelectionChanged(object sender, Virtualization.Delegate.SelectionValueEventArgs args)
        {
            // MessageBox.Show(string.Format("{0}, {1}, {2}, {3}", args.SelectedColumn, args.SelectedRow, args.SelectedValue, args.IsGroupSelection ? args.ItemsInGroup.Count() : 0));
        }

        private void crC_SelectionDoubleClickChangedEvent(object sender, Virtualization.Delegate.SelectionValueEventArgs args)
        {
            if (args.SelectedValue == null) {
                var aw = new AddWindow();
                aw.PrName.Text = "Prod";

                aw.Groups.ItemsSource = groups;
                aw.Groups.SelectedItem = args.SelectedRow.Content;

                aw.SDate.SelectedDate = (DateTime)args.SelectedColumn.Content;
                aw.EDate.SelectedDate = ((DateTime)aw.SDate.SelectedDate).AddDays(1);

                aw.State.ItemsSource = crc;
                aw.State.SelectedIndex = 0;

                switch (aw.ShowDialog()) {
                    case true:
                        var item = new SampleGridItem()
                        {
                            StartDate = (DateTime)aw.SDate.SelectedDate,
                            EndDate = (DateTime)aw.EDate.SelectedDate,
                            ProductName = aw.PrName.Text,
                            Position = 0,
                            Group = (Group)aw.Groups.SelectedItem,

                            State = aw.State.SelectedIndex
                        };

                        crC.Add(item);
                        break;
                }
            }
        }

        private void crC_SelectionRighctClickChangedEvent(object sender, Virtualization.Delegate.SelectionValueEventArgs args)
        {
            if (args.SelectedValue == null) {
                Console.WriteLine(args.LastValue);
                Console.WriteLine(args.CurrentValue);
                Console.WriteLine(crC.Select(args.LastValue, args.CurrentValue));
            }
                //MessageBox.Show("RightClick -> " + args.SelectedValue + " [" + args.SelectedRow.Content + " |" + args.SelectedRow.Step + "|" + ", " + args.SelectedColumn.Content + " |" + args.SelectedColumn.Step + "|" + "] ");
        }

        private void crC_SelectionMiddleClickChanged(object sender, Virtualization.Delegate.SelectionValueEventArgs args)
        {
            if (args.SelectedValue == null)
                MessageBox.Show("MiddleClick -> " + args.SelectedValue + " [" + args.SelectedRow.Content + " |" + args.SelectedRow.Step + "|" + ", " + args.SelectedColumn.Content + " |" + args.SelectedColumn.Step + "|" + "] ");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            int step = random.Next(0, 5);
            var item = new SampleGridItem()
            {
                StartDate = new DateTime(2018, 9, 2).AddDays(step),
                EndDate = new DateTime(2018, 9, 6).AddDays(step + 2),
                ProductName = string.Format("item 2"),
                Position = 0,
                Group = groups[0],

                State = random.Next(0, 3)
            };

            crC.Add(item);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (crC.SelectedData != null)
                crC.Remove(crC.SelectedData);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (crC.SelectedValues != null)
                MessageBox.Show(crC.SelectedValues.ToArray()[0].Column.Content.ToString() + " " + crC.SelectedValues.ToArray()[0].Row.Content.ToString() + " " + crC.SelectedValues.ToArray()[0].Column.Step.ToString() + " " + crC.SelectedValues.ToArray()[0].Row.Step.ToString());
        }
    }
}
