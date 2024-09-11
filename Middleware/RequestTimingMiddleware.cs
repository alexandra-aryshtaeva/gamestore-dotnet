using System.Diagnostics;
namespace GameStore.Middleware;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate Next;
    private readonly ILogger<RequestTimingMiddleware> Logger;

    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        Next = next;
        Logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Measure any of the requests that will come across the request pipeline in milliseconds
        var stopwatch = Stopwatch.StartNew();
            try
            {
                stopwatch.Start();
                await Next(context);

            }
            finally 
            {
                stopwatch.Stop();
                var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        
                Logger.LogInformation("{RequestMethod} {RequestPath} request took {elapsedMilliseconds}ms to complete",
                    context.Request.Method,
                    context.Request.Path,
                    elapsedMilliseconds);
            }
            
    }
     
       
}