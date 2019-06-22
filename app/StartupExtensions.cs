using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
        {
            return services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("oauth2", new Swashbuckle.AspNetCore.Swagger.ApiKeyScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = "header",
                    Name = "Authorization",
                    Type = "apiKey"
                });

                c.OperationFilter<Swashbuckle.AspNetCore.Filters.SecurityRequirementsOperationFilter>();

                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "api-template",
                    Version = "v1",
                    Description = "API Template",
                    TermsOfService = "None",
                    Contact = new Swashbuckle.AspNetCore.Swagger.Contact
                    {
                        Name = "ihopethisnamecounts",
                        Email = string.Empty,
                        Url = "https://github.com/ihtnc/api-template"
                    }
                });
            });
        }

        public static IApplicationBuilder UseApiDocumentation(this IApplicationBuilder app)
        {
            return app.UseSwagger()
               .UseSwaggerUI(c => { c.SwaggerEndpoint($"/swagger/v1/swagger.json", $"Api Template V1"); });
        }
    }
}