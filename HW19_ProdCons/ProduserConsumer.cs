using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProduserConsumerNS
{
    public class MyClass
    {
        public MyClass()
        {
            var s = ProduserConsumer.Instance;
        }
    }
    public sealed class ProduserConsumer : IAsyncDisposable
    {
        private static readonly Lazy<ProduserConsumer> lazy = new(() => new ProduserConsumer());

        public AutoResetEvent startQueue = new AutoResetEvent(false);
        public bool stop = false;

        private ConcurrentQueue<int> numbersQueue = new ConcurrentQueue<int>();
        private List<int> primeNumbers = new List<int>();
        private Task worker;

        public static ProduserConsumer Instance => lazy.Value;
        private ProduserConsumer()
        {
            GenPrimeNum(10);
            worker = Task.Run(Work);
        }


        public void AddNumber(int num)
        {
            if (!stop)
            {
                numbersQueue.Enqueue(num);
                startQueue.Set();
            }
        }


        public async ValueTask DisposeAsync()
        {
            stop = true;
            await worker;
        }

        private void Work()
        {
            while (true)
            {
                if (numbersQueue.TryDequeue(out int num))
                {
                    Console.WriteLine("Count of prime numbers < "+num+ ": " + CalcPrimNum(num));
                }
                else
                {
                    if (stop) return;
                    startQueue.WaitOne();
                }
            }
        }

        private int CalcPrimNum(int num)
        {
            if (primeNumbers[primeNumbers.Count - 1] < num) GenPrimeNum(num);
            int i = 0;
            while (primeNumbers.Count > i && primeNumbers[i] < num)
            {
                i++;
            }
            Thread.Sleep(1000);
            return i;
        }

        private void GenPrimeNum(int maxNumber)
        {
            for (int i = primeNumbers.Count == 0 ? 2 : primeNumbers[primeNumbers.Count - 1] + 1; i <= maxNumber; i++)
            {
                if (IsPrime(i))
                {
                    primeNumbers.Add(i);
                }
            }
        }
        private bool IsPrime(int n)
        {
            if (n < 2)
                return false;
            if (n < 4)
                return true;

            for (int i = 2; i < n; i++)
            {
                if (n % i == 0) return false;
            }
            return true;
        }


    }
}
