// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Winton.DomainModelling.AspNetCore
{
    public class ErrorExtensionsTests
    {
        public sealed class ToActionResult : ErrorExtensionsTests
        {
            public static IEnumerable<object[]> TestCases => new List<object[]>
            {
                new object[]
                {
                    new UnauthorizedError("Access denied"),
                    new ObjectResult(
                        new ProblemDetails
                        {
                            Detail = "Access denied",
                            Status = 403,
                            Title = "Unauthorized",
                            Type = "https://httpstatuses.com/403"
                        })
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    }
                },
                new object[]
                {
                    new NotFoundError("The requested object was not found"),
                    new NotFoundObjectResult(
                        new ProblemDetails
                        {
                            Detail = "The requested object was not found",
                            Status = 404,
                            Title = "Not found",
                            Type = "https://httpstatuses.com/404"
                        })
                },
                new object[]
                {
                    new Error("Foo error", "A generic error occurred."),
                    new BadRequestObjectResult(
                        new ProblemDetails
                        {
                            Detail = "A generic error occurred.",
                            Status = 400,
                            Title = "Foo error",
                            Type = "https://httpstatuses.com/400"
                        })
                }
            };

            [Theory]
            [MemberData(nameof(TestCases))]
            private void ShouldReturnDefaultResponseIfMappingFunctionIsNull(
                Error error,
                IActionResult expected)
            {
                IActionResult actionResult = error.ToActionResult(null);

                actionResult.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
            }

            [Theory]
            [MemberData(nameof(TestCases))]
            private void ShouldReturnDefaultResponseIfMappingFunctionReturnsNull(
                Error error,
                IActionResult expected)
            {
                IActionResult actionResult = error.ToActionResult(e => null);

                actionResult.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
            }

            [Fact]
            private void ShouldUseSelectedProblemDetailsIfNotNull()
            {
                var error = new Error("Unsupported Beverage Type", "You cannot make coffee in a teapot.");

                ActionResult actionResult = error.ToActionResult(
                    e =>
                        new ProblemDetails
                        {
                            Detail = e.Detail,
                            Status = StatusCodes.Status418ImATeapot,
                            Title = e.Title,
                            Type = "https://example.com/teapots/unsuppported-beverage"
                        });

                actionResult.Should().BeEquivalentTo(
                    new ObjectResult(
                        new ProblemDetails
                        {
                            Detail = "You cannot make coffee in a teapot.",
                            Status = StatusCodes.Status418ImATeapot,
                            Title = "Unsupported Beverage Type",
                            Type = "https://example.com/teapots/unsuppported-beverage"
                        })
                    {
                        StatusCode = StatusCodes.Status418ImATeapot
                    });
            }
        }
    }
}