using System.Threading;
using System.Threading.Tasks;

namespace DistributedQueue.Common
{
    public interface IComputeFactorialJob
    {
        Task ComputeFactorialAsync(string name, int number, CancellationToken token);
    }
}
