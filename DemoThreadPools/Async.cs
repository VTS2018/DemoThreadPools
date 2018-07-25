using System;
using System.Threading;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

/*关于异步调用的案例*/
namespace DemoAsynchronous
{
    #region 定义委托
    //将委托理解为：方法的一个抽象，或者是说方法的“类”
    //定义委托的方法签名
    public delegate int AddHandler(int a, int b);

    //一个费时的函数
    public class Additive
    {
        public static int Add(int a, int b)
        {
            Console.WriteLine("Add当前线程唯一表示：" + Thread.CurrentThread.ManagedThreadId);

            Console.WriteLine("开始计算：" + a + "+" + b);
            Thread.Sleep(5000); //模拟该方法运行三秒
            Console.WriteLine("计算完成！");
            return a + b;
        }
    }
    #endregion

    public class Asynchronismcallback
    {
        static void Main()
        {
            //SynchronizationFun();
            AsynchronousMain();
            //AsynchronismcallbackFun();

            //系统委托
            //Action d = new Action(fun);
            //Action<int,int,int,int>
            //Func<>
        }

        #region 同步调用
        public static void SynchronizationFun()
        {
            Console.WriteLine("===== 同步调用 SyncInvokeTest =====");

            AddHandler handler = new AddHandler(Additive.Add);
            //同步的效果就是当前的线程必须阻塞，得到result结果的返回，在winform下就会造成界面的假死

            //“委托人”去Invoke
            int result = handler.Invoke(1, 2);

            //直到委托执行的方法完全完成 下面才会执行
            Console.WriteLine("继续做别的事情。。。");
            Console.WriteLine(result);

            Console.ReadKey();
        }
        /*运行结果：
         ===== 同步调用 SyncInvokeTest =====
         开始计算：1+2
         计算完成！
         继续做别的事情。。。
         3
         */
        #endregion

        #region 异步调用
        public static void AsynchronousMain()
        {
            Console.WriteLine("===== 异步调用 AsyncInvokeTest =====");
            //你会发现这里的ManagedThreadId和执行 Add方法的 线程ID不一致的，我们使用异步的方式 简洁的操作了线程
            Console.WriteLine("AsynchronousMain当前线程唯一表示：" + Thread.CurrentThread.ManagedThreadId);

            AddHandler handler = new AddHandler(Additive.Add);

            //IAsyncResult: 异步操作接口(interface)
            //BeginInvoke: 委托(delegate)的一个异步方法的开始
            //BeginInvoke开始一个异步操作

            //委托人：开始BeginInvoke
            IAsyncResult result = handler.BeginInvoke(1, 2, null, null);
            Console.WriteLine("继续做别的事情。。。");


            //委托人：监控异步操作返回
            Console.WriteLine(handler.EndInvoke(result));
            Console.ReadKey();
        }

        /*运行结果：
         ===== 异步调用 AsyncInvokeTest =====
         继续做别的事情。。。
         开始计算：1+2
         计算完成！
         3      
         */

        #endregion

        #region 异步回调

        public static void AsynchronismcallbackFun()
        {
            Console.WriteLine("===== 异步回调 AsyncInvokeTest =====");
            //1.先用委托 挂载一个要执行的函数
            AddHandler handler = new AddHandler(Additive.Add);

            //异步操作接口(注意BeginInvoke方法的不同！)
            //2.BeginInvoke开始异步调用，【委托定义的参数，方法执行完毕之后回调的函数，状态】返回一个异步操作的状态
            IAsyncResult result = handler.BeginInvoke(1, 2, new AsyncCallback(CallbackFuction), "AsycState:OK");

            //这里的好处就是不用等待结果的返回 代码还能够继续往下执行
            Console.WriteLine("继续做别的事情。。。");
            Console.ReadKey();
        }

        static void CallbackFuction(IAsyncResult result)
        {
            //result 是“加法类.Add()方法”的返回值

            //AsyncResult 是IAsyncResult接口的一个实现类，引用空间：System.Runtime.Remoting.Messaging
            //AsyncDelegate 属性可以强制转换为用户定义的委托的实际类。

            AddHandler handler = (AddHandler)((AsyncResult)result).AsyncDelegate;

            //MethodInfo m = handler.Method;
            //Console.WriteLine(m.Name);
            //ParameterInfo[] par = m.GetParameters();

            Console.WriteLine(handler.EndInvoke(result));//结束异步返回计算的结果
            Console.WriteLine(result.AsyncState);//异步结束后的状态

        }
        /*运行结果：
        ===== 异步回调 AsyncInvokeTest =====
        开始计算：1+2
        继续做别的事情。。。
        计算完成！
        3
        AsycState:OK
        */

        #endregion
    }
}