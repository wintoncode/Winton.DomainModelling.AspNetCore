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
        ///     This parameter can be used to extend the capabilities of this exception filter to meet an application's specific
        ///     requirements.
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