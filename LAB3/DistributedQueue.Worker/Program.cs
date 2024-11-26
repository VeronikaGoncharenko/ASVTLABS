using DistributedQueue.Common;
using Hangfire;
using Hangfire.Common;
using Hangfire.Redis.StackExchange;
using Hangfire.Server;
using StackExchange.Redis;
using System;
using System.ComponentModel;

var redis = ConnectionMultiplexer.Connect("localhost:6379");
GlobalConfiguration.Configuration.UseRedisStorage(redis);

GlobalConfiguration.Configuration.UseActivator(new ContainerJobActivator());

using (var server = new BackgroundJobServer(new BackgroundJobServerOptions()
{
    Queues = new string[] { "compute", "pi", "factorial" }, WorkerCount = 3, 
}))
{
    Console.WriteLine("Hangfire Server started. Press any key to exit...");
    Console.ReadKey();
}
