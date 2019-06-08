using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Autofac;
using Autofac.Extras.DynamicProxy;
using Autofac.Extensions.DependencyInjection;

using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole;

namespace XService.Driver
{
  public class Program
    {
        private static string _InputFolder;
        private static string _OutputFolder;

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.UTF8Encoding.UTF8;

            var hostBuilder = new HostBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                
                .ConfigureContainer<ContainerBuilder>(ConfigureContainer())
                .ConfigureHostConfiguration(ConfigureHostConfiguration(args))
                .ConfigureAppConfiguration(ConfigureAppConfiguration())
                .ConfigureLogging(ConfigureLogging());
            // return the host builder task
            hostBuilder.RunConsoleAsync();
        }

    /// <summary>
    /// Configure Logging
    /// </summary>
    /// <returns></returns>
    private static Action<ILoggingBuilder> ConfigureLogging()
    {
      return loggingBuilder =>
      {
        var logger = new LoggerConfiguration()
                    .Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name)
                    .Enrich.WithProperty("AppDomain", AppDomain.CurrentDomain)
                    .Enrich.WithProperty("RuntimeVersion", Environment.Version)
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message} {NewLine}{Exception}")
                    .CreateLogger();
        loggingBuilder
            .AddSerilog(logger:logger, dispose: true);

        logger.Information("Application Name: {ApplicationName}");
        logger.Information("App Domain      : {AppDomain}");
        logger.Information("Runtime Version : {RuntimeVersion}");
      };
    }

    /// <summary>
    /// Configures the container
    /// </summary>
    /// <returns></returns>
    private static Action<HostBuilderContext, ContainerBuilder> ConfigureContainer()
        {
            return (context, builder) =>
            {
                builder.RegisterType<Business.Rules.SampleRule>()
                    .As<Business.Rules.IRule>()
                    .EnableInterfaceInterceptors()
                    .InterceptedBy(typeof(Aspects.LoggingAspect));

                // Register interceptors
                builder.RegisterType<XService.Driver.Aspects.LoggingAspect>();

                // Registers the BusinessEngine an IHostedService
                builder.RegisterType<XService.Business.BusinessEngine>()
                    .As<IHostedService>();
            };
        }

        /// <summary>
        /// Configures the Application
        /// </summary>
        /// <returns></returns>
        private static Action<HostBuilderContext, IConfigurationBuilder> ConfigureAppConfiguration()
        {
            return (context, builder) =>
            {
                // Pass the setup from the host to the app
                var env = context.HostingEnvironment;
                builder.AddEnvironmentVariables();
                var config = builder.Build();

                // Read the configuration
                _InputFolder = config.GetValue<string>("InputFolder");
                _OutputFolder = config.GetValue<string>("OutputFolder");

            };
        }

        /// <summary>
        /// Configure the Host
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Action<IConfigurationBuilder> ConfigureHostConfiguration(string[] args)
        {
            return config =>
            {
                // Setup Environment Variable Configuration
                config.AddEnvironmentVariables();

                // Add & Overwrite any command line configs
                // e.g. --Config "Value"
                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            };
        }
    }
}
