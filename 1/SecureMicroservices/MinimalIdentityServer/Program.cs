using MinimalIdentityServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityServer()
                    .AddInMemoryClients(Config.Clients)
                    .AddInMemoryApiScopes(Config.ApiScopes)
                    .AddDeveloperSigningCredential();

var app = builder.Build();
app.UseIdentityServer();
app.Run();

