using hqm_ranked_backend.Helpers;
using MassTransit;
using Microsoft.Extensions.Localization;
using Serilog;
using System.Net;
using System.Text.Json;

namespace hqm_ranked_backend.Common
{
    public class ExceptionMiddleware : IMiddleware
    {
        public readonly ILogger<ExceptionMiddleware>? _logger;

        public ExceptionMiddleware(ILoggerFactory factory)
        {
            _logger = factory.CreateLogger<ExceptionMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                string errorId = Guid.NewGuid().ToString();
                var errorResult = new ErrorResult()
                {
                    Messages = new(),
                    Source = exception.TargetSite?.DeclaringType?.FullName,
                    Exception = exception.Message.Trim(),
                    ErrorId = errorId,
                    Error = exception.Message.Trim(),
                    StackTrace = exception.StackTrace
                };
                errorResult.Messages.Add(exception.Message);

                switch (exception)
                {
                    case KeyNotFoundException:
                        errorResult.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        errorResult.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var log = LogHelper.GetErrorLog(errorResult.Exception, exception.StackTrace);
                Log.Error(log);

                var response = context.Response;
                if (!response.HasStarted)
                {
                    response.ContentType = "application/json";
                    response.StatusCode = errorResult.StatusCode;
                    await response.WriteAsync(JsonSerializer.Serialize(errorResult));
                }
            }
        }
    }
}
