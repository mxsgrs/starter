var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Starter_WebApi>("starter-webapi");

builder.AddProject<Projects.Starter_MockWebApi>("starter-mockwebapi");

builder.Build().Run();
