# Winton.DomainModelling.AspNetCore

[![Appveyor](https://ci.appveyor.com/api/projects/status/k94y5or6toq2un7d?svg=true)](https://ci.appveyor.com/project/wintoncode/winton-domainmodelling-aspnetcore/branch/master)
[![Travis CI](https://travis-ci.com/wintoncode/Winton.DomainModelling.AspNetCore.svg?branch=master)](https://travis-ci.com/wintoncode/Winton.DomainModelling.AspNetCore)
[![NuGet version](https://img.shields.io/nuget/v/Winton.DomainModelling.AspNetCore.svg)](https://www.nuget.org/packages/Winton.DomainModelling.AspNetCore)
[![NuGet version](https://img.shields.io/nuget/vpre/Winton.DomainModelling.AspNetCore.svg)](https://www.nuget.org/packages/Winton.DomainModelling.AspNetCore)

Conventions useful for creating an ASP.NET Core based REST API on top of a domain model. Specifically, it provides extension methods which convert from domain model types, as defined in [`Winton.DomainModelling.Abstractions`](https://github.com/wintoncode/Winton.DomainModelling.Abstractions) to ASP.NET Core types.

## `Result` Extensions

`Result<TData>` is a type defined in the `Winton.DomainModelling.Abstractions` package. 
It is a type that is intended to be returned from domain operations.
It allows operations to indicate both successes and failures to the client.
In this case the client is an ASP.NET Core Controller.
In a Controller, however, we need to return an `IActionResult` rather than a `Result<TData>`. We have two cases to consider:
* If the `Result<TData>` was a success then we want to return a 2xx response from the API containing the data in the body.
* If the `Result<TData>` was a failure then we want to return a 4xx response from the API containing [problem details](https://tools.ietf.org/html/rfc7807) in the body.

This library provides a `ToActionResult` extension method for `Result<TData>` which matches on the result and converts it to an appropriate `IActionResult`.
There are various overloads to provide flexibility. 
It is expected that this will be used within an [`ApiController`](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-2.2#annotation-with-apicontroller-attribute) so that ASP.NET Core will apply its REST API conventions to the `IActionResult`.

### Successful Result Mappings

The following default mappings happen when the `Result` is a `Success`.

| Result           | IActionResult         | HTTP Status   |
| ---------------- | --------------------- | ------------- |
| `Success<TData>` | `ActionResult<TData>` | 200 Ok        |
| `Success<Unit>`  | `NoContentResult`     | 204 NoContent |

The defaults can be overriden by calling the extension method that takes a success mapping function. 
A common example of when this is used is in a `POST` action when an entity has been created and we would like to return a 201 Created response to the client.

```csharp
[HttpPost]
public async Task<IActionResult> CreateFoo(NewFoo newFoo)
{
    return await CreateFoo(newFoo.Bar)
        .Select(FooResource.Create)
        .ToActionResult(
            f => Created(
                Url.Action(nameof(Get), new { f.Id }),
                f));
}
```

The `CreateFoo` method performs the domain logic to create a new `Foo` and returns `Result<Foo>`.

*In a real application it would be defined in the domain model project. 
To give the domain model an API which is defined in terms of commands and queries and to decouple it from the outer application layers the mediator pattern is often adopted. 
Jimmy Bogard's [MediatR](https://github.com/jbogard/MediatR) is a useful library for implementing that pattern.*

### Failure Result Mappings

If the `Result` is a `Failure` then the `Error` it contains is mapped to a [`ProblemDetails`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.problemdetails) and is wrapped in an `IActionResult` with the corresponding status code.

The following table shows the default mappings.

| Error                | IActionResult         | HTTP Status    |
| -------------------- | --------------------- | -------------- |
| `Error`*             | `BadRequestResult`    | 400 BadRequest |
| `UnauthorizedError`  | `ForbidResult`        | 403 Forbidden  |
| `NotFoundError`      | `NoContentResult`     | 404 NotFound   |

_*This includes any other types that inherit from `Error` and are not explicitly listed._

The defaults can be overriden by calling the extension method that takes an error mapping function. 
This is useful when the domain model has defined additional error types and these need to be converted to the relevant problem details. 
The status code that is set on the `ProblemDetails` will also be set on the `IActionResult` by the extension method so that the HTTP status code on the response is correct.

For example consider a domain model that deals with payments. 
It could be a news service which requires a subscription to access content. 
It might contain several operations that require payment to be made before they can proceed. 
This domain may therefore define a new error type as follows:

```csharp
public class PaymentRequired : Error
{
    public PaymentRequired(string detail)
        : base("Payment Required", detail)
    {
    }
}
```

It would therefore make sense to map this to a `402 Payment Required` HTTP response with relevant `ProblemDetails`. 
This can be achieved like so:

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetNewsItem(string id)
{
    return await GetNewsItem(id)
        .ToActionResult(
            error => new ProblemDetails
            {
                Detail = error.Detail,
                Status = StatusCodes.Status402PaymentRequired,
                Title = error.Title,
                Type = "https://example.com/problems/payment-required"
            }
        )
}
```

The type field should return a URI that resolves to human-readable documentation about the type of error that has occurred. 
This can either be existing documentation, such as https://httpstatuses.com for common errors, or your own documentation for domain-specific errors.

Problem details is formally documented in [RFC 7807](https://tools.ietf.org/html/rfc7807). 
More information about how the fields should be used can be found there.

In order to maintain a loose coupling between the API layer and the domain model each action method should know how to map any kind of domain error.
To achieve this we could define a function that does this mapping for us and then use it throughout.
For example:

```csharp
internal static ProblemDetails MapDomainErrors(Error error)
{
    switch (error)
    {
        case PaymentRequired _:
            return new ProblemDetails
            {
                Detail = error.Detail,
                Status = StatusCodes.Status402PaymentRequired,
                Title = error.Title,
                Type = "https://example.com/problems/payment-required"
            }
        // handle other custom types
        default:
            return null;
    }
}
```

By using C# pattern matching we can easily match on the type of error and map it to a `ProblemDetails`. 
Returning `null` in the default case means the existing error mappings for the common error types, as defined above, are used.

If you have a custom error type and you are happy for your REST API to return `400 Bad Request` when it occurs, then the default error mappings for the base `Error` type should already work for you. 
It maps the error's details and title to the corresponding fields on the problem details.
