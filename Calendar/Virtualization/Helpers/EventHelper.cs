using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace Virtualization.Helpers
{
    public class EventHelper<T>
    {
        private readonly Dispatcher dispatcher;

        private readonly DispatcherPriority dispatcherPriority;

        private readonly Action<T> executeAction;

        private readonly DispatcherTimer timer;

        private T nextEventParameter;

        private bool isLimiting;

        private bool invocationRequired;

        public EventHelper(Dispatcher dispatcher, DispatcherPriority priority, int checkInterval, Action<T> executeAction)
        {
            this.dispatcher = dispatcher ?? throw new ArgumentNullException("dispatcher");
            this.dispatcherPriority = priority;
            this.executeAction = executeAction ?? throw new ArgumentNullException("executeAction");

            this.timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, checkInterval), priority, this.TimerTickHandler, dispatcher);
        }

        public void Trigger(T arg)
        {
            if (this.isLimiting)
            {
                this.nextEventParameter = arg;
                this.invocationRequired = true;
            }
            else
            {
                this.dispatcher.BeginInvoke(this.dispatcherPriority, this.executeAction, arg);
                this.isLimiting = true;
                this.timer.Start();
            }
        }

        private void TimerTickHandler(object sender, EventArgs eventArgs)
        {
            this.timer.Stop();

            if (this.invocationRequired)
                this.dispatcher.BeginInvoke(this.dispatcherPriority, this.executeAction, this.nextEventParameter);

            this.isLimiting = false;
        }
    }
}
