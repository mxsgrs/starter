﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;

namespace Starter.WebApi.IntegrationTests.Facts.Factories;

public class StarterWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder().Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<StarterContext>));

            string connectionString = _dbContainer.GetConnectionString();
            services.AddDbContext<StarterContext>(options => options
                .UseSqlServer(connectionString));
        });
    }

    public HttpClient CreateAuthorizedClient()
    {
        HttpClient httpClient = CreateClient();

        // Expiration date is around 2034
        string jwt = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJiM2UzZjAyYS0xNzU2LTQ1NzUtODNjZS04MGI2NjE2M2QzYmUiLCJzdWIiOiIxIiwiZW1haWwiOiJqb2huLmRvZUBnbWFpbC5jb20iLCJhdWQiOiJodHRwczovL3N0YXJ0ZXJ3ZWJhcGkuY29tIiwiaXNzIjoiaHR0cHM6Ly9zdGFydGVyd2ViYXBpLmNvbSIsImV4cCI6MjAzOTI3MDA1Mn0.aeUd-y_mUKKEXSLh4JQrXV7fRw2oqcAPmcrjnXfYxpeV1f6afMCSCPrIlUJ-v8fJg4TX-r8zBQK9yyyIFTo4BA";
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwt);

        return httpClient;
    }

    public StarterContext MigrateDbContext()
    {
        IServiceScope scope = Services.CreateScope();
        StarterContext dbContext = scope.ServiceProvider.GetRequiredService<StarterContext>();
        dbContext.Database.Migrate();

        return dbContext;
    }

    public Task InitializeAsync()
        => _dbContainer.StartAsync();

    public new Task DisposeAsync()
        => _dbContainer.DisposeAsync().AsTask();
}

public static class JsonOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true
    };
}