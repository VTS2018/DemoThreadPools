using System;
using System.Threading;

//C#多线程学习(三) 生产者和消费者
namespace DemoThreadPools
{
    class Program
    {
        //线程数组
        static internal Thread[] threads = new Thread[10];
        static void Main(string[] args)
        {
            Account acc = new Account(0);

            //准备10个线程
            for (int i = 0; i < 10; i++)
            {
                Thread t = new Thread(new ThreadStart(acc.DoTransactions));
                threads[i] = t;
            }

            //设置10个线程的名字
            for (int i = 0; i < 10; i++)
                threads[i].Name = i.ToString();

            //启动10个线程
            for (int i = 0; i < 10; i++)
                threads[i].Start();
            Console.ReadLine();
        }
    }

    internal class Account
    {
        private int balance;
        private Random r = new Random();
        internal Account(int initial)
        {
            balance = initial;
        }

        //多线程共享一段代码段，值属性共享
        internal int Withdraw(int amount)
        {
            if (balance < 0)
            {
                //如果balance小于0则抛出异常
                throw new Exception("Negative Balance");
            }

            //下面的代码保证在当前线程修改balance的值完成之前
            //不会有其他线程也执行这段代码来修改balance的值
            //因此，balance的值是不可能小于0 的
            //也就是说：虽然有10个线程在同时执行这段代码，单必须要排队进行，在上一个进程没有完成修改之前下一个不能来
            lock (this)
            {
                Console.WriteLine("Current Thread:" + Thread.CurrentThread.Name);
                //如果没有lock关键字的保护，那么可能在执行完if的条件判断之后
                //另外一个线程却执行了balance=balance-amount修改了balance的值
                //而这个修改对这个线程是不可见的，所以可能导致这时if的条件已经不成立了
                //但是，这个线程却继续执行balance=balance-amount，所以导致balance可能小于0
                if (balance >= amount)
                {
                    Thread.Sleep(5);
                    balance = balance - amount;
                    return amount;
                }
                else
                {
                    return 0; // transaction rejected
                }
            }
        }

        internal void DoTransactions()
        {
            for (int i = 0; i < 100; i++)
                Withdraw(r.Next(-50, 100));
        }
    }
}