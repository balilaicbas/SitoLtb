using Microsoft.AspNetCore.Diagnostics;

namespace SitoLtb.Utilities
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Eccezione non gestita durante l'elaborazione della richiesta {Path}", httpContext.Request.Path);

            httpContext.Response.Redirect("/Home/Error");
            return ValueTask.FromResult(true);
        }
    }
}
