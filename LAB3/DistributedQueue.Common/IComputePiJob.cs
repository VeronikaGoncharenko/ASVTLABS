using System.Threading;
using System.Threading.Tasks;

namespace DistributedQueue.Common
{
    public interface IComputePiJob
    {
        Task ComputePyAsync(string name, int iterrations, CancellationToken token);
    }
}