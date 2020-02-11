using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Pluralsight.RewardsApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthentication("bearer")
                .AddIdentityServerAuthentication("bearer", options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.ApiName = "wiredbrain_api";
                    options.ApiSecret = "apisecret";
                    options.RequireHttpsMetadata = false; // DEV only!!
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
