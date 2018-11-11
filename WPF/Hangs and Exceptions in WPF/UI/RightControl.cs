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

            // This method has shown to sometimes work, sometimes not for 11.
            // An extreme observed case: 11 run failed, 11 11 (twice in a row) run worked, 11 run worked, 11 11 run failed*.
            // Thus, we conclude that we do not control where the Task is executed (CAN be the UI thread).
            // Notice that 3 (not to be confused with -3) also always works here! 
            // * Notice that no 11 was run twice because 11 11 => 11 in the HashSet that was used.
            if (executing == 11) { var c = SynchronizationContext.Current; SynchronizationContext.SetSynchronizationContext(null); o = await readSetupError.callAwaited(); SynchronizationContext.SetSynchronizationContext(c); }
            if (executing == 12) { var c = SynchronizationContext.Current; SynchronizationContext.SetSynchronizationContext(null); o = await readSetupError.callAwaited(); await Task.Delay(1000); SynchronizationContext.SetSynchronizationContext(c); }
            if (executing == 13) { SynchronizationContext.SetSynchronizationContext(null); o = await readSetupError.callAwaited(); }

            return "R: " + o;
        }
    }
}