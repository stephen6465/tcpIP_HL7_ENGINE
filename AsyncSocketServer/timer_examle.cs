using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Timers;

namespace AsyncSocketServer
{
    class timer_examle
    {
   private static Timer aTimer;

   //public static void Main()
   //{
   //     // Create a timer with a two second interval.
   //     aTimer = new System.Timers.Timer(2000);
   //     // Hook up the Elapsed event for the timer. 
   //     aTimer.Elapsed += OnTimedEvent;
   //     aTimer.Enabled = true;

   //     Console.WriteLine("Press the Enter key to exit the program... ");
   //     Console.ReadLine();
   //     Console.WriteLine("Terminating the application...");
   //}

    private static void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
    }
}
// The example displays output like the following: 
//       Press the Enter key to exit the program... 
//       The Elapsed event was raised at 4/5/2014 8:48:58 PM 
//       The Elapsed event was raised at 4/5/2014 8:49:00 PM 
//       The Elapsed event was raised at 4/5/2014 8:49:02 PM 
//       The Elapsed event was raised at 4/5/2014 8:49:04 PM 
//       The Elapsed event was raised at 4/5/2014 8:49:06 PM 
//        
//       Terminating the application...
 }

