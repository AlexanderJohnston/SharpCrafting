using System;

namespace SharpCrafting
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var runtime = new Runtime();
            runtime.Entry();
            Console.WriteLine("Press any key to stop.");
            Console.ReadKey () ;
        }
    }
}
