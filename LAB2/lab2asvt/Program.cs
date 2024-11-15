
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // Создаем и запускаем четыре задачи асинхронно.
        Task task1 = LoadDataAsync("Задача 1", 1000);
        Task task2 = LoadDataAsync("Задача 2", 2000);
        Task task3 = LoadDataAsync("Задача 3", 1500);
        Task task4 = LoadDataAsync("Задача 4", 500);

        // Запускаем все задачи параллельно и ждем их завершения.
        await Task.WhenAll(task1, task2, task3, task4);
    }

    // Асинхронный метод для загрузки данных.
    private static async Task LoadDataAsync(string taskName, int delay)
    {
        Console.WriteLine($"{taskName} начинает загрузку данных...");

        // Ожидание длительной операции с использованием Task.Delay вместо Thread.Sleep.
        await Task.Delay(delay);

        Console.WriteLine($"{taskName} завершила загрузку данных.");
    }
}