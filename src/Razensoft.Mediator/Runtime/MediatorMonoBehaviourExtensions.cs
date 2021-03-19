using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Razensoft.Mediator
{
    public static class UnityExtensions
    {
        public static IDisposable RegisterHandler<TRequest>(
            this Component component,
            Func<TRequest, CancellationToken, UniTask> action)
            where TRequest : IAsyncRequest
        {
            var cancellationToken = component.GetCancellationTokenOnDestroy();
            var handler = OutputMediator.Unity.RegisterRequestHandler(action, cancellationToken);
            handler.AddTo(cancellationToken);
            return handler;
        }

        public static IDisposable RegisterHandler<TRequest>(
            this GameObject gameObject,
            Func<TRequest, CancellationToken, UniTask> action)
            where TRequest : IAsyncRequest
        {
            var cancellationToken = gameObject.GetCancellationTokenOnDestroy();
            var handler = OutputMediator.Unity.RegisterRequestHandler(action, cancellationToken);
            handler.AddTo(cancellationToken);
            return handler;
        }

        public static IDisposable RegisterHandler<TRequest>(
            this Component component,
            Action<TRequest> action)
            where TRequest : IRequest
        {
            var cancellationToken = component.GetCancellationTokenOnDestroy();
            var handler = OutputMediator.Unity.RegisterRequestHandler(action);
            handler.AddTo(cancellationToken);
            return handler;
        }

        public static IDisposable RegisterHandler<TRequest>(
            this GameObject gameObject,
            Action<TRequest> action)
            where TRequest : IRequest
        {
            var cancellationToken = gameObject.GetCancellationTokenOnDestroy();
            var handler = OutputMediator.Unity.RegisterRequestHandler(action);
            handler.AddTo(cancellationToken);
            return handler;
        }
    }
}