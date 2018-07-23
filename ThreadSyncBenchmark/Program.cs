using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadSyncBenchmark
{
    class Program
    {
        static int _count = 0;
        static readonly object _locker = new object();
        static SpinLock _spinLock = new SpinLock(false);

        static void Main(string[] args)
        {
            Console.WriteLine("Starting");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Parallel.For(0, 2, i =>
            {
                IterateAll();
            });

            sw.Stop();
            Console.WriteLine($"Count: {_count}");
            Console.WriteLine($"Finished in {sw.ElapsedMilliseconds}ms");
        }

        static void IterateAll()
        {
            for (int i = 0; i < 10000000; i++)
            {
                Thread.Sleep(0);
                IterateInterlocked();
            }
        }

        static void IterateNoLock()
        {
            _count++;
        }

        static void IterateMonitorLock()
        {
            lock (_locker)
            {
                _count++;
            }
        }

        static void IterateSpinLock()
        {
            bool lockTaken = false;

            try
            {
                _spinLock.Enter(ref lockTaken);
                _count++;
            }
            finally
            {
                if (lockTaken) _spinLock.Exit();
            }          
        }

        static void IterateSpinWait()
        {
            bool lockTaken = false;

            try
            {
                _spinLock.Enter(ref lockTaken);
                _count++;
            }
            finally
            {
                if (lockTaken) _spinLock.Exit();
            }
        }

        static void IterateInterlocked()
        {
            Interlocked.Increment(ref _count);
        }

        static void IterateMemoryBarrier()
        {
            Thread.MemoryBarrier();
            _count++;
            Thread.MemoryBarrier();
        }
    }
}
