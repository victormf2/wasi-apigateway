using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WasiApiGateway.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WasiApiGateway
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<ApiHandlersContainer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                var apiHandlersContainer = context.RequestServices.GetService<ApiHandlersContainer>();
                var apiHandler = apiHandlersContainer.GetApiHandler(context.Request.Path, context.Request.Method);
                if (apiHandler != null)
                {
                    context.SetEndpoint(new Endpoint(async context =>
                    {
                        var applicationRequest = await context.Request.AsApplicationRequest(context.RequestAborted);

                        var response = await apiHandler.Handle(applicationRequest, context.RequestAborted);

                        if (response.Headers != null) {
                            foreach (var entry in response.Headers) {
                                context.Response.Headers[entry.Key] = entry.Value;
                            }
                        }

                        await context.Response.Body.WriteAsync(response.Body, context.RequestAborted);
                    }, null, null));
                }

                await next();
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
