using System;
using System.Threading;

//C#多线程学习(一) 多线程的相关概念
namespace DemoThreadPools
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ////Demo1 给当前线程起名为"System Thread"
            Thread.CurrentThread.Name = "System Thread";
            Console.WriteLine(Thread.CurrentThread.Name
                + "'Status:" + Thread.CurrentThread.ThreadState
                + ",HashCode:" + Thread.CurrentThread.GetHashCode()
                + ",ManagedThreadId:" + Thread.CurrentThread.ManagedThreadId
                + ",IsAlive:" + Thread.CurrentThread.IsAlive
                + ",IsBackground:" + Thread.CurrentThread.IsBackground
                + ",IsThreadPoolThread:" + Thread.CurrentThread.IsThreadPoolThread);
            Console.ReadLine();

            //Demo 2 
            //普通方式执行
            //Alpha al = new Alpha();
            //al.Beta();

            Console.WriteLine("Thread Start/Stop/Join Sample");
            Alpha oAlpha = new Alpha();

            //创建一个线程，使之执行Alpha类的Beta()方法
            Thread oThread = new Thread(new ThreadStart(oAlpha.Beta));
            oThread.Start();

            //如果oThread线程被终止了，就暂停一秒
            while (!oThread.IsAlive)
            {
                Thread.Sleep(1);
            }

            //终止掉线程
            oThread.Abort();

            //Thread.Join()方法使主线程等待，直到oThread线程结束
            oThread.Join();
            Console.WriteLine();
            Console.WriteLine("Alpha.Beta has finished");
            try
            {
                Console.WriteLine("Try to restart the Alpha.Beta thread");
                //尝试再次启动，发现无法启动该线程
                oThread.Start();
            }
            catch (ThreadStateException)
            {
                Console.Write("ThreadStateException trying to restart Alpha.Beta. ");
                Console.WriteLine("Expected since aborted threads cannot be restarted.");
                Console.ReadLine();
            }
        }
    }

    public class Alpha
    {
        //被线程来执行的函数
        public void Beta()
        {
            while (true)
            {
                Console.WriteLine("Alpha.Beta is running in its own thread.");
            }
        }
    }
}