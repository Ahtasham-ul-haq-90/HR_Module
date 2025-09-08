using Application.DTO;

namespace HR_System.Extension.MiddleWare
{
    public class ExceptionMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleWare> _logger;
        public ExceptionMiddleWare(RequestDelegate next, ILogger<ExceptionMiddleWare> logger)
        {
                _logger = logger;
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex,ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var response = new APIResponse<object>
                {
                    Success = false,
                    Message = "An Unexpected error occurred.",
                    Data = null,
                    Errors = ex.Message,
                    StatusCode = context.Response.StatusCode,
                    Timestamp = DateTime.Now,
                };
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
