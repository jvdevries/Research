import java.util.*;

public class compilerHang {
 private boolean stop = false;

 private void Run(int timeOut) {
   var t2 = new Thread() {
    public void run() {
     try {
      Thread.sleep(timeOut * 1000);
      System.out.println("Timeout occured.");
      System.exit(0);
     } catch (InterruptedException e) {}
     stop = true;
    }
   };
   t2.start();
   var t1 = new Thread() {
    public void run() {
     try {
      Thread.sleep(100);
     } catch (InterruptedException e) {}
     stop = true;
    }
   };
   t1.start();

   while (!stop) ;

   System.out.println("Loop ended.");
   System.exit(0);

   try {
    t2.join();
   } catch (InterruptedException e) {}
   try {
    t1.join();
   } catch (InterruptedException e) {}
 }

 public static void main(String[] args) {
  var e = new compilerHang();
  e.Run(Integer.parseInt(args[0]));
 }
}