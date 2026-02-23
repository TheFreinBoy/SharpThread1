
namespace SharpThread
{
    class Program
    {
        private static int _threadCount = 8;
        private static bool[] _canStop = new bool[_threadCount];
        
        static void Main()
        {
            new Program().Start();
        }

        void Start()
        {
            Console.WriteLine("Запуск обчислень...\n");
            
            for (int i = 0; i < _threadCount; i++)
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

            } while (!Volatile.Read(ref _canStop[threadIndex]));
            
            Console.WriteLine($"[Потік {threadId}] завершив роботу. " +
                              $"Крок: {step,2} | Доданків: {count,9} | Сума: {sum}");
        }

        private void Stopper()
        {
            Random rnd = new Random();
            
            for (int i = 0; i < _threadCount; i++)
            {
                int delay = rnd.Next(1000, 2000);
                Thread.Sleep(delay);
                
                Volatile.Write(ref _canStop[i], true);
                
            }
        }
    }
}
