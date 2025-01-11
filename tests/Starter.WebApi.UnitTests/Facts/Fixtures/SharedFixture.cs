﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Starter.Application.Interfaces;
using Starter.Domain.Authentication;
using Starter.Infrastructure.Persistance;

namespace Starter.WebApi.UnitTests.Facts.Fixtures;

public class SharedFixture
{
    public readonly IConfigurationRoot Configuration;
    public readonly IAppContextAccessor AppContextAccessor;

    public SharedFixture()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").Build();

        AppContextAccessor = new MockContextAccessor();
    }

    private class MockContextAccessor : IAppContextAccessor
    {
        public UserClaims UserClaims { get; set; } = new()
        {
            UserId = Guid.NewGuid()
        };
    }

    public static StarterDbContext CreateDatabaseContext()
    {
        DbContextOptions<StarterDbContext> options = new DbContextOptionsBuilder<StarterDbContext>()
            .ConfigureWarnings(warning => warning.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new StarterDbContext(options);
    }

    public static string ReadLocalJson(string relativePath)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string path = $"{currentDirectory}{relativePath}";
        string json = "";

        if (File.Exists(path))
        {
            json = File.ReadAllText(path);
        }

        return json;
    }
}
