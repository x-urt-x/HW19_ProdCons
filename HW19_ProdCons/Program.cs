using System;
using System.Reflection.Metadata.Ecma335;
using ProduserConsumerNS;

namespace HW19_ProdCons
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await using (var prodCons = ProduserConsumer.Instance)
            {
                var task1 = Task.Run(() => AddSomeNum(prodCons));
                Thread.Sleep(3000);             //задержки чтобы была видна асинхронность 
                var task2 = Task.Run(() => AddSomeNum(prodCons));
                Thread.Sleep(1000);             //задержки чтобы была видна асинхронность 
                var task3 = Task.Run(() => AddSomeNum(prodCons));
                Task.WaitAll(task1, task2, task3);
            }
            Console.WriteLine("end");
            Console.ReadLine();
        }
        public static async Task AddSomeNum(ProduserConsumer prodCons)
        {
            var rnd = new Random();
            int num;
            for (int i = 0; i < 5; i++)
            {
                num = rnd.Next(100);
                Console.WriteLine($"adding new number {num} from {Thread.CurrentThread.ManagedThreadId} tread");
                prodCons.AddNumber(num);
            }
        }
    }
}
