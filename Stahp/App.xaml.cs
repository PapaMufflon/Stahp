using System;
using Hardcodet.Wpf.TaskbarNotification;

namespace Stahp
{
    public partial class App
    {
        private TaskbarIcon tb;

        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            tb = (TaskbarIcon)FindResource("MyNotifyIcon");

            var viewModel = new StahpViewModel();
            viewModel.Notify += (s, evt) => tb.ShowBalloonTip("Stahp!!!", "Time's up.", BalloonIcon.Error);

            tb.DataContext = viewModel;
        }

        protected override void OnExit(System.Windows.ExitEventArgs e)
        {
            tb.Dispose();
        }
    }
}