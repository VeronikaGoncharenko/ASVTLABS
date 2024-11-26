using System;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedQueue.Common
{

    public class LongRunningJob : ILongRunningJob
    {
        public async Task RunAsync(string project, CancellationToken token)
        {
            var console = new ConsoleHelper();

            var startTime = DateTime.Now;
            var endTime = startTime + TimeSpan.FromSeconds(30);
            console.Write($"{startTime}: Started compute: ")
                .WriteLine(project, ConsoleColor.DarkCyan);
            while (DateTime.Now <= endTime)  
            {
                double q = 1;
                double w = 1.5;
                for (var s = 1; s < 10000; s++) {
                    q = q * 2 + w * s;
                }

                Console.WriteLine($"Compute task: {project}, %: {((DateTime.Now - startTime).TotalSeconds / 30) * 100:00.0}%");

                if (token.IsCancellationRequested)
                {
                    Console.WriteLine($"{DateTime.Now}: Cancelled compute: {project}");
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(1));

            }
            console.Write($"{endTime}: Compute completed ")
                .WriteLine(project, ConsoleColor.DarkCyan);
        }
    }
}
