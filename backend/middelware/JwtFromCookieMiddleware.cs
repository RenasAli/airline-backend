public class JwtFromCookieMiddleware
{
    private readonly RequestDelegate _next;

    public JwtFromCookieMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Cookies.ContainsKey("AuthToken"))
        {
            var token = context.Request.Cookies["AuthToken"];
            context.Request.Headers.Add("Authorization", "Bearer " + token);
        }

        await _next(context);
    }
}