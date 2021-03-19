namespace Razensoft.Mediator
{
    public interface IBaseRequest { }

    public interface IRequest<out TResponse> : IBaseRequest { }

    public interface IRequest : IRequest<Unit> { }

    public interface IAsyncRequest<out TResponse> : IBaseRequest { }

    public interface IAsyncRequest : IAsyncRequest<Unit> { }
}