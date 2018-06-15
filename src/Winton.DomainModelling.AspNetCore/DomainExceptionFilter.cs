using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Winton.DomainModelling.AspNetCore
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception filter for converting <see cref="DomainException" />s to <see cref="IActionResult" />s.
    /// </summary>
    public sealed class DomainExceptionFilter : IExceptionFilter
    {
        private readonly Func<DomainException, ErrorResponse, IActionResult> _exceptionMapper;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DomainExceptionFilter" /> class.
        /// </summary>
        public DomainExceptionFilter()
            : this(null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DomainExceptionFilter" /> class.
        /// </summary>
        /// <param name="exceptionMapper">
        ///     A function to map custom domain exceptions, that are not handled by this class, to action
        ///     results.
        /// </param>
        public DomainExceptionFilter(Func<DomainException, ErrorResponse, IActionResult> exceptionMapper)
        {
            _exceptionMapper = exceptionMapper;
        }

        /// <inheritdoc />
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is DomainException exception)
            {
                context.ExceptionHandled = true;
                context.Result = CreateResult(exception);
            }
        }

        /// <summary>
        ///     Maps a <see cref="DomainException" /> to an <see cref="IActionResult" />.
        ///     This method can be overriden by classes that extend this one to extend or redefine this mapping.
        /// </summary>
        /// <param name="domainException">The <see cref="DomainException" />.</param>
        /// <returns>The <see cref="IActionResult" />.</returns>
        private IActionResult CreateResult(DomainException domainException)
        {
            var errorResponse = new ErrorResponse(domainException);
            switch (domainException)
            {
                case EntityNotFoundException _:
                    return new NotFoundObjectResult(errorResponse);
                case UnauthorizedException _:
                    return new UnauthorizedResult();
                default:
                    return _exceptionMapper?.Invoke(domainException, errorResponse) ??
                           new BadRequestObjectResult(errorResponse);
            }
        }
    }
}