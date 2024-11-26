using System.Threading;
using System.Threading.Tasks;

namespace DistributedQueue.Common
{
    public interface ILongRunningJob
    {
        Task RunAsync(string name, CancellationToken token);
    }
}