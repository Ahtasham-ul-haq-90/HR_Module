using Application.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HR_System.Extension.Filters
{
    public class ApiResponseFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
          
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if(context.Result is ObjectResult objectResult)
            {
                var response = new APIResponse<object>
                {
                    Success = objectResult.StatusCode >= 200 && objectResult.StatusCode < 300,
                    Message = objectResult.StatusCode == 201 ? "Resource created successfully" : "Request processed successfully",
                    Data = objectResult.Value,
                    Errors = null,
                    StatusCode = objectResult.StatusCode ?? 200,
                    Timestamp = DateTime.UtcNow
                };
                context.Result = new ObjectResult(response) { 
                StatusCode = response.StatusCode,
                };
            }
        }
    }
}
