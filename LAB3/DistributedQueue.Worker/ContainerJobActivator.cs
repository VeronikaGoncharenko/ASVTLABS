using DistributedQueue.Common;
using Hangfire;
using System;
using System.Collections.Generic;

public class ContainerJobActivator : JobActivator
{
    private static Dictionary<Type, Type> keyValuePairs = new Dictionary<Type, Type>()
    {
        [typeof(IComputePiJob)] = typeof(GregoryLeibnizGetPIJob),
        [typeof(ILongRunningJob)] = typeof(LongRunningJob),
        [typeof(IComputeFactorialJob)] = typeof(FactorialJob)
    };

    public override object ActivateJob(Type type)
    {
        var implementor = keyValuePairs.GetValueOrDefault(type);
        if (implementor == null)
        {
            return new LongRunningJob();
        }

        return Activator.CreateInstance(implementor)!;
    }
}