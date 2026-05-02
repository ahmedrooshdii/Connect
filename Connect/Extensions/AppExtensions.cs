using Connect.Options;

namespace Connect.Extensions
{
    public static class AppExtensions
    {
        public static void AddOptions(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddOptions<Jwt>()
                .Bind(builder.Configuration.GetSection("JWT"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
    }
}
