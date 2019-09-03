# XService - Enterprise Console Service Template

The goal of this template is to provide a [12-Factor](https://12factor.net/) ready .NET Core based console service solution from the `dotnet new` cli.

Other than the IoC container and logging mechansim, this project is designed to keep tooling opinions to a minimum, while providing the a starting point for creating robust enterprise level applications.

There are other templates that focus on domain driven design or implement stronger paradigms; however, the world is full of FileWatchers, adhocs and data movers that do not gain value of DDD or other architectural paradigms. Since the value is in quick turn-around, I wanted to make sure developers spent less time undoing opinions are more time adding value. You are welcome.

## What is included

- [AutoFac](https://autofac.org/)
  - Provides DI support as an Inversion of Control container
  - Aspect Oriented Programming support for
    - Interceptors
    - Attributes
- [Serilog](https://serilog.net/)
  - Standard logging functionality
  - Logging via configuration with support for the following sinks
    - Console (SystemConsole)
    - File
  - Logging is managed through IoC using `ILogger<T>` as a dependency
- Decoupled hosted service model
  - Allows the business hosted service to remain portable from its execution profile
- Linting
  - [.editorconfig](https://editorconfig.org/) settings to enforce (or encourage) development standards
  - [dotnet-format](https://github.com/dotnet/format)
    - Allows for automated code corrections via .editorconfig file
    - _For VS Code/Codium Users_ there are tasks supporting installation and running
- [Docker](https://www.docker.com/) Support
  - The Driver application supports docker for .NET Core 2.2
  - At a solution level, docker-compose support allows for multi application services

## Coming soon

- Conditional test project
- Coverlet support for code coverage reporting
- Support for automated documentation
- Install via nuget

### To install as a dotnet new template

```
dotnet new -i .
```

### To uninstall

```
dotnet new -u xservice
```

### To use

By default this applicaton will assume the name of the directory it is created in and includes 3 projects in the solution:

- XService.Business
  - Contains business logic supporting unique activities and behaviors
- XService.Driver
  - Contains the execution logic
- XService.Enterprise
  - Contains core enterprise functionality agnostic of the business layer, such as aspects

When the application is created `XService` is replaced with the directory name or the value provided with the `-n` parameter

```
mkdir mydemoservice
cd mydemoservice
dotnet new xservice
```

This will create 3 folders in ./mydemoservice with each solution

```
./mydemoservice
  - mydemoservice.Business
  - mydemoservice.Driver
  - mydemoservice.Enterprise
```
