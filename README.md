<h1 align="center">LogFlake Client .NET Core <img alt="Version" src="https://img.shields.io/badge/version-1.5.1-blue.svg?cacheSeconds=2592000" /></h1>

> This repository contains the sources for the client-side components of the LogFlake product suite for applications logs and performance collection for ASP.NET applications.

### 🏠 [LogFlake Website](https://logflake.io) |  🔥 [CloudPhoenix Website](https://cloudphoenix.it)

## Downloads

|NuGet Package Name|Version|Downloads|
|:-:|:-:|:-:|
| [LogFlake.Client.AspNetCore](https://www.nuget.org/packages/LogFlake.Client.AspNetCore) | ![NuGet Version](https://img.shields.io/nuget/v/logflake.client.aspnetcore) | ![NuGet Downloads](https://img.shields.io/nuget/dt/logflake.client.aspnetcore) |

## Prerequisites
You also need to install & configure [LogFlake.Client.NetCore](https://www.nuget.org/packages/LogFlake.Client.NetCore).    
Please refer to the documentation of that package for usage instructions.

## Usage
1. [Optional] If you want to customize some automatic logs from the middleware, add the following section to your `secrets.json` file:
```json
"LogFlakeMiddlewareSettings": {
    "LogRequest": true,
    "LogResponse": true,
    "LogNotFoundErrors": true
},
```
All of them are optional, if a property it's missing, the default value is `false`;

2. Create a class that implements `IConfigureOptions<LogFlakeMiddlewareOptions>` and configure each property;
3. Register that class as a Singleton:
```csharp
services.AddSingleton<IConfigureOptions<LogFlakeMiddlewareOptions>, ConfigureLogFlakeMiddlewareOptions>();
```
4. Register the middleare on your `WebApplication`
```csharp
app.UseLogFlakeMiddleware();
```
