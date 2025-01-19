using ProvidersMS.Core.Utils.Result;

namespace ProvidersMS.Core.Application.Services
{
    public interface IService<T, R>
    {
        Task<Result<R>> Execute(T data);
    }
}
