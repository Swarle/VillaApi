using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace PLL.Infrastructure
{
    public static class ServiceConfigurationSerilog
    {
        public static void ConfigureSerilog(this ILoggingBuilder logging)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo
                .Console(outputTemplate: "{Timestamp:HH:mm:ss}  [{Level:}] {SourceContext}  \n{Message:l}\n{Exception}",
                    theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            logging.AddSerilog(logger);
        }
    }
}
