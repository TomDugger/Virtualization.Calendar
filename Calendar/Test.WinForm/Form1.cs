using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Test.WinForm
{
    public partial class Form1 : Form
    {
        List<SampleGridItem> dataItems = new List<SampleGridItem>();

        public Form1()
        {
            InitializeComponent();

            var random = new Random();

            var groups = Enumerable.Range(2, 10).Select(x => new Group { Name = string.Format("Group {0}", x), EntryCount = random.Next(5, 8), GroupName = "Lv " + random.Next(1, 3) }).ToArray();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(random.Next(0, 27));
                    dataItems.Add(new SampleGridItem()
                    {
                        StartDate = date.AddDays(i).Date.AddHours(random.Next(0, 24)),
                        EndDate = date.AddDays(i).Date.AddDays(random.Next(1, 5)).AddHours(random.Next(0, 24)),
                        ProductName = string.Format("item {0} {1}", i, j),
                        Position = 0,
                        Group = groups[random.Next(0, 10)]
                    });
                }
            }

            //groups[0].EntryCount = 3;
            //groups[0].GroupName = "Lv 1";

            //var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //dataItems.Add(new SampleGridItem()
            //{
            //    StartDate = new DateTime(2018, 8, 5).AddHours(23),
            //    EndDate = new DateTime(2018, 8, 6).AddHours(0),
            //    ProductName = string.Format("item 1"),
            //    Position = 0,
            //    Group = groups[0]
            //});

            //dataItems.Add(new SampleGridItem()
            //{
            //    StartDate = date.AddDays(1).AddHours(12),
            //    EndDate = date.AddDays(2).AddHours(20),
            //    ProductName = string.Format("item 2"),
            //    Position = 0,
            //    Group = groups[0]
            //});

            //dataItems.Add(new SampleGridItem()
            //{
            //    StartDate = date.AddDays(0),
            //    EndDate = date.AddDays(random.Next(1, 5)),
            //    ProductName = string.Format("item 3"),
            //    Position = 0,
            //    Group = groups[0]
            //});

            crC.DataSource= dataItems.Where(x => !(x.StartDate > crC.EndDate || x.EndDate < crC.StartDate)).OrderBy(x => x.StartDate).ToArray();

            crC.SetParametry(item => new object[] { ((SampleGridItem)item).Group },
                item => Enumerable.Range(0, 1 + ((SampleGridItem)item).EndDate.Date.Subtract(((SampleGridItem)item).StartDate.Date).Days).Select(x => (object)((SampleGridItem)item).StartDate.Date.AddDays(x)),
                () => groups,
                () => Enumerable.Range(0, 1 + crC.EndDate.Subtract(crC.StartDate).Days).Select(x => (object)crC.StartDate.Date.AddDays(x)),
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
                (item) => ((SampleGridItem)item).StartDate.Hour < 6 ? 0 : (((SampleGridItem)item).StartDate.Hour < 12 ? 1 : (((SampleGridItem)item).StartDate.Hour < 18 ? 2 : 3)),
                (item) => ((SampleGridItem)item).StartDate.Hour < 6 ? 0 : (((SampleGridItem)item).StartDate.Hour < 12 ? 1 : (((SampleGridItem)item).StartDate.Hour < 18 ? 2 : 3))
                + (((SampleGridItem)item).EndDate.Hour >= 18 ? 0 : (((SampleGridItem)item).EndDate.Hour >= 12 ? 1 : (((SampleGridItem)item).EndDate.Hour >= 6 ? 2 : 3)))
                );

            crC.SetEventHandlerDateChange((sender, arg) => {
                crC.DataSource = dataItems.Where(x => !(x.StartDate > crC.EndDate || x.EndDate < crC.StartDate)).OrderBy(x => x.StartDate).ToArray();
            });
        }

        public class SampleGridItem
        {
            public string ProductName { get; set; }

            public Group Group { get; set; }

            public DateTime StartDate { get; set; }

            public DateTime EndDate { get; set; }

            public int Position { get; set; }

            public override string ToString()
            {
                return string.Format("{0}, {1}, {2}, {3}", ProductName, Group, StartDate, EndDate);
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
    }
}
