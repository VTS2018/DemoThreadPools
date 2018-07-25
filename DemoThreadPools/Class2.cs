using System;
using System.Threading;

//C#多线程学习(三) 生产者和消费者
namespace DemoThreadPools
{
    class Program
    {
        //Monitor
        static void Main(string[] args)
        {
            int result = 0; //一个标志位，如果是0表示程序没有出错，如果是1表明有错误发生
            Cell cell = new Cell();

            //下面使用cell初始化CellProd和CellCons两个类，生产和消费次数均为20次
            CellProd prod = new CellProd(cell, 20);
            CellCons cons = new CellCons(cell, 20);

            Thread producer = new Thread(new ThreadStart(prod.ThreadRun));
            Thread consumer = new Thread(new ThreadStart(cons.ThreadRun));
            //生产者线程和消费者线程都已经被创建，但是没有开始执行 
            try
            {
                producer.Start();
                consumer.Start();

                producer.Join();
                consumer.Join();
                //xx.Join()是指等待xx线程运行结束再运行其他线程；
                //xx.join(int)是指等待xx线程运行int毫秒后再运行其他线程。
                Console.ReadLine();
            }
            catch (ThreadStateException e)
            {
                //当线程因为所处状态的原因而不能执行被请求的操作
                Console.WriteLine(e);
                result = 1;
            }
            catch (ThreadInterruptedException e)
            {
                //当线程在等待状态的时候中止
                Console.WriteLine(e);
                result = 1;
            }
            //尽管Main()函数没有返回值，但下面这条语句可以向父进程返回执行结果
            Environment.ExitCode = result;
            //相互交互
            //在上面的例程中，同步是通过等待Monitor.Pulse()来完成的。
            //首先生产者生产了一个值，而同一时刻消费者处于等待状态，直到收到生产者的“脉冲(Pulse)”通知它生产已经完成，此后消费者进入消费状态，
            //而生产者开始等待消费者完成操作后将调用Monitor.Pulese()发出的“脉冲”。
        }
    }

    public class Cell
    {
        //当多线程公用一个对象时，也会出现和公用代码类似的问题
        //Monitor类可以锁定一个对象，一个线程只有得到这把锁才可以对该对象进行操作。
        //对象锁机制保证了在可能引起混乱的情况下一个时刻只有一个线程可以访问这个对象。
        //Monitor必须和一个具体的对象相关联，但是由于它是一个静态的类，所以不能使用它来定义对象，而且它的所有方法都是静态的，不能使用对象来引用

        /// <summary>
        /// Cell对象里边的内容
        /// </summary>
        int cellContents;

        /// <summary>
        /// 状态标志，为true时可以读取，为false则正在写入
        /// </summary>
        bool readerFlag = false;

        //供消费者读取
        public int ReadFromCell()
        {
            //读取线程逐个进行
            lock (this) // Lock关键字保证了什么，请大家看前面对lock的介绍
            {
                //一个线程进去该代码执行：会遇到两种情况，可读和不可读
                if (!readerFlag)//如果现在不可读取
                {
                    try
                    {
                        //等待WriteToCell方法中调用Monitor.Pulse()方法
                        Monitor.Wait(this);
                    }
                    catch (SynchronizationLockException e)
                    {
                        Console.WriteLine(e);
                    }
                    catch (ThreadInterruptedException e)
                    {
                        Console.WriteLine(e);
                    }
                }
                //开始读取
                Console.WriteLine("Consume: {0}", cellContents);
                //我读取完毕，设置FALSE表示可以由其他线程写入了
                readerFlag = false;

                //重置readerFlag标志，表示消费行为已经完成
                Monitor.Pulse(this);

                //通知WriteToCell()方法（该方法在另外一个线程中执行，等待中）
            }
            return cellContents;
        }

        //供生成者写入
        public void WriteToCell(int n)
        {
            lock (this)
            {
                if (readerFlag)//如果不可写
                {
                    try
                    {
                        Monitor.Wait(this);
                    }
                    catch (SynchronizationLockException e)
                    {
                        //当同步方法（指Monitor类除Enter之外的方法）在非同步的代码区被调用
                        Console.WriteLine(e);
                    }
                    catch (ThreadInterruptedException e)
                    {
                        //当线程在等待状态的时候中止 
                        Console.WriteLine(e);
                    }
                }
                cellContents = n;
                Console.WriteLine("Produce: {0}", cellContents);
                readerFlag = true;
                Monitor.Pulse(this);
                //通知另外一个线程中正在等待的ReadFromCell()方法
            }
        }
    }

    /// <summary>
    /// 生产者类
    /// </summary>
    public class CellProd
    {
        Cell cell; // 被操作的Cell对象
        int quantity = 1; // 生产者生产次数，初始化为1 

        public CellProd(Cell box, int request)
        {
            //构造函数
            cell = box;
            quantity = request;
        }

        public void ThreadRun()
        {
            for (int looper = 1; looper <= quantity; looper++)
                cell.WriteToCell(looper); //生产者向操作对象写入信息
        }
    }

    /// <summary>
    /// 消费者类
    /// </summary>
    public class CellCons
    {
        Cell cell;
        int quantity = 1;

        public CellCons(Cell box, int request)
        {
            //构造函数
            cell = box;
            quantity = request;
        }

        public void ThreadRun()
        {
            int valReturned;
            for (int looper = 1; looper <= quantity; looper++)
                valReturned = cell.ReadFromCell();//消费者从操作对象中读取信息
        }
    }
}