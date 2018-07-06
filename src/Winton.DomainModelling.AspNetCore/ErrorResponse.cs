// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace Winton.DomainModelling.AspNetCore
{
    /// <summary>
    ///     A response that represents an error.
    /// </summary>
    public struct ErrorResponse
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ErrorResponse" /> struct.
        /// </summary>
        /// <param name="exception">The <see cref="DomainException" /> that resulted from the error.</param>
        internal ErrorResponse(DomainException exception)
        {
            Reason = exception.Message;
        }

        /// <summary>
        ///     Gets a description of the error that occured.
        /// </summary>
        public string Reason { get; }
    }
}