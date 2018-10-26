import java.util.*;

public class Volatile {
 public volatile int volFieldA, volFieldB = 0;

 private void Run() {
 	 volFieldA = 1;
 	 volFieldB = volFieldA;
 }

 public static void main(String[] args) {
  Volatile e = new Volatile();
  
  // Java needs to pre-compile for Hotspot, but doesn't do this unless a significant number of runs is detected.
  for (int i = 0; i < Integer.parseInt(args[0]); i++) {
  	  e.Run();
  }
  
  // Prevent the compiler from optimizing everything away.
  System.out.println(e.volFieldA + " " + e.volFieldB);
 }
}