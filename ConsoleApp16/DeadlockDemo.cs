namespace DeadlockLab;

public class DeadlockDemo
{
    public static void Run()
    {
        Console.WriteLine("=== DEADLOCK DEMO ===");

        // Два независимых ресурса. Каждый объект используется как отдельная блокировка.
        object lockA = new();
        object lockB = new();
        object lockC = new();


        //using Barrier barrier = new(2);

        Thread threadA = new(() =>
        {
            Console.WriteLine("Поток A: ожидает ресурс A");

            lock (lockA)
            {
                Console.WriteLine("Поток A: захватил ресурс A");
                //barrier.SignalAndWait();

                Console.WriteLine("Поток A: ожидает ресурс B");

                lock (lockB)
                {
                    Console.WriteLine("Поток A: захватил ресурс B");
                    Console.WriteLine("Поток A: ожидает ресурс С");

                    lock (lockC)
                    {
                        Console.WriteLine("Поток A: получил все ресурсы");
                    }
                }
            }
        });

        Thread threadB= new(() =>
        {
            Console.WriteLine("Поток B: ожидает ресурс A");

            lock (lockA)
            {
                Console.WriteLine("Поток B: захватил ресурс A");
                //barrier.SignalAndWait();

                Console.WriteLine("Поток B: ожидает ресурс B");

                lock (lockB)
                {
                    Console.WriteLine("Поток B: захватил ресурс B");
                    Console.WriteLine("Поток B: ожидает ресурс С");

                    lock (lockC)
                    {
                        Console.WriteLine("Поток B: получил все ресурсы");
                    }
                }
            }
        });

        Thread threadC = new(() =>
        {
            Console.WriteLine("Поток C: ожидает ресурс A");

            lock (lockA)
            {
                Console.WriteLine("Поток C: захватил ресурс A");

                Console.WriteLine("Поток C: ожидает ресурс B");

                lock (lockB)
                {
                    Console.WriteLine("Поток C: захватил ресурс B");
                    Console.WriteLine("Поток C: ожидает ресурс С");

                    lock (lockC)
                    {
                        Console.WriteLine("Поток C: получил все ресурсы");
                    }
                }
            }
        });

        threadA.IsBackground = true;
        threadB.IsBackground = true;
        threadC.IsBackground = true;

        threadA.Start();
        threadB.Start();
        threadC.Start();

        bool completedA = threadA.Join(5000);
        bool completedB = threadB.Join(5000);
        bool completedC = threadC.Join(5000);


        Console.WriteLine();

        if (!completedA || !completedB || !completedC)
        {
            Console.WriteLine("DEADLOCK ОБНАРУЖЕН!");
            Console.WriteLine("Потоки не завершились за 5 секунд.");
        }
        else
        {
            Console.WriteLine("Все потоки завершились.");
        }
    }
}