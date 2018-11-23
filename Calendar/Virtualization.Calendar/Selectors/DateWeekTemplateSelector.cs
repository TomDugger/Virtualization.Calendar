using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Virtualization.Calendar.Selectors
{
    public class DateWeekTemplateSelector: DataTemplateSelector
    {
        public DataTemplate YearTemplate { get; set; }
        public DataTemplate MontTemplate { get; set; }
        public DataTemplate DayTemplate { get; set; }

        public DataTemplate WeekTemplate { get; set; }
        public DataTemplate CurrentTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                var res = item.ToString();
                if (res.Length == 4)
                    return YearTemplate;
                else if (res.Length <= 2)
                    return MontTemplate;
                else if (item is DateTime date)
                {
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                        return WeekTemplate;
                    else if (date.Date == DateTime.Now.Date)
                        return CurrentTemplate;
                    return DayTemplate;
                }
                else return base.SelectTemplate(item, container);
            }
            else return base.SelectTemplate(item, container);
        }
    }
}
