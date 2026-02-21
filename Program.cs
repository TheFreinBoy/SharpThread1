using System;
using System.Threading;

namespace SharpThread
{
    class Program
    {
        private volatile bool[] canStop = new bool[4];

        static void Main(string[] args)
        {
            new Program().Start();
        }

        void Start()
        {
            Console.WriteLine("Запуск обчислень...\n");
            
            for (int i = 0; i < 4; i++)
            {
                Thread t = new Thread(Calculator);
                t.Start(i); 
            }
            
            Thread stopper = new Thread(Stopper);
            stopper.Start();
        }

        void Calculator(object obj)
        {
            int threadIndex = (int)obj;
            int threadId = threadIndex + 1; 
            int step = threadId * 2;        

            long sum = 0;
            long count = 0;
            long currentNumber = 0;
            
            do
            {
                sum += currentNumber;
                count++;
                currentNumber += step;

            } while (!canStop[threadIndex]);
            
            Console.WriteLine($"[Потік {threadId}] завершив роботу. " +
                              $"Крок: {step,2} | Доданків: {count,9} | Сума: {sum}");
        }

        public void Stopper()
        {
            Random rnd = new Random();
            
            for (int i = 0; i < 4; i++)
            {
                int delay = rnd.Next(1000, 2000);
                Thread.Sleep(delay);
                
                canStop[i] = true;
                
            }
        }
    }
}
