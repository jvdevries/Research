import java.util.*;
import java.lang.reflect.*;
import sun.misc.*;

public class NoCachingPossibleStoreLoadSwapPossible {
 private volatile int X, Y, r1, r2, r3, r4 = 0;

 private void Run(int maxRuns, int printPer) {
  var start = System.currentTimeMillis();

  Unsafe unsafe = null;
  try {
   var f = Unsafe.class.getDeclaredField("theUnsafe");
   f.setAccessible(true);
   unsafe = (Unsafe) f.get(null);
  } catch (Exception e) {}

  for (int run = 1; run < maxRuns; run++) {
   r1 = r2 = r3 = r4 = X = Y = 0;
   unsafe.fullFence();

   var t1 = new Thread() {
    public void run() {
     X = 1;
     r2 = Y;
    }
   };

   var t2 = new Thread() {
    public void run() {
     Y = 1;
     r4 = X;
    }
   };

   t1.start();
   t2.start();

   try {
    t1.join();
   } catch (InterruptedException e) {}
   try {
    t2.join();
   } catch (InterruptedException e) {}

   if (r2 == 0 && r4 == 0)
    OccurenceFoundExit(run, start, System.currentTimeMillis());

   if (run % printPer == 0)
    PrintRunTime(run, start, System.currentTimeMillis());
  }

  PrintRunTime(maxRuns, start, System.currentTimeMillis());
 }

 private void OccurenceFoundExit(int runs, long start, long end) {
  System.out.println("Found occurence after " + runs + " runs, which took " + GetRunTime(runs, start, end));
  System.exit(1);
 }

 private void PrintRunTime(int runs, long start, long end) {
  System.out.println("Done " + runs + " runs, which took " + GetRunTime(runs, start, end));
 }

 private String GetRunTime(int runs, long start, long end) {
  var runTime = end - start;
  return (runTime < 60000) ?
   String.format("%.1f", ((float) runTime / 1000)) + " seconds" :
   String.format("%.2f", ((float) runTime / 60000)) + " minutes";
 }

 public static void main(String[] args) {
  var e = new NoCachingPossibleStoreLoadSwapPossible();
  e.Run(Integer.parseInt(args[0]), Integer.parseInt(args[1]));
 }
}