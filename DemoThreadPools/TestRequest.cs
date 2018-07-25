using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.IO;

namespace DemoThreadPools
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = new Uri("http://localhost:29875/Handler2.ashx");
            var num = 50;

            for (int i = 0; i < num; i++)
            {
                var request = WebRequest.Create(url);

                request.GetResponseAsync().ContinueWith(t =>
                {
                    var stream = t.Result.GetResponseStream();
                    using (TextReader tr = new StreamReader(stream))
                    {
                        Console.WriteLine(tr.ReadToEnd());
                    }
                });
            }
            Console.ReadLine();
        }
    }
}
