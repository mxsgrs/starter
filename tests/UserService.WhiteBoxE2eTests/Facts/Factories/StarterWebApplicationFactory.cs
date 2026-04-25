using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using UserService.Infrastructure.Messaging;

namespace UserService.WhiteBoxE2eTests.Facts.Factories;

public class StarterWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04")
        .Build();

    private const string _rabbitMqUsername = "guest";
    private const string _rabbitMqPassword = "1MRt58rcy4NtfpcMcxsMJb";
    private const int _rabbitMqContainerPort = 5672;
    private const int _rabbitMqHostPort = 5673;

    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder("rabbitmq:3.11")
        .WithUsername(_rabbitMqUsername)
        .WithPassword(_rabbitMqPassword)
        .WithPortBinding(_rabbitMqHostPort, _rabbitMqContainerPort)
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<UserDbContext>>();

            string connectionString = _dbContainer.GetConnectionString();
            services.AddDbContext<UserDbContext>((sp, options) =>
            {
                options.UseSqlServer(connectionString);
                options.AddInterceptors(sp.GetRequiredService<DomainEventInterceptor>());
            });

            services.RemoveAll<ICheckUserAddressService>();
            services.AddScoped<ICheckUserAddressService, AlwaysValidAddressService>();
        });

        builder.ConfigureAppConfiguration((hostingContext, config) =>
        {
            string rabbitMqConnectionString = $"amqp://{_rabbitMqUsername}:{_rabbitMqPassword}@{_rabbitMqContainer.Hostname}:{_rabbitMqHostPort}";

            IEnumerable<KeyValuePair<string, string?>> settings = 
            [
                new("ConnectionStrings:RabbitMq", rabbitMqConnectionString)
            ];

            config.AddInMemoryCollection(settings);
        });
    }

    public HttpClient CreateAuthorizedClient()
    {
        HttpClient httpClient = CreateClient();

        // Expiration date is around 2034
        string jwt = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJodHRwczovL3N0YXJ0ZXJ3ZWJhcGkuY29tIiwiaXNzIjoiaHR0cHM6Ly9zdGFydGVyd2ViYXBpLmNvbSIsImV4cCI6MjA0NTUwNzU1NiwianRpIjoiN2FkYjM1M2ItMWMyMC00NGVmLWI5MjMtMDU3Mzg2ZTdhZWMyIiwic3ViIjoiNWYxYzNmMTMtZDhlZS00NGMzLTIzNjktMDhkY2Y1ZmQ1MmIxIiwiZW1haWwiOiJqb2huLmRvZUBleGFtcGxlLmNvbSJ9.0DqU6uyHcOw8mZm8CXXwv6t7PRDajU2kwqJrMMCJTEVor8WPs1iOz9Vq-2KZ6Ew3-uKmrEhdG7K2woDokBf-Rg";
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwt);

        return httpClient;
    }

    public UserDbContext MigrateDbContext()
    {
        IServiceScope scope = Services.CreateScope();
        UserDbContext dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
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