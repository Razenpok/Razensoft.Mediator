using System.Threading;
using Cysharp.Threading.Tasks;

namespace Razensoft.Mediator
{
    public interface IOutputMediator
    {
        void Publish<TNotification>(TNotification notification)
            where TNotification : INotification;

        void Send<TRequest>(TRequest request)
            where TRequest : IRequest;

        UniTask AsyncSend<TRequest>(
            TRequest request,
            CancellationToken cancellationToken)
            where TRequest : IAsyncRequest;

        void ForgetSend<TRequest>(
            TRequest request,
            CancellationToken cancellationToken = default)
            where TRequest : IAsyncRequest;
    }
}