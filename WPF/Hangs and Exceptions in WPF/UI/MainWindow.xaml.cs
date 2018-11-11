using System;
using System.Windows;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
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
                4, // Access exception
                5,
                //6, // Access exception
                7, // Access exception
                8, // Access exception
                9,
                10,
                11,
                12
            };

            foreach (var executing in toExecute)
            {
                var ST = Thread.CurrentThread.ManagedThreadId;
                var SCS = SynchronizationContext.Current;
                var EC = ExecutionContext.Capture();
                RightText.Text = "Doing " + executing;
                    
                var o = "Did " + executing + " : " + await RightControl.Run(executing);
                //SynchronizationContext.SetSynchronizationContext(SCS);
                var RT = Thread.CurrentThread.ManagedThreadId;
                if (RT != ST)
                    Console.WriteLine();
                RightText.Text = o;
                Dispatcher.Invoke(() => RightText.Text = o);
                SCS.Post(delegate
                {
                    RightText.Text = o;
                }, null);
                await Task.Delay(100);
            }
            RightText.Text = "Done";
        }

        private async void LeftButtonClick(object sender, RoutedEventArgs e)
        {
            var toExecute = new HashSet<int>
            {
                //1, // Hangs UI thread
                //2, // Hangs UI thread
                3, 
                //4, // Access exception
                5,
                //6, // Access exception
                //7, // Access exception
                //8, // Access exception
                9,
                10,
                11,
                12
            };

            foreach (var executing in toExecute)
            {
                var readSetupError = new FileReader(15);

                var o = string.Empty;

                if (executing == 1) o = await readSetupError.CallUnawaited();
                if (executing == 2) o = await readSetupError.CallUnawaited().ConfigureAwait(false);
                if (executing == 3) o = await readSetupError.CallAwaited();
                if (executing == 4) o = await readSetupError.CallAwaited().ConfigureAwait(false);

                if (executing == 5) { var c = SynchronizationContext.Current; SynchronizationContext.SetSynchronizationContext(null); o = await readSetupError.CallUnawaited(); SynchronizationContext.SetSynchronizationContext(c); }
                if (executing == 6) { var c = SynchronizationContext.Current; SynchronizationContext.SetSynchronizationContext(null); o = await readSetupError.CallAwaited(); SynchronizationContext.SetSynchronizationContext(c); }
                if (executing == 7) { SynchronizationContext.SetSynchronizationContext(null); o = await readSetupError.CallUnawaited(); }
                if (executing == 8) { SynchronizationContext.SetSynchronizationContext(null); o = await readSetupError.CallAwaited(); }

                if (executing == 9) o = await Task.Run(async () => await readSetupError.CallUnawaited());
                if (executing == 10) o = await Task.Run(async () => await readSetupError.CallUnawaited().ConfigureAwait(false));
                if (executing == 11) o = await Task.Run(async () => await readSetupError.CallAwaited());
                if (executing == 12) o = await Task.Run(async () => await readSetupError.CallAwaited().ConfigureAwait(false));

                LeftText.Text = "Did " + o;
                await Task.Delay(100);
            }
            LeftText.Text = "Done";
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