using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Assessment.Group.Phase.Middleware
{
    public interface IExceptionHandlerMiddleware
    {
        Task InvokeAsync(HttpContext context);
    }
}
