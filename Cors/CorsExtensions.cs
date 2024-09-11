namespace GameStore.Cors;

public static class CorsExtensions
{
    private const string allowedOriginSetting = "AllowedOrigin";
    
    public static IServiceCollection AddGameStoreCors(this IServiceCollection services, IConfiguration configuration) 
    {
        return services.AddCors(options => 
            {
                options.AddDefaultPolicy(corsBuilder =>
                 {
                     //if its null throw an exception
                     var allowedOrigin = configuration["AllowedOrigin"] ?? 
                                         throw new InvalidOperationException("AllowedOrigin cannot be set"); 
                     corsBuilder
                    .WithOrigins(allowedOrigin)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders("X-Pagination");
                 });
            });
    }
}