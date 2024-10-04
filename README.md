# LogFlake Client ASP.NET Core ![Version](https://img.shields.io/badge/version-1.8.1-blue.svg?cacheSeconds=2592000)

> This repository contains the sources for the client-side components of the LogFlake product suite for applications logs and performance collection for ASP.NET applications.

### 🏠 [LogFlake Website](https://logflake.io) |  🔥 [CloudPhoenix Website](https://cloudphoenix.it)

## Downloads

|NuGet Package Name|Version|Downloads|
|:-:|:-:|:-:|
| [LogFlake.Client.AspNetCore](https://www.nuget.org/packages/LogFlake.Client.AspNetCore) | ![NuGet Version](https://img.shields.io/nuget/v/logflake.client.aspnetcore) | ![NuGet Downloads](https://img.shields.io/nuget/dt/logflake.client.aspnetcore) |

## Dependencies
This package depends on [LogFlake.Client.NetCore](https://www.nuget.org/packages/LogFlake.Client.NetCore).    
Please refer to the documentation of that package for usage instructions.

## Usage
1. [Optional] If you want to customize some automatic logs from the middleware, add the following section to your `secrets.json` file:
```json
"LogFlakeMiddlewareSettings": {
    "LogRequest": true,
    "LogResponse": true,
    "LogNotFoundErrors": true,
    "ClientIdSelector": ""
},
```
All of them are optional, if a boolean property it's missing, the default value is `false`;
`ClientIdSelector` should be the key of the OAuth Claim that contains what you consider a client identifier, its value will be added to the log entry as `clientId`.

2. Create a class (name it something like `ConfigureLogFlakeMiddlewareOptions`) that implements `IConfigureOptions<LogFlakeMiddlewareOptions>` and configure each property;
3. Register your class as a Singleton:
```csharp
services.AddSingleton<IConfigureOptions<LogFlakeMiddlewareOptions>, ConfigureLogFlakeMiddlewareOptions>();
```
4. After calling `services.AddLogFlake(configuration);`, add the following line of code:
```csharp
services.ConfigureLogFlakeMiddlewareOptions(configuration);
```
> Note: You can choose between a `Guid`, a [TraceIdentifier](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpcontext.traceidentifier) or a Custom correlation id.

> Note: In order to use a Custom correlation id you must implement and register `ICorrelationService` interface (suggestion: as a AddScoped).
5. Register the middleware on your `WebApplication`
```csharp
app.UseLogFlakeMiddleware();
```
