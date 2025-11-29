using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DataAccess;
using DataAccess.Models;
using Domain.Enums;
using Logic;

namespace DatabasePopulator;

class Program
{
    [SuppressMessage("Globalization", "CA1303:Не передавать литералы в качестве локализованных параметров")]
    private static async Task Main()
    {
        Console.WriteLine("Start");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<Context>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

        await using var context = new Context(optionsBuilder.Options);

        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();

        await SeedDataPresetAsync(context);

        Console.WriteLine("End");
    }

    private static async Task SeedDataPresetAsync(Context context)
    {
        var hasher = new PasswordProvider();

        foreach (UserType userType in Enum.GetValues(typeof(UserType)))
        {
            var username = userType.ToString();
            var passwordHash = hasher.HashPassword(username);
            string email;
            if (userType == UserType.Admin)
            {
                email = "andrey.12.56.34@gmail.com";
            }
            else
                email = username + "@test.com";
            var user = new UserDb(username, passwordHash, email, userType);
            await context.Users.AddAsync(user);
        }

        await context.SaveChangesAsync();

        var persons = new[]
        {
            new PersonDb(Guid.NewGuid(), Sex.Male, "Алехандро", null),
            new PersonDb(Guid.NewGuid(), Sex.Male, "Сырник", null),
            new PersonDb(Guid.NewGuid(), Sex.Male, "Иван Иванович", null),
            new PersonDb(Guid.NewGuid(), Sex.Female, "Женщина Один", null),
            new PersonDb(Guid.NewGuid(), Sex.Female, "Женщина Два", null),
            new PersonDb(Guid.NewGuid(), Sex.Female, "Женщина Три", null),
        };

        await context.Persons.AddRangeAsync(persons);
        await context.SaveChangesAsync();

        var relationships = new[]
        {
            new RelationshipDb(persons[0].Id, persons[1].Id, RelationshipType.Friend), // Алехандро и Сырник - друзья
            new RelationshipDb(persons[2].Id, persons[3].Id, RelationshipType.Spouse), // Иван Иванович и Женщина Один - супруги
            new RelationshipDb(persons[1].Id, persons[4].Id, RelationshipType.Colleague), // Сырник и Женщина Два - коллеги
            new RelationshipDb(persons[3].Id, persons[4].Id, RelationshipType.Sibling), // Женщина Один и Женщина Два - сестры
        };

        var inverseRelationships = relationships
            .Select(r => new RelationshipDb(
                r.Person2Id,
                r.Person1Id,
                RelationshipHelper.GetInverseRelationship(r.Type)))
            .ToList();

        var allRelationships = relationships.Concat(inverseRelationships).ToList();

        await context.Relationships.AddRangeAsync(allRelationships);
        await context.SaveChangesAsync();
    }
}