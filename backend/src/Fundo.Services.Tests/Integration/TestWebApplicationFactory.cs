using Fundo.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Fundo.Services.Tests.Integration
{
    public class TestWebApplicationFactory : WebApplicationFactory<Fundo.Applications.WebApi.Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<LoanDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<LoanDbContext>(options =>
                {
                    options.UseInMemoryDatabase("LoanTestDB");
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<LoanDbContext>();

                    TestDBUtilities.ResetDatabase(db);
                }
            });
        }
    }
}
