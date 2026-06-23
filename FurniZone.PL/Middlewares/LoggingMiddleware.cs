namespace FurniZone.PL.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            _logger.LogInformation(
                "Request {Method} {Path} started at {Timestamp}",
                request.Method,
                request.Path,
                DateTime.UtcNow);

            await _next(context);

            stopwatch.Stop();

            var statusCode = context.Response.StatusCode;
            var level = statusCode >= 400 ? LogLevel.Warning : LogLevel.Information;

            _logger.Log(level,
                "Request {Method} {Path} completed with status {StatusCode} in {ElapsedMs}ms",
                request.Method,
                request.Path,
                statusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
