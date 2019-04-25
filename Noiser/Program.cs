using System;

namespace Noiser
{
    class Program
    {
        static void RunStuff()
        {
            var noiser = new Noiser();
            noiser.Run();
        }

        static void Main(string[] args)
        {
            RunStuff();
            Console.ReadKey();
        }
    }
}
