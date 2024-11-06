using Serilog;

namespace MultiTenant
{
    public static class ApplicationExtension
    {
        public static void ConfigureSerilog(this IHostBuilder host)
        {
            host.UseSerilog((ctx, lc) =>
            {
                lc.WriteTo.Console();
            });
        }
    }
}
