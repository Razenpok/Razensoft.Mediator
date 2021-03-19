using System.Threading;
using Cysharp.Threading.Tasks;

namespace Razensoft.Mediator
{
    public interface IBaseInputRequestHandler
    {
    }

    public interface IInputRequestHandler<in TRequest, out TResponse> : IBaseInputRequestHandler
        where TRequest : IRequest<TResponse>
    {
        TResponse Handle(TRequest request);
    }

    public interface IInputRequestHandler<in TRequest> : IInputRequestHandler<TRequest, Unit>
        where TRequest : IRequest<Unit>
    {
    }

    public interface IAsyncInputRequestHandler<in TRequest, TResponse> : IBaseInputRequestHandler
        where TRequest : IAsyncRequest<TResponse>
    {
        UniTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
    }

    public interface IAsyncInputRequestHandler<in TRequest> : IAsyncInputRequestHandler<TRequest, Unit>
        where TRequest : IAsyncRequest<Unit>
    {
    }

    public abstract class AsyncInputRequestHandler<TRequest, TResponse> : IAsyncInputRequestHandler<TRequest, TResponse>
        where TRequest : IAsyncRequest<TResponse>
    {
        UniTask<TResponse> IAsyncInputRequestHandler<TRequest, TResponse>.Handle(TRequest request,
            CancellationToken cancellationToken)
        {
            return Handle(request, cancellationToken);
        }

        protected abstract UniTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }

    public abstract class AsyncInputRequestHandler<TRequest> : IAsyncInputRequestHandler<TRequest>
        where TRequest : IAsyncRequest
    {
        async UniTask<Unit> IAsyncInputRequestHandler<TRequest, Unit>.Handle(TRequest request,
            CancellationToken cancellationToken)
        {
            await Handle(request, cancellationToken);
            return Unit.Value;
        }

        protected abstract UniTask Handle(TRequest request, CancellationToken cancellationToken);
    }

    public abstract class InputRequestHandler<TRequest, TResponse> : IInputRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        TResponse IInputRequestHandler<TRequest, TResponse>.Handle(TRequest request)
        {
            return Handle(request);
        }

        protected abstract TResponse Handle(TRequest request);
    }

    public abstract class InputRequestHandler<TRequest> : IInputRequestHandler<TRequest>
        where TRequest : IRequest
    {
        Unit IInputRequestHandler<TRequest, Unit>.Handle(TRequest command)
        {
            Handle(command);
            return Unit.Value;
        }

        protected abstract void Handle(TRequest command);
    }
}