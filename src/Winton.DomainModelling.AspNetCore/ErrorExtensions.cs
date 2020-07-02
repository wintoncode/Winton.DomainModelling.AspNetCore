// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Winton.DomainModelling.AspNetCore
{
    internal static class ErrorExtensions
    {
        internal static ActionResult ToActionResult(this Error error, Func<Error, ProblemDetails?>? selectProblemDetails)
        {
            ProblemDetails problemDetails = selectProblemDetails?.Invoke(error) ?? CreateDefaultProblemDetails(error);
            return new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };
        }

        private static ProblemDetails CreateDefaultProblemDetails(Error error)
        {
            int statusCode = error switch
            {
                UnauthorizedError _ => StatusCodes.Status403Forbidden,
                NotFoundError _ => StatusCodes.Status404NotFound,
                ConflictError _ => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest
            };
            return new ProblemDetails
            {
                Detail = error.Detail,
                Status = statusCode,
                Title = error.Title,
                Type = $"https://httpstatuses.com/{statusCode}"
            };
        }
    }
}