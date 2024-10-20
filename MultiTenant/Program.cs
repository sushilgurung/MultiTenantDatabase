var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerExtension();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddServiceDiscovery();
builder.Services.AddPersistenceInfrastructure(builder.Configuration);

var app = builder.Build();
await app.InitializeDatabaseAsync();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<TenantMiddleware>();
using (var scope = app.Services.CreateScope())
{
    var migrationService = scope.ServiceProvider.GetRequiredService<TenantMigrationService>();
    await migrationService.MigrateAllTenantsAsync();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
//app.MapRoutes();
app.MapCarter();
app.Run();
