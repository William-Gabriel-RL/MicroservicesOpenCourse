using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataService.Grpc;
using PlatformService.SyncDataService.Http;

var builder = WebApplication.CreateBuilder(args);

// WG --> This set the actual Database used

if(builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using SqlServer Db");
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}
else
{
    Console.WriteLine("--> Using InMem Db");
    Console.WriteLine(builder.Configuration.GetConnectionString("PlatformsConn"));
    builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));
}

// WG --> This register the Dependency Injection -- IMPORTANT --
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

// Singleton: Created 1st time requested, subsequent requests use the same instance
// Scoped: Same within a erquest but created for every new request
// Transient: New Instance provided everytime, never the same/reused
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddGrpc();
// WG --> End of DI


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// WG --> To Start PrepDb
PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

// WG --> Debug to see if the command service endpoint still alright
Console.WriteLine($"--> Command Service Endpoint {app.Configuration["CommandService"]}");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/protos/platforms.proto", async context => {
    await context.Response.WriteAsync(System.IO.File.ReadAllText("Protos/platforms.proto"));
});

app.Run();
