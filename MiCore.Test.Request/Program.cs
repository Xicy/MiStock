using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MiCore.Test.Request
{
    class Program
    {
        static void Main(string[] args)
        {
            Parallel.For(1, 1000, i =>
            {
                try
                {
                    WebRequest.Create($"http://127.0.0.1:8080/{i % 5}.png").GetResponse();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            });

        }
    }
}
