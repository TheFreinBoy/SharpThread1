using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SharpThread
{
    class Program
    {
        private static bool[] _canStop;

        static void Main()
        {
            new Program().Start();
        }

        void Start()
        {
            Console.WriteLine("Введіть крок роботи потоків:");
            if (!int.TryParse(Console.ReadLine(), out int step))
            {
                Console.WriteLine("Некоректний ввід. Програму завершено.");
                return;
            }
            
            Console.WriteLine("Введіть час роботи потоків у секундах через пробіл:");
            string input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input))
            {
                 Console.WriteLine("Некоректний ввід часу. Програму завершено.");
                 return;
            }

            int[] workTimes = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(int.Parse)
                                   .ToArray();

            int threadCount = workTimes.Length;
            _canStop = new bool[threadCount];

            Thread[] workers = new Thread[threadCount];
            
            for (int i = 0; i < threadCount; i++)
            {
                workers[i] = new Thread(Calculator);
                
                ThreadData data = new ThreadData 
                { 
                    Index = i, 
                    Step = step, 
                    WorkTimeSeconds = workTimes[i] 
                };
                
                workers[i].Start(data);
            }
            
            Thread stopper = new Thread(Stopper);
            stopper.Start(workTimes);
            
            foreach (Thread t in workers)
            {
                t.Join();
            }
            stopper.Join();

            Console.WriteLine("Усі потоки завершили роботу.");
        }

        class ThreadData
        {
            public int Index;
            public int Step;
            public int WorkTimeSeconds;
        }

        void Calculator(object obj)
        {
            ThreadData data = (ThreadData)obj;
            int threadId = data.Index + 1;

            long sum = 0;
            long count = 0;
            long currentNumber = 0;
            
            do
            {
                sum += currentNumber;
                count++;
                currentNumber += data.Step;

            } while (!Volatile.Read(ref _canStop[data.Index]));
            
            Console.WriteLine($"[Потік №{threadId}] Сума: {sum} | Крок: {data.Step} | Доданків: {count} | Час: {data.WorkTimeSeconds} сек.");
        }

        private void Stopper(object obj)
        {
            int[] times = (int[])obj;
            Stopwatch sw = Stopwatch.StartNew(); 
            
            int activeCount = times.Length;
            bool[] stopped = new bool[times.Length]; 
            
            while (activeCount > 0)
            {
                double elapsedSeconds = sw.Elapsed.TotalSeconds;

                for (int i = 0; i < times.Length; i++)
                {
                    if (!stopped[i] && elapsedSeconds >= times[i])
                    {
                        Volatile.Write(ref _canStop[i], true); 
                        stopped[i] = true;
                        activeCount--;
                    }
                }
                
                Thread.Sleep(50); 
            }
        }
    }
}