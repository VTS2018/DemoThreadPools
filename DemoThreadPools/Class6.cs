using System;
using System.Threading;

//C#线程系列讲座(1)：BeginInvoke和EndInvoke方法
namespace DemoThreadPools
{
    class Program
    {
        private static int newTask(int ms)
        {
            Console.WriteLine("任务开始");
            Thread.Sleep(ms);
            Random random = new Random();
            int n = random.Next(10000);
            Console.WriteLine("\n任务完成");
            return n;
        }
        //表示委托，委托是一种数据结构，它引用静态方法或引用类实例及该类的实例方法。
        //和方法有一样的签名
        private delegate int NewTaskDelegate(int ms);
        static void Main(string[] args)
        {
            /*
            //声明委托指向一个方法名
            NewTaskDelegate task = newTask;

            //在创建了此对象的线程上异步执行委托。
            //BeginInvoke创建的线程都是后台线程
            //这种线程一但所有的前台线程都退出后（其中主线程就是一个前台线程），不管后台线程是否执行完毕，都会结束线程，并退出程序
            IAsyncResult asyncResult = task.BeginInvoke(200, null, null);

            // EndInvoke方法将被阻塞2秒
            // 会卡死界面
            int result = task.EndInvoke(asyncResult);
            Console.WriteLine(result);
            */


            /*
            NewTaskDelegate task = newTask;
            IAsyncResult asyncResult = task.BeginInvoke(2000, null, null);

            //如果异步调用没有完成，就会执行该代码，一直输出，会给客户一种非常友好的方式
            while (!asyncResult.IsCompleted)
            {
                Console.Write("*");
                Thread.Sleep(100);
                //由于是异步，所以“*”可能会在“任务开始”前输出
            }

            // 由于异步调用已经完成，因此， EndInvoke会立刻返回结果
            int result = task.EndInvoke(asyncResult);
            Console.WriteLine(result);
            //区别：界面代码还在运行，代码还在执行，给异步调用充分的时间执行，等它执行完毕
            */

            NewTaskDelegate task = newTask;
            IAsyncResult asyncResult = task.BeginInvoke(2000, null, null);
            //WaitOne的第一个参数表示要等待的毫秒数，在指定时间之内，WaitOne方法将一直等待，直到异步调用完成，并发出通知，
            //WaitOne方法才返回true。当等待指定时间之后，异步调用仍未完成，WaitOne方法返回false，
            //如果指定时间为0，表示不等待，如果为-1，表示永远等待，直到异步调用完成
            while (!asyncResult.AsyncWaitHandle.WaitOne(100, false))
            {
                Console.Write("*");
            }
            int result = task.EndInvoke(asyncResult);
            Console.WriteLine(result);
        }
    }
}