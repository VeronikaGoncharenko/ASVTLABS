// Connect to the TaskQueue
using DistributedQueue.Common;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Hangfire.Storage.Monitoring;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal partial class Program
{
    private static ConsoleHelper console = new ConsoleHelper();

    private static async Task Main(string[] args)
    {
        var redis = ConnectionMultiplexer.Connect("localhost:6379");
        GlobalConfiguration.Configuration.UseRedisStorage(redis);
        GlobalConfiguration.Configuration.UseActivator(new JobActivator());

        List<string> jobMap = GetProcessingJobs().ToList();

        int jobIndex = GetMaxProjectId() + 1;

        PrintStatistics();

        var timer = new Timer(TimerCallbackFunc, null, 0, 1000);

        while (true)
        {
            var key = Console.ReadKey(true);

            PrintStatistics();

            if (key.Key == ConsoleKey.N) {
                var jobId = BackgroundJob.Enqueue<ILongRunningJob>("compute",
                    (w) => w.RunAsync(jobIndex.ToString(), CancellationToken.None));
                jobMap.Add(jobId);
                console.WriteLine($"Added job: {jobId}. Compute task: {jobIndex}");
                Interlocked.Increment(ref jobIndex);

            }

            if (key.Key == ConsoleKey.P)
            {
                var jobId = BackgroundJob.Enqueue<IComputePiJob>("pi",
                    (w) => w.ComputePyAsync(jobIndex.ToString(), 100000000, CancellationToken.None));
                jobMap.Add(jobId);
                console.WriteLine($"Added job: {jobId}. Compute task: {jobIndex}");
                Interlocked.Increment(ref jobIndex);

            }

            if (key.Key == ConsoleKey.R)
            {
                var jobId = BackgroundJob.Enqueue<IComputeFactorialJob>("factorial",
                    (w) => w.ComputeFactorialAsync(jobIndex.ToString(),456, CancellationToken.None));
                jobMap.Add(jobId);
                console.WriteLine($"Added job: {jobId}. Compute task: {jobIndex}");
                Interlocked.Increment(ref jobIndex);
            }

            if (key.Key == ConsoleKey.C)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                console.Write("Enter job key: ");
                var jobKey = Console.ReadLine() ?? string.Empty;

                jobKey = jobMap.FirstOrDefault(k => k.StartsWith(jobKey));

                if (jobKey == null)
                {
                    continue;
                }

                BackgroundJob.Delete(jobKey);
                console.WriteLine($"Canceled job: {jobKey}");
                PrintStatistics();
                timer.Change(0, 1000);
            }

            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }

    }

    private static void TimerCallbackFunc(object? state)
    {
        PrintStatistics();
    }

    private static IEnumerable<string> GetProcessingJobs()
    {
        var api = JobStorage.Current.GetMonitoringApi();
        var renderJobs = api.ProcessingJobs(0, 1000);
        return renderJobs.Select(x => x.Key);
    }

    private static void PrintJobs(JobList<ProcessingJobDto> renderJobs)
    {
        var api = JobStorage.Current.GetMonitoringApi();

        console.WriteLine("============");
        console.WriteLine("Processing:", ConsoleColor.Green);
        foreach (var renderJob in renderJobs)
        {
            console.WriteLine($"{renderJob.Value.StartedAt}: {renderJob.Key}. Compute task: {renderJob.Value.Job.Args.FirstOrDefault()}");
        }

        console.WriteLine("Enqueued:", ConsoleColor.DarkYellow);
        var enqueuedRenderJobs = api.EnqueuedJobs("compute", 0, 1000);
        foreach (var renderJob in enqueuedRenderJobs)
        {
            console.WriteLine($"{renderJob.Value.EnqueuedAt}: {renderJob.Key}. State={renderJob.Value.State}. " +
                $"Compute task: {renderJob.Value.Job.Args.FirstOrDefault()}");
        }
    }

    private static object _locker = new object();

    private static int GetMaxProjectId()
    {
        var api = JobStorage.Current.GetMonitoringApi();
        var enqueued = api.EnqueuedJobs("compute", 0, 1000);
        return 0;
        //return enqueued.Max(x => int.Parse(x.Value.Job.Args.FirstOrDefault()?.ToString() ?? "0"));
    }

    private static void PrintStatistics()
    {
        var api = JobStorage.Current.GetMonitoringApi();
        var statistics = api.GetStatistics();
        var renderJobs = api.ProcessingJobs(0, 1000);

        var enqueuedCount = api.EnqueuedJobs("compute", 0, 1000).Count;

        lock (_locker)
        {
            console.Clear();

            console.Write($"{"Servers: ", 12}")
                .WriteLine($"{statistics.Servers}", ConsoleColor.DarkCyan);
            console.WriteLine($"{"Recurring: ",12}{statistics.Recurring}");
            console.Write($"{"Enqueued: ", 12}")
                .WriteLine($"{enqueuedCount}", ConsoleColor.DarkYellow);
            console.WriteLine($"{"Queues: ",12}{statistics.Queues}");
            console.Write($"{"Scheduled: ", 12}")
                .WriteLine($"{statistics.Scheduled}", ConsoleColor.DarkYellow);
            console.Write($"{"Processing: ", 12}")
                .WriteLine($"{statistics.Processing}", ConsoleColor.Green);
            console.Write($"{"Succeeded: ", 12}")
                .WriteLine($"{statistics.Succeeded}", ConsoleColor.Green);
            console.Write($"{"Failed: ", 12}")
                .WriteLine($"{statistics.Failed}", ConsoleColor.Black, ConsoleColor.DarkRed);
            console.WriteLine($"{"Deleted: ",12}{statistics.Deleted}");

            PrintJobs(renderJobs);
        }
    }
}