using System;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace Guinea.Core
{
    public interface IAsyncManager
    {
        string DebugName { get; }

        Exception OperationException { get; }
        float PercentComplete { get; }
        AsyncOperationStatus Status { get; }
        Task Task { get; }

        void Initialize();
    }
}