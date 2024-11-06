using Microsoft.Extensions.Logging;
using MultiTenant;
using Serilog;

try
{
    Log.Information("Starting web host");

    var builder = WebApplication.CreateBuilder(args);
    Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerExtension();

    builder.Services.AddServiceRegister(builder.Configuration);

    builder.Host.UseSerilog();


    var app = builder.Build();
    await app.InitializeDatabaseAsync();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseSerilogRequestLogging();
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
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated");
}
finally
{
    Log.CloseAndFlush();
}

