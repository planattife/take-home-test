using Fundo.Infrastructure.Data;

namespace Fundo.Services.Tests.Integration
{
    public static class TestDBUtilities
    {
        public static void ResetDatabase(LoanDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}
