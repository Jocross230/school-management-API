using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SecSchoolApi.Model;

namespace SecSchoolApi.Filters
{
    public class ResponseEnvelopeFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var req = context.HttpContext.Request;
            var method = req.Method?.ToUpperInvariant() ?? "GET";

            // Helper to infer default message
            string DefaultMessage(int? status)
            {
                var code = status ?? 200;
                if (code >= 400) return code == 404 ? "Not found" : "Request failed";
                return method switch
                {
                    "POST" => "Created successfully",
                    "PUT" => "Updated successfully",
                    "PATCH" => "Updated successfully",
                    "DELETE" => "Deleted successfully",
                    _ => "Success"
                };
            }

            // Already wrapped?
            if (context.Result is ObjectResult obj1 && obj1.Value is ApiResponse)
            {
                await next();
                return;
            }

            switch (context.Result)
            {
                case ObjectResult obj:
                    var status = obj.StatusCode ?? (obj is OkObjectResult ? 200 : 200);
                    string msg = DefaultMessage(status);
                    // If the value is a string and status >=400, use it as message
                    if (obj.Value is string s && status >= 400)
                    {
                        msg = s;
                        obj.Value = null;
                    }
                    obj.Value = new ApiResponse { Success = status < 400, Message = msg, Data = obj.Value };
                    obj.StatusCode = status;
                    break;

                case StatusCodeResult scr:
                    var code = scr.StatusCode;
                    var outCode = code == 204 ? 200 : code; // convert 204 to 200 so we can include a body
                    context.Result = new ObjectResult(new ApiResponse
                    {
                        Success = code < 400,
                        Message = DefaultMessage(code),
                        Data = null
                    }) { StatusCode = outCode };
                    break;

                case EmptyResult:
                    context.Result = new ObjectResult(new ApiResponse
                    {
                        Success = true,
                        Message = DefaultMessage(200),
                        Data = null
                    }) { StatusCode = 200 };
                    break;

                case ContentResult cr:
                    var contentStatus = cr.StatusCode ?? 200;
                    context.Result = new ObjectResult(new ApiResponse
                    {
                        Success = contentStatus < 400,
                        Message = contentStatus < 400 ? DefaultMessage(contentStatus) : cr.Content ?? DefaultMessage(contentStatus),
                        Data = contentStatus < 400 ? cr.Content : null
                    }) { StatusCode = contentStatus };
                    break;
            }

            await next();
        }
    }
}
