using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Virtualization.Calendar
{
    /// <summary>
    /// Логика взаимодействия для DateLinePanel.xaml
    /// </summary>
    public partial class DateLinePanel : UserControl, INotifyPropertyChanged
    {
        public event EventHandler SelectedDateChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public DateLinePanel()
        {
            InitializeComponent();
        }

        #region Dependency property
        public static readonly DependencyProperty BeginDateProperty =
            DependencyProperty.Register("BeginDate", typeof(DateTime),
            typeof(DateLinePanel),
            new UIPropertyMetadata(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), new PropertyChangedCallback(DateValueChanged)));

        private static void DateValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs A)
        {
            DateTime sdt = (DateTime)d.GetValue(DateLinePanel.BeginDateProperty);
            DateTime edt = (DateTime)d.GetValue(DateLinePanel.EndDateProperty);
            if (sdt > edt)
                d.SetValue(DateLinePanel.EndDateProperty, sdt);

            ((DateLinePanel)d).SendPropertyChaged(nameof(MaxDate));
            ((DateLinePanel)d).SendPropertyChaged(nameof(MinDate));

            (d as DateLinePanel).SelectedDateChanged?.Invoke(d, new EventArgs());
        }

        public static readonly DependencyProperty EndDateProperty =
            DependencyProperty.Register("EndDate", typeof(DateTime),
            typeof(DateLinePanel),
            new UIPropertyMetadata(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), new PropertyChangedCallback(DateValueChanged)));


        #endregion

        #region Property
        public DateTime BeginDate {
            get { return (DateTime)this.GetValue(BeginDateProperty); }
            set { this.SetValue(BeginDateProperty, value); }
        }

        public DateTime EndDate {
            get { return (DateTime)this.GetValue(EndDateProperty); }
            set { this.SetValue(EndDateProperty, value); }
        }

        public DateTime MaxDate {
            get { return BeginDate.AddMonths(3).AddDays(-1); }
        }

        public DateTime MinDate {
            get { return BeginDate.AddMonths(-3).AddDays(-1); }
        }

        private bool _isWeekPeriod = false;
        public bool IsWeekPeriod {
            get { return _isWeekPeriod; }
            set {
                if (!_isWeekPeriod)
                {
                    _isWeekPeriod = value;
                    FifteenPeriod = false;
                }
                this.SendPropertyChaged(nameof(IsWeekPeriod));
                this.SendPropertyChaged(nameof(IsMonthPeriod));

                this.SendPropertyChaged(nameof(Year));
                this.SendPropertyChaged(nameof(Month));
            }
        }

        public bool IsMonthPeriod {
            get { return !_isWeekPeriod; }
            set {
                if (_isWeekPeriod)
                {
                    _isWeekPeriod = !value;
                    Month = BeginDate.Month;
                }

                this.SendPropertyChaged(nameof(IsWeekPeriod));
                this.SendPropertyChaged(nameof(IsMonthPeriod));

                this.SendPropertyChaged(nameof(Year));
                this.SendPropertyChaged(nameof(Month));
            }
        }

        private bool _fifteenPeriod = false;
        public bool FifteenPeriod {
            get { return _fifteenPeriod; }
            set {
                _fifteenPeriod = value;
                if (value)
                {
                    FifteenDaysRang = 1;
                }
                else
                {
                    Month = Month;
                }
                this.SendPropertyChaged(nameof(FifteenPeriod));
            }
        }

        public IEnumerable<int> Years {
            get {
                var result = new int[100];
                for (int i = 0; i < 100; i++)
                    result[i] = DateTime.Now.Year - 50 + i;
                return result;
            }
        }

        public IEnumerable<Tuple<int, string>> Months {
            get {
                var result = new Tuple<int, string>[] {
                    new Tuple<int, string>(1, "Январь"),
                    new Tuple<int, string>(2, "Февраль"),
                    new Tuple<int, string>(3, "Март"),
                    new Tuple<int, string>(4, "Апрель"),
                    new Tuple<int, string>(5, "Май"),
                    new Tuple<int, string>(6, "Июнь"),
                    new Tuple<int, string>(7, "Июль"),
                    new Tuple<int, string>(8, "Август"),
                    new Tuple<int, string>(9, "Сентябрь"),
                    new Tuple<int, string>(10, "Октябрь"),
                    new Tuple<int, string>(11, "Ноябрь"),
                    new Tuple<int, string>(12, "Декабрь"),
                };
                return result;
            }
        }

        public IEnumerable<Tuple<int, string>> FifteenDaysList {
            get {
                return new Tuple<int, string>[] {
                new Tuple<int, string>(1, "Первая половина месяца"),
                new Tuple<int, string>(2, "Вторая половина месяца"),
            };
            }
        }

        public int Year {
            get { return BeginDate.Year; }
            set {
                BeginDate = new DateTime(value, BeginDate.Month, 1, 0, 0, 0);
                EndDate = new DateTime(value, BeginDate.Month, DateTime.DaysInMonth(value, BeginDate.Month), 23, 59, 59);
            }
        }

        public int Month {
            get { return BeginDate.Month; }
            set {
                BeginDate = new DateTime(BeginDate.Year, value, 1, 0, 0, 0);
                EndDate = new DateTime(BeginDate.Year, value, DateTime.DaysInMonth(BeginDate.Year, value), 23, 59, 59);
            }
        }

        private int _fifteenDaysRang = 1;
        public int FifteenDaysRang {
            get { return _fifteenDaysRang; }
            set {
                _fifteenDaysRang = value;
                int mn = DateTime.DaysInMonth(BeginDate.Year, BeginDate.Month);
                int st = 1;
                int et = DateTime.DaysInMonth(BeginDate.Year, BeginDate.Month) / 2;

                if (value == 2)
                {
                    st = DateTime.DaysInMonth(BeginDate.Year, BeginDate.Month) / 2;
                    et = DateTime.DaysInMonth(BeginDate.Year, BeginDate.Month);
                }
                BeginDate = new DateTime(BeginDate.Year, BeginDate.Month, st, 0, 0, 0);
                EndDate = new DateTime(BeginDate.Year, BeginDate.Month, et, 23, 59, 59);
                this.SendPropertyChaged(nameof(FifteenDaysRang));
            }
        }
        #endregion

        #region Helps
        private void SendPropertyChaged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            BeginDate = BeginDate.AddDays(-1);
            EndDate = EndDate.AddDays(-1);
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            BeginDate = BeginDate.AddDays(1);
            EndDate = EndDate.AddDays(1);
        }
    }
}
