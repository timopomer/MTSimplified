using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTSimplified
{
    class Program
    {
        static int i = 0;

        static void Main(string[] args)
        {
            ThreadOrganizer threadO = new ThreadOrganizer(10,100);

            threadO.onDone += (s,e) => done();
            threadO.onStopped += (s, e) => canceled();
            threadO.onThreadCreate += (s, e) => created();
            threadO.onThreadFinish += (s, e) => finished();

            for (int i = 0; i<200;i++)
            threadO.addTask(print);

            threadO.startAll();
            Console.ReadLine();
            threadO.stopAll();

            while (true) ; //comment
        }



        public static void print()
        {
            Thread.Sleep(1000);
            i++;
            Console.WriteLine(i);
            
        }
        public static void created()
        {
            Console.WriteLine("created");
        }
        public static void finished()
        {
            Console.WriteLine("finished");
        }
        public static void done()
        {
            Console.WriteLine("Everything is done");
        }
        public static void canceled()
        {
            Console.WriteLine("Everything is canceled");
        }

    }
}
