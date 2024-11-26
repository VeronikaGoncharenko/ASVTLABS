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