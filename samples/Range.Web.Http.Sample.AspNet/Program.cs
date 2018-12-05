using System;
using Microsoft.Owin.Hosting;

namespace Range.Web.Http.Sample.AspNet
{
    public class Program
    {
        private static void Main()
        {
            var baseAddress = "http://localhost:64686/";

            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine($"Running at {baseAddress}");
                Console.ReadLine();
            }
        }
    }
}
