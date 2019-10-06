using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyCompany.Web.Rest.Problems {
    public class InvalidPasswordException : ProblemDetailsException {
        public InvalidPasswordException() : base(new ProblemDetails {
            Type = ErrorConstants.InvalidPasswordType,
            Detail = "Incorrect Password",
            Status = StatusCodes.Status400BadRequest
        })
        {
        }
    }
}
