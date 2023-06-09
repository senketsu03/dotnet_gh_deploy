using Microsoft.EntityFrameworkCore;

namespace Deploy.Tests;

public class ContextTests
{
    [SetUp]
    public void Setup()
    { }

    [Test]
    public void EmptyConstructorTest()
    {
        var db = new Context();

        Assert.IsNotNull(db);

        db.Dispose();
    }

    [Test]
    public void PropertiesTest()
    {
        // ctor and setters
        var db = new Context();

        // users
        var mockUsers = new Mock<DbSet<User>>();
        db.Users = mockUsers.Object;
        Assert.NotNull(db.Users);

        // companies
        var mockCompanies = new Mock<DbSet<Company>>();
        db.Companies = mockCompanies.Object;
        Assert.NotNull(db.Companies);

        db.Dispose();
    }

    [Test]
    public void OnConfiguringTest()
    {
        // ctor and setters
        var db = new Context();

        // users
        var mockUsers = new Mock<DbSet<User>>();
        db.Users = mockUsers.Object;
        Assert.NotNull(db.Users);

        // companies
        var mockCompanies = new Mock<DbSet<Company>>();
        db.Companies = mockCompanies.Object;
        Assert.NotNull(db.Companies);

        Assert.DoesNotThrow(() => db.SaveChanges());

        db.Dispose();
    }
}
