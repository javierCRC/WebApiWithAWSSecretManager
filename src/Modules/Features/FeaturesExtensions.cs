using Microsoft.AspNetCore.Mvc;

namespace WebApiHandsOn.Modules.Features
{
    public static class FeaturesExtensions
    {
        public static IServiceCollection AddFeature(this IServiceCollection services, IConfiguration configuration)
        {
            string myApiPolicy = "WebApiHandsOnCORSPolicy";

            services.AddCors(options => options.AddPolicy(myApiPolicy, body => body.WithOrigins(configuration["Config:OriginCors"].Split(",")) //body.WithOrigins("*") 
                .AllowAnyHeader()
                .AllowAnyMethod())
            );
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);

            return services;
        }// end method AddFeature
    }
}
