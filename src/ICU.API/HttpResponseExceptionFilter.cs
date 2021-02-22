
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ICU.API
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; set; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is HttpResponseException exception && exception.Status != System.Net.HttpStatusCode.InternalServerError)
            {
                context.Result = new ObjectResult(exception.Value)
                {
                    StatusCode = (int?)exception.Status,
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
