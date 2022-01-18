// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Winton.DomainModelling.AspNetCore;

/// <summary>
///     Extension methods for converting <see cref="Result{TData}"/> types into <see cref="IActionResult"/> types.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    ///     Converts a <see cref="Result{Unit}"/> to an <see cref="IActionResult"/>.
    /// </summary>
    /// <param name="result">
    ///     The result that this extension method is invoked on.
    /// </param>
    /// <returns>
    ///     If this result is a success then a <see cref="NoContentResult"/> is returned;
    ///     otherwise it is converted to the appropriate 4xx response with Problem Details
    ///     containing information about the error.
    /// </returns>
    public static IActionResult ToActionResult(this Result<Unit> result)
    {
        return result.ToActionResult(null as Func<Error, ProblemDetails>);
    }

    /// <summary>
    ///     Converts a <see cref="Result{Unit}"/> to an <see cref="IActionResult"/>.
    /// </summary>
    /// <param name="result">
    ///     The result that this extension method is invoked on.
    /// </param>
    /// <param name="onError">
    ///     The function that is invoked if this <paramref name="result"/> is a <see cref="Failure{TData}"/>.
    ///     It is responsible for mapping the <see cref="Error"/> to <see cref="ProblemDetails"/>.
    ///     If this function returns <c>null</c> then the default error mapping conventions are used.
    ///     This therefore provides a way to customize the error mapping from <see cref="Error"/> to <see cref="ProblemDetails"/>.
    /// </param>
    /// <returns>
    ///     If this result is a success then a <see cref="NoContentResult"/> is returned;
    ///     otherwise it is converted to an error response containing <see cref="ProblemDetails"/>
    ///     in the response body.
    /// </returns>
    public static IActionResult ToActionResult(
        this Result<Unit> result,
        Func<Error, ProblemDetails>? onError)
    {
        return result.Match(_ => new NoContentResult(), error => error.ToActionResult(onError));
    }

    /// <summary>
    ///     Converts a <see cref="Task{Result}"/> of a <see cref="Result{Unit}"/> to an <see cref="IActionResult"/>.
    /// </summary>
    /// <param name="resultTask">
    ///     The asynchronous result that this extension method is invoked on.
    /// </param>
    /// <returns>
    ///     If this result is a success then a <see cref="NoContentResult"/> is returned;
    ///     otherwise it is converted to the appropriate 4xx response containing <see cref="ProblemDetails"/>
    ///     in the response body.
    /// </returns>
    public static async Task<IActionResult> ToActionResult(this Task<Result<Unit>> resultTask)
    {
        return (await resultTask).ToActionResult();
    }

    /// <summary>
    ///     Converts a <see cref="Task{Result}"/> of a <see cref="Result{Unit}"/> to an <see cref="IActionResult"/>.
    /// </summary>
    /// <param name="resultTask">
    ///     The asynchronous result that this extension method is invoked on.
    /// </param>
    /// <param name="onError">
    ///     The function that is invoked if this <paramref name="resultTask"/> is a <see cref="Failure{TData}"/>.
    ///     It is responsible for mapping the <see cref="Error"/> to <see cref="ProblemDetails"/>.
    ///     If this function returns <c>null</c> then the default error mapping conventions are used.
    ///     This therefore provides a way to customize the error mapping from <see cref="Error"/> to <see cref="ProblemDetails"/>.
    /// </param>
    /// <returns>
    ///     If this result is a success then a <see cref="NoContentResult"/> is returned;
    ///     otherwise it is converted to an error response containing <see cref="ProblemDetails"/>
    ///     in the response body.
    /// </returns>
    public static async Task<IActionResult> ToActionResult(
        this Task<Result<Unit>> resultTask,
        Func<Error, ProblemDetails> onError)
    {
        return (await resultTask).ToActionResult(onError);
    }

    /// <summary>
    ///     Converts a <see cref="Result{TData}"/> to an <see cref="ActionResult{TData}"/>.
    /// </summary>
    /// <typeparam name="TData">
    ///     The type of data encapsulated by the result.
    /// </typeparam>
    /// <param name="result">
    ///     The result that this extension method is invoked on.
    /// </param>
    /// <returns>
    ///     If this result is a success then an <see cref="ActionResult{TData}"/> is returned;
    ///     otherwise it is converted to the appropriate 4xx response containing <see cref="ProblemDetails"/>
    ///     in the response body.
    /// </returns>
    public static ActionResult<TData> ToActionResult<TData>(this Result<TData> result)
    {
        return result.ToActionResult(null as Func<Error, ProblemDetails>);
    }

    /// <summary>
    ///     Converts a <see cref="Result{TData}"/> to an <see cref="ActionResult{TData}"/>.
    /// </summary>
    /// <typeparam name="TData">
    ///     The type of data encapsulated by the result.
    /// </typeparam>
    /// <param name="result">
    ///     The result that this extension method is invoked on.
    /// </param>
    /// <param name="onError">
    ///     The function that is invoked if this <paramref name="result"/> is a <see cref="Failure{TData}"/>.
    ///     It is responsible for mapping the <see cref="Error"/> to <see cref="ProblemDetails"/>.
    ///     If this function returns <c>null</c> then the default error mapping conventions are used.
    ///     This therefore provides a way to customize the error mapping from <see cref="Error"/> to <see cref="ProblemDetails"/>.
    /// </param>
    /// <returns>
    ///     If this result is a success then an <see cref="ActionResult{TData}"/> is returned;
    ///     otherwise it is converted to an error response containing <see cref="ProblemDetails"/>
    ///     in the response body.
    /// </returns>
    public static ActionResult<TData> ToActionResult<TData>(
        this Result<TData> result,
        Func<Error, ProblemDetails>? onError)
    {
        return result.Match(data => new ActionResult<TData>(data), error => error.ToActionResult(onError));
    }

    /// <summary>
    ///     Asynchronously converts a <see cref="Task{Result}"/> of a <see cref="Result{TData}"/>
    ///     to an <see cref="ActionResult{TData}"/>.
    /// </summary>
    /// <typeparam name="TData">
    ///     The type of data encapsulated by the result.
    /// </typeparam>
    /// <param name="resultTask">
    ///     The asynchronous result that this extension method is invoked on.
    /// </param>
    /// <returns>
    ///     If this result is a success then an <see cref="ActionResult{TData}"/> is returned;
    ///     otherwise it is converted to the appropriate 4xx response containing <see cref="ProblemDetails"/>
    ///     in the response body.
    /// </returns>
    public static async Task<ActionResult<TData>> ToActionResult<TData>(this Task<Result<TData>> resultTask)
    {
        return (await resultTask).ToActionResult();
    }

    /// <summary>
    ///     Asynchronously converts a <see cref="Task{Result}"/> of a <see cref="Result{TData}"/>
    ///     to an <see cref="ActionResult{TData}"/>.
    /// </summary>
    /// <typeparam name="TData">
    ///     The type of data encapsulated by the result.
    /// </typeparam>
    /// <param name="resultTask">
    ///     The asynchronous result that this extension method is invoked on.
    /// </param>
    /// <param name="onError">
    ///     The function that is invoked if this <paramref name="resultTask"/> is a <see cref="Failure{TData}"/>.
    ///     It is responsible for mapping the <see cref="Error"/> to <see cref="ProblemDetails"/>.
    ///     If this function returns <c>null</c> then the default error mapping conventions are used.
    ///     This therefore provides a way to customize the error mapping from <see cref="Error"/> to <see cref="ProblemDetails"/>.
    /// </param>
    /// <returns>
    ///     If this result is a success then an <see cref="ActionResult{TData}"/> is returned;
    ///     otherwise it is converted to an error response containing <see cref="ProblemDetails"/>
    ///     in the response body.
    /// </returns>
    public static async Task<ActionResult<TData>> ToActionResult<TData>(
        this Task<Result<TData>> resultTask,
        Func<Error, ProblemDetails> onError)
    {
        return (await resultTask).ToActionResult(onError);
    }

    /// <summary>
    ///     Converts a <see cref="Result{TData}"/> to an <see cref="IActionResult"/>.
    /// </summary>
    /// <typeparam name="TData">
    ///     The type of data encapsulated by the result.
    /// </typeparam>
    /// <param name="result">
    ///     The result that this extension method is invoked on.
    /// </param>
    /// <param name="onSuccess">
    ///     The function that is invoked if this <paramref name="result"/> is a <see cref="Success{TData}"/>.
    ///     It is invoked to map the data to an <see cref="IActionResult"/>.
    /// </param>
    /// <returns>
    ///     If this result is a success the result of <paramref name="onSuccess"/> is returned;
    ///     otherwise it is converted to the appropriate 4xx response containing <see cref="ProblemDetails"/>
    ///     in the response body.
    /// </returns>
    public static IActionResult ToActionResult<TData>(
        this Result<TData> result,
        Func<TData, IActionResult> onSuccess)
    {
        return result.ToActionResult(onSuccess, null);
    }

    /// <summary>
    ///     Converts a <see cref="Result{TData}"/> to an <see cref="IActionResult"/>.
    /// </summary>
    /// <typeparam name="TData">
    ///     The type of data encapsulated by the result.
    /// </typeparam>
    /// <param name="result">
    ///     The result that this extension method is invoked on.
    /// </param>
    /// <param name="onSuccess">
    ///     The function that is invoked if this <paramref name="result"/> is a <see cref="Success{TData}"/>.
    ///     It is invoked to map the data to an <see cref="IActionResult"/>.
    /// </param>
    /// <param name="onError">
    ///     The function that is invoked if this <paramref name="result"/> is a <see cref="Failure{TData}"/>.
    ///     It is invoked to map the <see cref="Error"/> to <see cref="ProblemDetails"/>.
    ///     If this function returns <c>null</c> then the default error mapping conventions are used.
    ///     This therefore provides a way to customize the error mapping from <see cref="Error"/> to <see cref="ProblemDetails"/>.
    /// </param>
    /// <returns>
    ///     If this result is a success the result of <paramref name="onSuccess"/> is returned;
    ///     otherwise it is converted to an error response containing <see cref="ProblemDetails"/>
    ///     in the response body.
    /// </returns>
    public static IActionResult ToActionResult<TData>(
        this Result<TData> result,
        Func<TData, IActionResult> onSuccess,
        Func<Error, ProblemDetails>? onError)
    {
        return result.Match(onSuccess, error => error.ToActionResult(onError));
    }

    /// <summary>
    ///     Asynchronously converts a <see cref="Task{Result}"/> of a <see cref="Result{TData}"/>
    ///     to an <see cref="IActionResult"/>.
    /// </summary>
    /// <typeparam name="TData">
    ///     The type of data encapsulated by the result.
    /// </typeparam>
    /// <param name="resultTask">
    ///     The asynchronous result that this extension method is invoked on.
    /// </param>
    /// <param name="onSuccess">
    ///     The function that is invoked if this <paramref name="resultTask"/> is a <see cref="Success{TData}"/>.
    ///     It is invoked to map the data to an <see cref="IActionResult"/>.
    /// </param>
    /// <returns>
    ///     If this result is a success the result of <paramref name="onSuccess"/> is returned;
    ///     otherwise it is converted to the appropriate 4xx response containing <see cref="ProblemDetails"/>
    ///     in the response body.
    /// </returns>
    public static async Task<IActionResult> ToActionResult<TData>(
        this Task<Result<TData>> resultTask,
        Func<TData, IActionResult> onSuccess)
    {
        return (await resultTask).ToActionResult(onSuccess);
    }

    /// <summary>
    ///     Asynchronously converts a <see cref="Task{Result}"/> of a <see cref="Result{TData}"/>
    ///     to an <see cref="IActionResult"/>.
    /// </summary>
    /// <typeparam name="TData">
    ///     The type of data encapsulated by the result.
    /// </typeparam>
    /// <param name="resultTask">
    ///     The asynchronous result that this extension method is invoked on.
    /// </param>
    /// <param name="onSuccess">
    ///     The function that is invoked if this <paramref name="resultTask"/> is a <see cref="Success{TData}"/>.
    ///     It is invoked to map the data to an <see cref="IActionResult"/>.
    /// </param>
    /// <param name="onError">
    ///     The function that is invoked if this <paramref name="resultTask"/> is a <see cref="Failure{TData}"/>.
    ///     It is invoked to map the <see cref="Error"/> to <see cref="ProblemDetails"/>.
    ///     If this function returns <c>null</c> then the default error mapping conventions are used.
    ///     This therefore provides a way to customize the error mapping from <see cref="Error"/> to <see cref="ProblemDetails"/>.
    /// </param>
    /// <returns>
    ///     If this result is a success the result of <paramref name="onSuccess"/> is returned;
    ///     otherwise it is converted to an error response containing <see cref="ProblemDetails"/>
    ///     in the response body.
    /// </returns>
    public static async Task<IActionResult> ToActionResult<TData>(
        this Task<Result<TData>> resultTask,
        Func<TData, IActionResult> onSuccess,
        Func<Error, ProblemDetails> onError)
    {
        return (await resultTask).ToActionResult(onSuccess, onError);
    }
}