// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Winton.DomainModelling.AspNetCore
{
    public class ResultExtensionsTests
    {
        public class ToActionResult : ResultExtensionsTests
        {
            public class GenericResult : ToActionResult
            {
                public sealed class WithErrorMapping : GenericResult
                {
                    public static IEnumerable<object[]> TestCases => new List<object[]>
                    {
                        new object[]
                        {
                            new Success<int>(13),
                            new Func<Error, ProblemDetails>(
                                _ => new ProblemDetails { Status = StatusCodes.Status409Conflict }),
                            13
                        },
                        new object[]
                        {
                            new Failure<int>(new Error("Title", "Details")),
                            new Func<Error, ProblemDetails>(
                                _ => new ProblemDetails { Status = StatusCodes.Status409Conflict }),
                            new ConflictObjectResult(new ProblemDetails { Status = StatusCodes.Status409Conflict })
                        }
                    };

                    [Theory]
                    [MemberData(nameof(TestCases))]
                    private void ShouldReturnCorrectActionResult(
                        Result<int> result,
                        Func<Error, ProblemDetails> onError,
                        ActionResult<int> expected)
                    {
                        ActionResult<int> actionResult = result.ToActionResult(onError);

                        actionResult.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
                    }
                }

                public sealed class WithoutErrorMapping : GenericResult
                {
                    public static IEnumerable<object[]> TestCases => new List<object[]>
                    {
                        new object[]
                        {
                            new Success<int>(13),
                            13
                        },
                        new object[]
                        {
                            new Failure<int>(new Error("Title", "Details")),
                            new Error("Title", "Details").ToActionResult(null)
                        }
                    };

                    [Theory]
                    [MemberData(nameof(TestCases))]
                    private void ShouldReturnCorrectActionResult(Result<int> result, ActionResult<int> expected)
                    {
                        ActionResult<int> actionResult = result.ToActionResult();

                        actionResult.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
                    }
                }

                public sealed class WithSuccessAndErrorMapping : GenericResult
                {
                    public static IEnumerable<object[]> TestCases => new List<object[]>
                    {
                        new object[]
                        {
                            new Success<int>(13),
                            new Func<int, IActionResult>(data => new CreatedResult("https://example.com/13", data)),
                            new Func<Error, ProblemDetails>(
                                _ => new ProblemDetails { Status = StatusCodes.Status409Conflict }),
                            new CreatedResult("https://example.com/13", 13)
                        },
                        new object[]
                        {
                            new Failure<int>(new Error("Title", "Details")),
                            new Func<int, IActionResult>(data => new CreatedResult("https://example.com/13", data)),
                            new Func<Error, ProblemDetails>(
                                _ => new ProblemDetails { Status = StatusCodes.Status409Conflict }),
                            new ConflictObjectResult(new ProblemDetails { Status = StatusCodes.Status409Conflict })
                        }
                    };

                    [Theory]
                    [MemberData(nameof(TestCases))]
                    private void ShouldReturnCorrectActionResult(
                        Result<int> result,
                        Func<int, IActionResult> onSuccess,
                        Func<Error, ProblemDetails> onError,
                        IActionResult expected)
                    {
                        IActionResult actionResult = result.ToActionResult(onSuccess, onError);

                        actionResult.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
                    }
                }

                public sealed class WithSuccessMapping : GenericResult
                {
                    public static IEnumerable<object[]> TestCases => new List<object[]>
                    {
                        new object[]
                        {
                            new Success<int>(13),
                            new Func<int, IActionResult>(data => new CreatedResult("https://example.com/13", data)),
                            new CreatedResult("https://example.com/13", 13)
                        },
                        new object[]
                        {
                            new Failure<int>(new Error("Title", "Details")),
                            new Func<int, IActionResult>(data => new CreatedResult("https://example.com/13", data)),
                            new Error("Title", "Details").ToActionResult(null)
                        }
                    };

                    [Theory]
                    [MemberData(nameof(TestCases))]
                    private void ShouldReturnCorrectActionResult(
                        Result<int> result,
                        Func<int, IActionResult> onSuccess,
                        IActionResult expected)
                    {
                        IActionResult actionResult = result.ToActionResult(onSuccess);

                        actionResult.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
                    }
                }
            }

            public class GenericResultAsync : ToActionResult
            {
                public sealed class WithErrorMapping : GenericResultAsync
                {
                    public static IEnumerable<object[]> TestCases => new List<object[]>
                    {
                        new object[]
                        {
                            Task.FromResult<Result<int>>(new Success<int>(13)),
                            new Func<Error, ProblemDetails>(
                                _ => new ProblemDetails { Status = StatusCodes.Status409Conflict }),
                            13
                        },
                        new object[]
                        {
                            Task.FromResult<Result<int>>(new Failure<int>(new Error("Title", "Details"))),
                            new Func<Error, ProblemDetails>(
                                _ => new ProblemDetails { Status = StatusCodes.Status409Conflict }),
                            new ConflictObjectResult(new ProblemDetails { Status = StatusCodes.Status409Conflict })
                        }
                    };

                    [Theory]
                    [MemberData(nameof(TestCases))]
                    private async Task ShouldAwaitResultAndReturnCorrectActionResult(
                        Task<Result<int>> result,
                        Func<Error, ProblemDetails> onError,
                        ActionResult<int> expected)
                    {
                        ActionResult<int> actionResult = await result.ToActionResult(onError);

                        actionResult.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
                    }
                }

                public sealed class WithoutErrorMapping : GenericResultAsync
                {
                    public static IEnumerable<object[]> TestCases => new List<object[]>
                    {
                        new object[]
                        {
                            Task.FromResult<Result<int>>(new Success<int>(13)),
                            13
                        },
                        new object[]
                        {
                            Task.FromResult<Result<int>>(new Failure<int>(new Error("Title", "Details"))),
                            new Error("Title", "Details").ToActionResult(null)
                        }
                    };

                    [Theory]
                    [MemberData(nameof(TestCases))]
                    private async Task ShouldAwaitResultAndReturnCorrectActionResult(
                        Task<Result<int>> result,
                        ActionResult<int> expected)
                    {
                        ActionResult<int> actionResult = await result.ToActionResult();

                        actionResult.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
                    }
                }

                public sealed class WithSuccessAndErrorMapping : GenericResultAsync
                {
                    public static IEnumerable<object[]> TestCases => new List<object[]>
                    {
                        new object[]
                        {
                            Task.FromResult<Result<int>>(new Success<int>(13)),
                            new Func<int, IActionResult>(data => new CreatedResult("https://example.com/13", data)),
                            new Func<Error, ProblemDetails>(
                                _ => new ProblemDetails { Status = StatusCodes.Status409Conflict }),
                            new CreatedResult("https://example.com/13", 13)
                        },
                        new object[]
                        {
                            Task.FromResult<Result<int>>(new Failure<int>(new Error("Title", "Details"))),
                            new Func<int, IActionResult>(data => new CreatedResult("https://example.com/13", data)),
                            new Func<Error, ProblemDetails>(
                                _ => new ProblemDetails { Status = StatusCodes.Status409Conflict }),
                            new ConflictObjectResult(new ProblemDetails { Status = StatusCodes.Status409Conflict })
                        }
                    };

                    [Theory]
                    [MemberData(nameof(TestCases))]
                    private async Task ShouldReturnCorrectActionResult(
                        Task<Result<int>> result,
                        Func<int, IActionResult> onSuccess,
                        Func<Error, ProblemDetails> onError,
                        IActionResult expected)
                    {
                        IActionResult actionResult = await result.ToActionResult(onSuccess, onError);

                        actionResult.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
                    }
                }

                public sealed class WithSuccessMapping : GenericResultAsync
                {
                    public static IEnumerable<object[]> TestCases => new List<object[]>
                    {
                        new object[]
                        {
                            Task.FromResult<Result<int>>(new Success<int>(13)),
                            new Func<int, IActionResult>(data => new CreatedResult("https://example.com/13", data)),
                            new CreatedResult("https://example.com/13", 13)
                        },
                        new object[]
                        {
                            Task.FromResult<Result<int>>(new Failure<int>(new Error("Title", "Details"))),
                            new Func<int, IActionResult>(data => new CreatedResult("https://example.com/13", data)),
                            new Error("Title", "Details").ToActionResult(null)
                        }
                    };

                    [Theory]
                    [MemberData(nameof(TestCases))]
                    private async Task ShouldReturnCorrectActionResult(
                        Task<Result<int>> result,
                        Func<int, IActionResult> onSuccess,
                        IActionResult expected)
                    {
                        IActionResult actionResult = await result.ToActionResult(onSuccess);

                        actionResult.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
                    }
                }
            }

            public class UnitResult : ToActionResult
            {
                public sealed class WithErrorMapping : UnitResult
                {
                    public static IEnumerable<object[]> TestCases => new List<object[]>
                    {
                        new object[]
                        {
                            Success.Unit(),
                            new Func<Error, ProblemDetails>(
                                _ => new ProblemDetails { Status = StatusCodes.Status409Conflict }),
                            new NoContentResult()
                        },
                        new object[]
                        {
                            new Failure<Unit>(new Error("Title", "Details")),
                            new Func<Error, ProblemDetails>(
                                _ => new ProblemDetails { Status = StatusCodes.Status409Conflict }),
                            new ConflictObjectResult(new ProblemDetails { Status = StatusCodes.Status409Conflict })
                        }
                    };

                    [Theory]
                    [MemberData(nameof(TestCases))]
                    private void ShouldReturnCorrectActionResult(
                        Result<Unit> result,
                        Func<Error, ProblemDetails> onError,
                        IActionResult expected)
                    {
                        IActionResult actionResult = result.ToActionResult(onError);

                        actionResult.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
                    }
                }

                public sealed class WithoutErrorMapping : UnitResult
                {
                    public static IEnumerable<object[]> TestCases => new List<object[]>
                    {
                        new object[]
                        {
                            Success.Unit(),
                            new NoContentResult()
                        },
                        new object[]
                        {
                            new Failure<Unit>(new Error("Title", "Details")),
                            new Error("Title", "Details").ToActionResult(null)
                        }
                    };

                    [Theory]
                    [MemberData(nameof(TestCases))]
                    private void ShouldReturnCorrectActionResult(Result<Unit> result, IActionResult expected)
                    {
                        IActionResult actionResult = result.ToActionResult();

                        actionResult.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
                    }
                }
            }

            public class UnitResultAsync : ToActionResult
            {
                public sealed class WithErrorMapping : UnitResultAsync
                {
                    public static IEnumerable<object[]> TestCases => new List<object[]>
                    {
                        new object[]
                        {
                            Task.FromResult<Result<Unit>>(Success.Unit()),
                            new Func<Error, ProblemDetails>(
                                _ => new ProblemDetails { Status = StatusCodes.Status409Conflict }),
                            new NoContentResult()
                        },
                        new object[]
                        {
                            Task.FromResult<Result<Unit>>(new Failure<Unit>(new Error("Title", "Details"))),
                            new Func<Error, ProblemDetails>(
                                _ => new ProblemDetails { Status = StatusCodes.Status409Conflict }),
                            new ConflictObjectResult(new ProblemDetails { Status = StatusCodes.Status409Conflict })
                        }
                    };

                    [Theory]
                    [MemberData(nameof(TestCases))]
                    private async Task ShouldAwaitResultAndReturnCorrectActionResult(
                        Task<Result<Unit>> result,
                        Func<Error, ProblemDetails> onError,
                        IActionResult expected)
                    {
                        IActionResult actionResult = await result.ToActionResult(onError);

                        actionResult.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
                    }
                }

                public sealed class WithoutErrorMapping : UnitResultAsync
                {
                    public static IEnumerable<object[]> TestCases => new List<object[]>
                    {
                        new object[]
                        {
                            Task.FromResult<Result<Unit>>(Success.Unit()),
                            new NoContentResult()
                        },
                        new object[]
                        {
                            Task.FromResult<Result<Unit>>(new Failure<Unit>(new Error("Title", "Details"))),
                            new Error("Title", "Details").ToActionResult(null)
                        }
                    };

                    [Theory]
                    [MemberData(nameof(TestCases))]
                    private async Task ShouldAwaitResultAndReturnCorrectActionResult(
                        Task<Result<Unit>> result,
                        IActionResult expected)
                    {
                        IActionResult actionResult = await result.ToActionResult();

                        actionResult.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
                    }
                }
            }
        }
    }
}