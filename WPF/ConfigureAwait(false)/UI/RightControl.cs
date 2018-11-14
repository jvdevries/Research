using System.Threading;
using System.Threading.Tasks;
using Model;

namespace UI
{
    public static class RightControl
    {
        public static async Task<string> Run(int executing)
        {
            var readSetupError = new FileReader(15);

            var o = string.Empty;

            var ThreadB = System.Threading.Thread.CurrentThread.ManagedThreadId;
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
            var ThreadA = System.Threading.Thread.CurrentThread.ManagedThreadId;

            //await Task.Delay(15); // Solves path 6 crash

            return o;
        }
    }
}