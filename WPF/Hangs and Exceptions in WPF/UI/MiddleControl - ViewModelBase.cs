using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Model;
using UI;

namespace VM
{
    public partial class ViewModelBase : BindableBase
    {
        public ICommand MiddleButtonClick => new RelayCommand(async programName =>
        {
            var readSetupError = new FileReader(15);

            var execute = new HashSet<int>
            {
                //1, // Hangs UI Thread
                //2, // Hangs UI Thread
                3,
                4,
                5,
                6,
                7,
                8,
                9
            };

            foreach (var executing in execute)
            {
                var o = "Doing " + executing;
                MiddleText = "Doing " + executing;

                // an unawaited async at a deeper level Hangs
                if (executing == 1) o = await readSetupError.CallUnawaited();

                // ConfigureAwait(false) does not propagate, so it does not fix the hang
                if (executing == 2) o = await readSetupError.CallUnawaited().ConfigureAwait(false);

                // ConfigureAwait(false) can cause exception errors IF it runs on the UI thread
                if (executing == 3) o = await readSetupError.CallAwaited().ConfigureAwait(false);

                if (executing == 4) o = await readSetupError.CallAwaited();

                // Removing the SetSynchronizationContext resolves the Hang
                if (executing == 5) { var c = SynchronizationContext.Current; SynchronizationContext.SetSynchronizationContext(null); o = await readSetupError.CallUnawaited(); SynchronizationContext.SetSynchronizationContext(c); }

                // Removing the SetSynchronizationContext causes Exception error IF it runs on the UI thread
                if (executing == 6) { var c = SynchronizationContext.Current; SynchronizationContext.SetSynchronizationContext(null); o = await readSetupError.CallAwaited(); SynchronizationContext.SetSynchronizationContext(c); }

                if (executing == 7) o = await Task.Run(async () => await readSetupError.CallUnawaited());
                if (executing == 8) o = await Task.Run(async () => await readSetupError.CallUnawaited().ConfigureAwait(false));
                if (executing == 9) o = await Task.Run(async () => await readSetupError.CallAwaited().ConfigureAwait(false));

                MiddleText = "Did " + executing + " : " + o;
                await Task.Delay(100);
            }
        });

        public string LeftText
        {
            get => _leftText;
            set => SetProperty(ref _leftText, value);
        }
        private string _leftText = "Left";

        public string MiddleText
        {
            get => _middleText;
            set => SetProperty(ref _middleText, value);
        }
        private string _middleText = "Middle";

    }
}