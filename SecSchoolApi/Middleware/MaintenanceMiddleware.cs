using Microsoft.AspNetCore.Authorization;
using SecSchoolApi.Interface;

namespace SecSchoolApi.Middleware
{
    public class MaintenanceMiddleware
    {
        private readonly RequestDelegate _next;
        public MaintenanceMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context, IAppControlService control)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
            if (path.StartsWith("/swagger") || path.StartsWith("/api/system/health") || path.StartsWith("/api/auth/"))
            {
                await _next(context);
                return;
            }

            var isMaintenance = await control.GetMaintenanceAsync(context.RequestAborted);
            if (isMaintenance && !(context.User?.IsInRole("Admin") ?? false))
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsJsonAsync(new { error = "Service is under maintenance" }, context.RequestAborted);
                return;
            }

            await _next(context);
        }
    }
}
