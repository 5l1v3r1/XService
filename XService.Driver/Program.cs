using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Autofac;
using Autofac.Extras.DynamicProxy;
using Autofac.Extensions.DependencyInjection;

using Serilog;

using XService.Enterprise.Aspects;
using XService.Enterprise.Contracts;
using XService.Enterprise.Providers;

namespace XService.Driver {

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Program {

        private static IConfigurationRoot _Configuration;
        public static void Main(string[] args) {
            // static async Task ...  await hostBuilder.RunConsoleAsync() for service daemon
            Environment.ExitCode = DriverExitCodes.Success;

            // This can be changed to support emojis on the cli -- 99.9% you don't need a rocket emoji unless something blows up
            Console.OutputEncoding = System.Text.UTF8Encoding.UTF8;
            var hostBuilder = new HostBuilder();

            try {
                hostBuilder
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .ConfigureContainer(ConfigureContainer())
                    .ConfigureHostConfiguration(ConfigureHostConfiguration(args))
                    .ConfigureAppConfiguration(ConfigureAppConfiguration())
                    .ConfigureLogging(ConfigureLogging())
                    .ConfigureServices(ConfigureServices());
            } catch( Exception ) {
                Environment.ExitCode = DriverExitCodes.DriverConfigurationFault;
                throw;
            }

            hostBuilder.RunConsoleAsync();
        }

        /// <summary>
        /// Configure services hook
        /// </summary>
        /// <returns></returns>
        private static Action<HostBuilderContext, IServiceCollection> ConfigureServices() {
            return (context, services) => {
                services.Configure<ConsoleLifetimeOptions>(options => {
                    options.SuppressStatusMessages = true;
                });
            };
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
                logger.Information($"Runtime Version      : {Environment.Version}");
                logger.Information($"User                 : {Environment.UserDomainName}\\{Environment.UserName}");
                logger.Information($"Application Name     : {typeof(Program).Assembly.GetName().Name}");
                logger.Information($"Application Version  : {typeof(Program).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version}");
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
                    .InterceptedBy(typeof(LoggingInterceptor))
                    .InterceptedBy(typeof(ModelValidatorInterceptor));

                builder.RegisterType<MemoryCacheProvider>()
                    .As<ICacheProvider>()
                    .SingleInstance();

                // Register interceptors
                builder.RegisterType<LoggingInterceptor>();
                builder.RegisterType<CachingInterceptor>()
                    .SingleInstance();
                builder.RegisterType<ModelValidatorInterceptor>()
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
