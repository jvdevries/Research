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
            var o = "Initialized";

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

            return "R: " + o;
        }
    }
}