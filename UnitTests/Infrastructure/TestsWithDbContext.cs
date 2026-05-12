using Microsoft.EntityFrameworkCore;
using Prictionary.Database;

namespace UnitTests.Infrastructure;

public abstract class TestsWithDbContext : IDisposable
{
    protected PrictionaryContext DbContext { get; }

    protected TestsWithDbContext()
    {
        var options = new DbContextOptionsBuilder<PrictionaryContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        DbContext = new PrictionaryContext(options);
        DbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        DbContext.Dispose();
    }
}
