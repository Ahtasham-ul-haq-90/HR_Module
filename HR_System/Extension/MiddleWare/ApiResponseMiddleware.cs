using System.Net;
using System.Text.Json;
using Application.DTO;

namespace HR_System.Extension.MiddleWare
{
    public class ApiResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            using var newBodyStream = new MemoryStream();
            context.Response.Body = newBodyStream;

            try
            {
                // Continue down the pipeline
                await _next(context);

                // Read response
                newBodyStream.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(newBodyStream).ReadToEndAsync();
                newBodyStream.Seek(0, SeekOrigin.Begin);

                // Wrap into APIResponse
                var apiResponse = new APIResponse<object>
                {
                    Success = context.Response.StatusCode >= 200 && context.Response.StatusCode < 300,
                    Message = context.Response.StatusCode >= 200 && context.Response.StatusCode < 300
                              ? "Request successful"
                              : "An error occurred",
                    Data = string.IsNullOrWhiteSpace(responseBody) ? null : TryParseJson(responseBody),
                    Errors = null,
                    StatusCode = context.Response.StatusCode,
                    Timestamp = DateTime.UtcNow
                };

                // Restore original body before writing
                context.Response.Body = originalBodyStream;

                context.Response.ContentType = "application/json";
                var json = JsonSerializer.Serialize(apiResponse);
                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                context.Response.Body = originalBodyStream; // restore
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var apiResponse = new APIResponse<object>
            {
                Success = false,
                Message = "An unhandled error occurred",
                Data = null,
                Errors = ex.Message,
                StatusCode = context.Response.StatusCode,
                Timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(apiResponse);
            await context.Response.WriteAsync(json);
        }

        private static object? TryParseJson(string str)
        {
            try
            {
                return JsonSerializer.Deserialize<object>(str);
            }
            catch
            {
                return str; // if not JSON, return as string
            }
        }
    }
}
