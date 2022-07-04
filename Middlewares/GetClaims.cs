using System.Globalization;
using System.Security.Claims;
using jwt_employee.Constants;

namespace jwt_employee.Middlewares
{
    public class GetClaimsMiddleware
    {

        private readonly RequestDelegate _next;

        public GetClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // var identity = context.User.Identity as ClaimsIdentity;
            var user_id = context.User.Claims.FirstOrDefault(x => x.Type.ToString().Equals(DbConstants.Claims_UserID,
                                                                                           StringComparison.InvariantCultureIgnoreCase));
            if (user_id != null)
            {
                context.Items[DbConstants.Claims_UserID] = user_id.Value;
            }
            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }

    }

    public static class GetClaimsMiddlewareExtensions
    {
        public static IApplicationBuilder GetClaims(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GetClaimsMiddleware>();
        }
    }
}
