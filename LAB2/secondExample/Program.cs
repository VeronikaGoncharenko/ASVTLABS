using System;
using System.Threading;

class Program
{
    // СоздаемMutex для синхронизации доступа к общему ресурсу.
    private static Mutex mutex = new Mutex();

    static void Main()
    {
        // Создаем четыре потока с разными действиями.
        Thread thread1 = new Thread(() => WriteToResource("Поток 1", 5));
        Thread thread2 = new Thread(() => WriteToResource("Поток 2", 5));
        Thread thread3 = new Thread(() => WriteToResource("Поток 3", 5));
        Thread thread4 = new Thread(() => WriteToResource("Поток 4", 5));

        // Запускаем потоки.
        thread1.Start();
        thread2.Start();
        thread3.Start();
        thread4.Start();

        // Ожидаем завершения потоков.
        thread1.Join();
        thread2.Join();
        thread3.Join();
        thread4.Join();
    }

    // Метод, в котором потоки записывают данные в общий ресурс.
    private static void WriteToResource(string threadName, int times)
    {
        for (int i = 0; i < times; i++)
        {
            // Ждем, пока не получим доступ к Mutex.
            mutex.WaitOne();

            try
            {
                // Критическая секция: начинаем работу с общим ресурсом.
                Console.WriteLine($"{threadName} записывает: сообщение {i + 1}");
                Thread.Sleep(100); // Симулируем небольшую задержку для наглядности.
            }
            finally
            {
                // Освобождаем Mutex, чтобы другие потоки могли получить доступ.
                mutex.ReleaseMutex();
            }
        }
    }
}