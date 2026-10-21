
var builder = DistributedApplication.CreateBuilder(args);

var connectionString = builder.AddConnectionString("BaseConnection");


builder.AddProject<Projects.Web>(name:"WebAPI")
    .WithReference(connectionString);

builder.Build().Run();
