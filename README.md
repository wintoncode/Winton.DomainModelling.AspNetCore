# Winton.DomainModelling.AspNetCore

[![Build status](https://ci.appveyor.com/api/projects/status/k94y5or6toq2un7d?svg=true)](https://ci.appveyor.com/project/wintoncode/winton-domainmodelling-aspnetcore/branch/master)
[![Travis Build Status](https://travis-ci.org/wintoncode/Winton.DomainModelling.AspNetCore.svg?branch=master)](https://travis-ci.org/wintoncode/Winton.DomainModelling.AspNetCore)
[![NuGet version](https://img.shields.io/nuget/v/Winton.DomainModelling.AspNetCore.svg)](https://www.nuget.org/packages/Winton.DomainModelling.AspNetCore)
[![NuGet version](https://img.shields.io/nuget/vpre/Winton.DomainModelling.AspNetCore.svg)](https://www.nuget.org/packages/Winton.DomainModelling.AspNetCore)

Conventions useful for creating an ASP.NET Core based REST API on top of a domain model.

## Exception Filters

### DomainExceptionFilter

An [Exception Filter](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filters.iexceptionfilter) for converting [supported Exceptions](https://github.com/wintoncode/Winton.DomainModelling.Abstractions#exceptions) to [IActionResult](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult)s, automatically setting the type, status code, and message, as appropriate. The following conversions are performed by default:

* Base [DomainException](https://github.com/wintoncode/Winton.DomainModelling.Abstractions#domainexception) to [BadRequest](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.badrequestobjectresult) (HTTP 400)
* [UnauthorizedException](https://github.com/wintoncode/Winton.DomainModelling.Abstractions#unauthorizedexception) to [Unauthorized](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.unauthorizedresult) (HTTP 401)
* [EntityNotFoundException](https://github.com/wintoncode/Winton.DomainModelling.Abstractions#entitynotfoundexception) to [NotFound](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.notfoundobjectresult) (HTTP 404)

#### Usage

The `DomainExceptionFilter` should be added to the collection of filters on the [MvcOptions](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.mvcoptions) configuration for your application. For example, if MVC Core is added to your service collection using a custom configurer

```csharp
services.AddMvcCore(options => options.ConfigureMvc())
```

then simply add the `DomainExceptionFilter` to the collection of filters

```csharp
internal static class MvcConfigurer
{
    public static void ConfigureMvc(this MvcOptions options)
    {
        ...
        options.Filters.Add(new DomainExceptionFilter());
    }
}
```

#### Extensibility

Since `DomainException` is extensible for any domain-specific error, `DomainExceptionFilter` can be extended to support custom Exception to Result mappings. Simply pass a `Func<DomainException, ErrorResponse, IActionResult>` to the constructor, such as

```csharp
new DomainExceptionFilter((exception, response) => exception is TeapotException ? new TeapotResult() : null) // 418
```

Note that all custom mappings are handled **after** `EntityNotFoundException`s and `UnauthorizedException`s.