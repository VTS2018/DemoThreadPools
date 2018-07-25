using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace DemoThreadPools
{
    class DeThreadPool
    {
        static void Main(string[] args)
        {
            WaitCallback callBack = new WaitCallback(PooledFunc);

            ThreadPool.QueueUserWorkItem(callBack, "Is there any screw left?");
            ThreadPool.QueueUserWorkItem(callBack, "How much is a 40W bulb?");

            int ticks = Environment.TickCount;
            while (Environment.TickCount - ticks < 2000) ;

            ThreadPool.QueueUserWorkItem(callBack, "Decrease stock of monkey wrench");

            Console.WriteLine("Main thread. Is pool thread: {0}, Hash: {1}", Thread.CurrentThread.IsThreadPoolThread, Thread.CurrentThread.GetHashCode());
            Console.ReadLine();
        }

        static void PooledFunc(object state)
        {
            Console.WriteLine("Processing request '{0}'." + " Is pool thread: {1}, Hash: {2}",
                   (string)state,
                   Thread.CurrentThread.IsThreadPoolThread,
                   Thread.CurrentThread.GetHashCode());

            Console.WriteLine("Processing request '{0}'", (string)state);
            // Simulation of processing time
            Thread.Sleep(2000);

            Console.WriteLine("Request processed");
        }
    }
}