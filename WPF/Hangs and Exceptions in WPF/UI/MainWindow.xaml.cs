using System;
using System.Windows;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Model;
using VM;

// Summary: Do not use the shortcuts ConfigureAwait(false) or SyncContext(null).
// Reason: Shortcuts hide bad async/await usage, which may become a race condition**.
// Notice: It CAN work without shortcuts, but it won't work per-se with*.
// * SyncContext(null) can fail-to-fix IF running on UI thread.
// * ConfigureAwait(false) can fail-to-fix IF running on UI thread.
// * ConfigureAwait(false) can fail-to-fix because it does not propagate.
// ** Any task MAY run on the UI thread, but may seldomly do this (e.g. only with high load).
// Alternative: run bad methods on a seperate thread till they are properly fixed.

// Lessons learned:
// A task is executed in a context, which includes a thread.
// We have no control over which thread it runs at *.
// After the task is done, the code following the task is executed (called continuation).
// This continuation can be in the task context, or in the pre-task context.
// ConfigureAwait is used to TRY to set the continuation context (pre-task or task).
// But because we do not know what runs where, this may either fix or cause problems.
// * MainWindow -> Seperate Class decreases the likelihood of running on UI thread.
// * MVVM seems to decrease the likelihood of running on UI thread to virtually 0.

namespace UI
{
    public partial class MainWindow : Window
    {
        private async void RightButtonClick(object sender, RoutedEventArgs e)
        {
            var toExecute = new HashSet<int>
            {
                //1, // Hangs UI thread
                //2, // Hangs UI thread
                3,
                4,
                5,
                //6, // Access exception
                7,
                8,
                9,
                //11, // Access exception
                //12, // Access exception
                13,
            };

            foreach (var executing in toExecute)
            {
                RightText.Text = "Doing " + executing;
                RightText.Text = "Did " + executing + " : " + await RightControl.Run(executing);
                await Task.Delay(100);
            }
        }

        private async void LeftButtonClick(object sender, RoutedEventArgs e)
        {
            var readSetupError = new FileReader(15);

            var toExecute = new HashSet<int>
            {
                //1, // hangs UI thread
                //2, // hangs UI thread
                //3, // Access exception
                4,
                5,
                //6, // Access exception
                7,
                8,
                9
            };

            foreach (var executing in toExecute)
            {
                var o = "Doing " + executing;
                LeftText.Text = o;

                // an unawaited async at a deeper level Hangs
                if (executing == 1) o = await readSetupError.callUnawaited();

                // ConfigureAwait(false) does not propagate, so it does not fix the hang
                if (executing == 2) o = await readSetupError.callUnawaited().ConfigureAwait(false);

                // ConfigureAwait(false) can cause exception errors IF it runs on the UI thread
                if (executing == 3) o = await readSetupError.callAwaited().ConfigureAwait(false);
                if (executing == 4) o = await readSetupError.callAwaited();

                // Removing the SetSynchronizationContext resolves the Hang
                if (executing == 5) { var c = SynchronizationContext.Current; SynchronizationContext.SetSynchronizationContext(null); o = await readSetupError.callUnawaited(); SynchronizationContext.SetSynchronizationContext(c); }

                // Removing the SetSynchronizationContext causes Exception error IF it runs on the UI thread
                if (executing == 6) { var c = SynchronizationContext.Current; SynchronizationContext.SetSynchronizationContext(null); o = await readSetupError.callAwaited(); SynchronizationContext.SetSynchronizationContext(c); }

                if (executing == 7) o = await Task.Run(async () => await readSetupError.callUnawaited());
                if (executing == 8) o = await Task.Run(async () => await readSetupError.callUnawaited().ConfigureAwait(false));
                if (executing == 9) o = await Task.Run(async () => await readSetupError.callAwaited().ConfigureAwait(false));

                RightText.Text = o;
                await Task.Delay(100);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            DataContext = new ViewModelBase();
        }

        readonly DispatcherTimer _timer = new DispatcherTimer();
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _timer.Interval = TimeSpan.FromMilliseconds(10);
            _timer.Tick += TimerTick;
            _timer.Start();
        }

        private int _ticks;
        private void TimerTick(object sender, EventArgs e)
        {
            _ticks++;
            txtTicks.Text = _ticks.ToString();
        }
    }
}