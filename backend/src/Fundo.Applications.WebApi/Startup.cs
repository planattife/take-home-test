using Fundo.Application.Interfaces;
using Fundo.Application.Services;
using Fundo.Domain.Interfaces;
using Fundo.Infrastructure.Data;
using Fundo.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fundo.Applications.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<ILoanService, LoanService>();
            services.AddScoped<ILoanRepository, LoanRepository>();

            services.AddDbContext<LoanDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp",
                    policy =>
                    {
                        policy.WithOrigins(Configuration["CorsSettings:AngularAppUrl"])
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<LoanDbContext>();
                db.Database.Migrate();
            }

            app.UseRouting();
            app.UseCors("AllowAngularApp");
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
