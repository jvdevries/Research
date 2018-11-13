using System;
using System.Windows;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using Model;
using VM;

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
                var o = string.Empty;
                var ThreadB = System.Threading.Thread.CurrentThread.ManagedThreadId;
                o = await RightControl.Run(executing);
                var ThreadA = System.Threading.Thread.CurrentThread.ManagedThreadId;
                //Dispatcher.Invoke(() => RightText.Text = o); // Use instead of above to solve crash 6
                RightText.Text = "Did " + o;
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
                //3, 
                4, // Access exception
                //5,
                6, // Access exception
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
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += TimerTick;
            _timer.Start();
        }

        private int _ticks;
        private void TimerTick(object sender, EventArgs e)
        {
            Slider.Value = ++_ticks;
            if (_ticks == 10)
                _ticks = 0;
        }
    }
}