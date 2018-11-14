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
                9,
                10,
                11,
                12
            };

            foreach (var executing in execute)
            {
                var o = "Doing " + executing;
                MiddleText = "Doing " + executing;

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

                MiddleText = "Did " + executing + " : " + o;
                await Task.Delay(100);
            }
        });

        public string LeftText
        {
            get => _leftText;
            set => SetProperty(ref _leftText, value, nameof(LeftText));
        }
        private string _leftText = "Left";

        public string MiddleText
        {
            get => _middleText;
            set => SetProperty(ref _middleText, value, nameof(MiddleText));
        }
        private string _middleText = "Middle";

    }
}