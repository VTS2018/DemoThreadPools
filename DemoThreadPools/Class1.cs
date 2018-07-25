using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemoThreadPools
{
    class Class1
    {
        static void Main(string[] args)
        {
            Thread th = new Thread(delegate()
            {
                Console.WriteLine(Thread.CurrentThread.GetHashCode());
            });
            th.Start();

            Console.WriteLine(Thread.CurrentThread.GetHashCode());
        }
    }
}
