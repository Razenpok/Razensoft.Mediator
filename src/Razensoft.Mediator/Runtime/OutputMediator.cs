using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Razensoft.Mediator
{
    public class OutputMediator : IOutputMediator
    {
        public static OutputMediator Unity { get; set; }

        private readonly IMessageBus _asyncMessageBus;

        public OutputMediator(IMessageBus asyncMessageBus)
        {
            _asyncMessageBus = asyncMessageBus;
        }

        public void Publish<TNotification>(TNotification notification) where TNotification : INotification
        {
            SubscriptionTracker<TNotification>.ThrowIfNoSubscribers();
            _asyncMessageBus.Publish(notification);
        }

        public void Send<TRequest>(TRequest request) where TRequest : IRequest
        {
            SubscriptionTracker<TRequest>.ThrowIfNoSubscribers();
            _asyncMessageBus.Publish(request);
        }

        public UniTask AsyncSend<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IAsyncRequest
        {
            SubscriptionTracker<TRequest>.ThrowIfNoSubscribers();
            return _asyncMessageBus.PublishAsync(request, cancellationToken);
        }

        public void ForgetSend<TRequest>(TRequest request, CancellationToken cancellationToken = default)
            where TRequest : IAsyncRequest
        {
            SubscriptionTracker<TRequest>.ThrowIfNoSubscribers();
            _asyncMessageBus.PublishAsync(request, cancellationToken).Forget();
        }

        public IDisposable RegisterNotificationHandler<TNotification>(Action<TNotification> handler)
            where TNotification : INotification
        {
            var subscription = _asyncMessageBus.Subscribe(handler);
            return SubscriptionTracker<TNotification>.Track(subscription);
        }

        public IDisposable RegisterRequestHandler<TRequest>(Action<TRequest> handler)
            where TRequest : IRequest
        {
            SubscriptionTracker<TRequest>.ThrowIfHasSubscribers();
            var subscription = _asyncMessageBus.Subscribe(handler);
            return SubscriptionTracker<TRequest>.Track(subscription);
        }

        public IDisposable RegisterRequestHandler<TRequest>(
            Func<TRequest, CancellationToken, UniTask> handler,
            CancellationToken cancellationToken = default)
            where TRequest : IAsyncRequest
        {
            SubscriptionTracker<TRequest>.ThrowIfHasSubscribers();
            var subscription = _asyncMessageBus.Subscribe<TRequest>(
                (msg, cancellationToken2) => Handle(handler, msg, cancellationToken, cancellationToken2)
            );
            return SubscriptionTracker<TRequest>.Track(subscription);
        }

        private static async UniTask Handle<TRequest>(
            Func<TRequest, CancellationToken, UniTask> handler,
            TRequest message,
            CancellationToken cancellationToken,
            CancellationToken cancellationToken2)
            where TRequest : IAsyncRequest
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cancellationToken2);
            await handler(message, linkedTokenSource.Token);
            linkedTokenSource.Cancel();
            linkedTokenSource.Dispose();
        }

        private class SubscriptionTracker<T> : IDisposable
        {
            private readonly IDisposable _subscription;
            private bool _isDisposed;

            private SubscriptionTracker(IDisposable subscription)
            {
                _subscription = subscription;
            }

            public void Dispose()
            {
                if (_isDisposed)
                {
                    return;
                }

                _subscription.Dispose();
                SubscriptionCount--;
                _isDisposed = true;
            }

            // ReSharper disable once StaticMemberInGenericType
            public static int SubscriptionCount { get; private set; }

            public static void ThrowIfNoSubscribers()
            {
                if (SubscriptionCount == 0)
                {
                    throw new InvalidOperationException(
                        $"No handlers registered for handling {typeof(T).FullName}"
                    );
                }
            }

            public static void ThrowIfHasSubscribers()
            {
                if (SubscriptionCount != 0)
                {
                    throw new InvalidOperationException(
                        $"Only one handler can be registered for handling {typeof(T).FullName}"
                    );
                }
            }

            public static IDisposable Track(IDisposable subscription)
            {
                SubscriptionCount++;
                return new SubscriptionTracker<T>(subscription);
            }
        }
    }
}