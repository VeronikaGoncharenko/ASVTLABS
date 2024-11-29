# Лабораторная работа №3
## Распределенные задачи
### Внесенные изменения 
#### Интерфейс
``` csharp
if (key.Key == ConsoleKey.R)
            {
                var jobId = BackgroundJob.Enqueue<IComputeFactorialJob>("factorial",
                    (w) => w.ComputeFactorialAsync(jobIndex.ToString(),5, CancellationToken.None));
                jobMap.Add(jobId);
                console.WriteLine($"Added job: {jobId}. Compute task: {jobIndex}");
                Interlocked.Increment(ref jobIndex);
            }
```

При нажатии R начинается подсчет факториала
#### Создание нового класса
``` csharp
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedQueue.Common
{
    public class FactorialJob : IComputeFactorialJob
    {
        public async Task ComputeFactorialAsync(string name, int number, CancellationToken token)
        {
            if (number < 0)
            {
                Console.WriteLine($"{DateTime.Now}: Factorial of negative number {name} is not defined.");
                return;
            }

            long result = 1;

            for (int i = 1; i <= number; i++)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine($"{DateTime.Now}: Factorial computation for {name} cancelled.");
                    return;
                }

                result *= i;

                // Имитация долгих вычислений
                await Task.Yield(); // Позволяет другим задачам выполняться
            }

            Console.WriteLine($"Factorial of {number} is {result} (computed by {name}).");
        }
    }
}
```
#### Результат работы при нажатии R и P
![image](https://github.com/user-attachments/assets/37948031-7131-4f6f-97c7-a481712717b7)
