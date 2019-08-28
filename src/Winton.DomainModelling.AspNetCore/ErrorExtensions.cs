// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Winton.DomainModelling.AspNetCore
{
    internal static class ErrorExtensions
    {
        internal static ActionResult ToActionResult(this Error error, Func<Error, ProblemDetails> selectProblemDetails)
        {
            ProblemDetails problemDetails = selectProblemDetails?.Invoke(error) ?? CreateDefaultProblemDetails(error);
            return new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };
        }

        private static ProblemDetails CreateDefaultProblemDetails(Error error)
        {
            int GetStatusCode()
            {
                switch (error)
                {
                    case UnauthorizedError _:
                        return StatusCodes.Status403Forbidden;
                    case NotFoundError _:
                        return StatusCodes.Status404NotFound;
                    case ConflictError _:
                        return StatusCodes.Status409Conflict;
                    default:
                        return StatusCodes.Status400BadRequest;
                }
            }

            int statusCode = GetStatusCode();
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