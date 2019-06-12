using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Autofac;
using Autofac.Extras.DynamicProxy;
using Autofac.Extensions.DependencyInjection;

using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Settings.Configuration;
using Serilog.Sinks.File;
using Serilog.Sinks.SystemConsole;

using XService.Enterprise.Aspects;
using XService.Enterprise.Contracts;
using XService.Enterprise.Providers;

namespace XService.Driver {
    public class Program {

        private static IConfigurationRoot _Configuration;
        public static void Main(string[] args) {
            Console.OutputEncoding = System.Text.UTF8Encoding.UTF8;

            var hostBuilder = new HostBuilder()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(ConfigureContainer())
                .ConfigureHostConfiguration(ConfigureHostConfiguration(args))
                .ConfigureAppConfiguration(ConfigureAppConfiguration())
                .ConfigureLogging(ConfigureLogging());
            // return the hostBuilder task and set the Main to Task to run as a continuous service
            hostBuilder.RunConsoleAsync();
        }

        /// <summary>
        /// Configure Logging
        /// </summary>
        /// <returns></returns>
        private static Action<ILoggingBuilder> ConfigureLogging() {
            return loggingBuilder => {
                var logger = new LoggerConfiguration()
                            .Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name)
                            .Enrich.WithProperty("RuntimeVersion", Environment.Version)
                            .ReadFrom.Configuration(_Configuration)
                            .CreateLogger();

                loggingBuilder
                    .AddSerilog(logger: logger, dispose: true);

                logger.Information("".PadRight(80, '*'));
                logger.Information($"Machine Name         : {Environment.MachineName}");
                logger.Information($"OS Name and Version  : {System.Runtime.InteropServices.RuntimeInformation.OSDescription}");
                logger.Information($"OS Architecture      : {System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}");
                logger.Information($"Process Architecture : {System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture}");
                logger.Information($"Processors           : {Environment.ProcessorCount}");
                logger.Information($"User                 : {Environment.UserDomainName}\\{Environment.UserName}");
                logger.Information("Application Name     : {ApplicationName}");
                logger.Information("Runtime Version      : {RuntimeVersion}");
                logger.Information("".PadRight(80, '*'));
            };
        }

        /// <summary>
        /// Configures the container
        /// </summary>
        /// <returns></returns>
        private static Action<HostBuilderContext, ContainerBuilder> ConfigureContainer() {
            return (context, builder) => {
                builder.RegisterType<Business.Rules.SampleRule>()
                    .As<Business.Rules.IRule>()
                    .EnableInterfaceInterceptors()
                    .InterceptedBy(typeof(LoggingInterceptor));

                builder.RegisterType<MemoryCacheProvider>()
                    .As<ICacheProvider>()
                    .SingleInstance();

                // Register interceptors
                builder.RegisterType<LoggingInterceptor>();
                builder.RegisterType<CachingInterceptor>()
                    .SingleInstance();

                // Registers the BusinessEngine an IHostedService
                builder.RegisterType<XService.Business.BusinessEngine>()
                    .As<IHostedService>();

                // Register any exspensive data operations with caching
                // builder.RegisterType<SomeExpensiveClass>()
                //     .As<IExpensiveOperation>()
                //     .EnableInterfaceInterceptors()
                //     .InterceptedBy(typeof(CachingAspect))
                //     .SingleInstance();
            };
        }

        /// <summary>
        /// Configures the Application
        /// </summary>
        /// <returns></returns>
        private static Action<HostBuilderContext, IConfigurationBuilder> ConfigureAppConfiguration() {
            return (context, builder) => {
                // Pass the environment from the host to the app
                var env = context.HostingEnvironment;
                builder
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile(
                        path: "appsettings.json",
                        optional: true,
                        reloadOnChange: true)
                    .AddJsonFile(
                        path: $"appsettings.{env.EnvironmentName}.json",
                        optional: true,
                        reloadOnChange: true)
                    .AddEnvironmentVariables();
                _Configuration = builder.Build();
            };
        }

        /// <summary>
        /// Configure the Host
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Action<IConfigurationBuilder> ConfigureHostConfiguration(string[] args) {
            return config => {
                // Setup Environment Variable Configuration
                config.AddEnvironmentVariables();

                // Add & Overwrite any command line configs
                // e.g. --Config "Value"
                if (args != null) {
                    config.AddCommandLine(args);
                }
            };
        }
    }
}
