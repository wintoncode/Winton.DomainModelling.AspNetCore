// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace Winton.DomainModelling.AspNetCore
{
    public class DomainExceptionFilterTests
    {
        public sealed class OnException : DomainExceptionFilterTests
        {
            public static IEnumerable<object[]> ExceptionHandledTestCases => new List<object[]>
            {
                new object[]
                {
                    new Exception(),
                    false
                },
                new object[]
                {
                    new DomainException("Foo"),
                    true
                },
                new object[]
                {
                    new UnauthorizedException("Foo"),
                    true
                },
                new object[]
                {
                    EntityNotFoundException.Create<TestEntity, int>(),
                    true
                }
            };

            [Fact]
            private void ShouldDefaultToBadRequestResultIfCustomMapperReturnsNull()
            {
                var exceptionContext =
                    new ExceptionContext(
                        new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                        new List<IFilterMetadata>())
                    {
                        Exception = new DomainException("Foo")
                    };
                var filter = new DomainExceptionFilter((exception, response) => null);

                filter.OnException(exceptionContext);

                exceptionContext.Result.Should().BeEquivalentTo(
                    new BadRequestObjectResult(new ErrorResponse(new DomainException("Foo"))));
            }

            [Fact]
            private void ShouldNotSetResultIfNotATypeOfDomainException()
            {
                var exceptionContext =
                    new ExceptionContext(
                        new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                        new List<IFilterMetadata>())
                    {
                        Exception = new Exception()
                    };
                var filter = new DomainExceptionFilter();

                filter.OnException(exceptionContext);

                exceptionContext.Result.Should().BeNull();
            }

            [Fact]
            private void ShouldSetBadRequestResultIfDomainException()
            {
                var exceptionContext =
                    new ExceptionContext(
                        new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                        new List<IFilterMetadata>())
                    {
                        Exception = new DomainException("Foo")
                    };
                var filter = new DomainExceptionFilter();

                filter.OnException(exceptionContext);

                exceptionContext.Result.Should().BeEquivalentTo(
                    new BadRequestObjectResult(new ErrorResponse(new DomainException("Foo"))));
            }

            [Theory]
            [MemberData(nameof(ExceptionHandledTestCases))]
            private void ShouldSetExceptionHandledIfATypeOfDomainException(Exception exception, bool expected)
            {
                var exceptionContext =
                    new ExceptionContext(
                        new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                        new List<IFilterMetadata>())
                    {
                        Exception = exception
                    };
                var filter = new DomainExceptionFilter();

                filter.OnException(exceptionContext);

                exceptionContext.ExceptionHandled.Should().Be(expected);
            }

            [Fact]
            private void ShouldSetNotFoundResultIfEntityNotFoundException()
            {
                EntityNotFoundException exception = EntityNotFoundException.Create<TestEntity, int>();
                var exceptionContext =
                    new ExceptionContext(
                        new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                        new List<IFilterMetadata>())
                    {
                        Exception = exception
                    };
                var filter = new DomainExceptionFilter();

                filter.OnException(exceptionContext);

                exceptionContext.Result.Should().BeEquivalentTo(new NotFoundObjectResult(new ErrorResponse(exception)));
            }

            [Fact]
            private void ShouldSetUnauthorizedResultIfUnauthorizedException()
            {
                var exceptionContext =
                    new ExceptionContext(
                        new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                        new List<IFilterMetadata>())
                    {
                        Exception = new UnauthorizedException("Foo")
                    };
                var filter = new DomainExceptionFilter();

                filter.OnException(exceptionContext);

                exceptionContext.Result.Should().BeEquivalentTo(new UnauthorizedResult());
            }

            [Fact]
            private void ShouldUseExceptionMapperForCustomExceptions()
            {
                var exceptionContext =
                    new ExceptionContext(
                        new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                        new List<IFilterMetadata>())
                    {
                        Exception = new DomainException("Foo")
                    };
                var filter = new DomainExceptionFilter((exception, response) => new OkResult());

                filter.OnException(exceptionContext);

                exceptionContext.Result.Should().BeEquivalentTo(new OkResult());
            }

            // ReSharper disable once ClassNeverInstantiated.Local
            private sealed class TestEntity : Entity<int>
            {
                public TestEntity(int id)
                    : base(id)
                {
                }
            }
        }
    }
}