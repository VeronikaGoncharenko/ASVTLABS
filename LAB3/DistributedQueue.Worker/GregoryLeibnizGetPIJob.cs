using System;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedQueue.Common
{
    internal class GregoryLeibnizGetPIJob : IComputePiJob
    {
        public Task ComputePyAsync(string name, int iterrations, CancellationToken token)
        {

            var startTime = DateTime.Now;

            decimal sum = 0;
            decimal temp = 0;

            var iterrationsToCheck = 1000000;
            var iterrationCurrent = 0;

            for (int i = 0; i < iterrations; i++)
            {
                temp = 4m / (1 + 2 * i);
                sum += i % 2 == 0 ? temp : -temp;

                if (iterrationCurrent >= iterrationsToCheck)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine($"{DateTime.Now}: GregoryLeibnizGetPI: Cancelled compute PI {name}");
                        break;
                    }
                    var percent = ((float)i / (float)iterrations) * 100;
                    Console.WriteLine($"{DateTime.Now}: Compute task: {name}, %: {percent:00.0}%");
                    iterrationCurrent = 0;
                }
                iterrationCurrent++;
            }


            Console.WriteLine($"GregoryLeibnizGetPI: {name}, Iterrations: {iterrations}, PI={sum}");

            return Task.CompletedTask;
        }
    }
}
