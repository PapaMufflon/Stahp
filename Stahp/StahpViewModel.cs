using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using Stahp.Annotations;

namespace Stahp
{
    public class StahpViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<EventArgs> Notify;

        private readonly DispatcherTimer timer;
        private DateTime? startAction;
        private TimeSpan restTime;
        private string totalRestTime;

        public StahpViewModel()
        {
            TotalRestTime = TimeSpan.FromHours(2).ToString();
            restTime = TimeSpan.Parse(TotalRestTime);

            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };

            InterceptMouse.Start();
            InterceptMouse.MouseAction += (s, e) =>
            {
                if (restTime == TimeSpan.Zero)
                {
                    OnNotify();
                    return;
                }

                if (startAction.HasValue && DateTime.Now.Subtract(startAction.Value) > restTime)
                {
                    restTime = TimeSpan.Zero;
                    startAction = null;
                    timer.Stop();
                    timer.IsEnabled = false;

                    OnPropertyChanged("RestTime");

                    return;
                }
                
                if (startAction == null)
                    startAction = DateTime.Now;

                timer.Stop();
                timer.Start();

                OnPropertyChanged("RestTime");
            };

            timer.Tick += (s, e) =>
            {
                if (startAction.HasValue)
                    restTime = restTime.Subtract(DateTime.Now.Subtract(startAction.Value));

                timer.Stop();
                startAction = null;
            };
        }

        public string TotalRestTime
        {
            get { return totalRestTime; }
            set
            {
                totalRestTime = value;

                restTime = DateTime.Parse(TotalRestTime).TimeOfDay;
            }
        }

        public TimeSpan RestTime
        {
            get
            {
                if (startAction.HasValue)
                    return restTime.Subtract(DateTime.Now.Subtract(startAction.Value));

                return restTime;
            }
        }

        protected virtual void OnNotify()
        {
            EventHandler<EventArgs> handler = Notify;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            InterceptMouse.Stop();
        }
    }
}