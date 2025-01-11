var builder = DistributedApplication.CreateBuilder(args);

var startersql = builder.AddSqlServer("startersql");
var startersqldb = startersql.AddDatabase("startersqldb");

builder.AddProject<Projects.Starter_WebApi>("starter-webapi").WithReference(startersqldb);

builder.AddProject<Projects.Starter_MockWebApi>("starter-mockwebapi");

builder.Build().Run();
