using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Razensoft.Mediator
{
    public interface IMessageBus
    {
        void Publish<T>(T message);

        UniTask PublishAsync<T>(T message, CancellationToken cancellationToken = default);

        IDisposable Subscribe<T>(
            Func<T, CancellationToken, UniTask> action,
            CancellationToken cancellationToken = default);

        IDisposable Subscribe<T>(Action<T> action);
    }
}