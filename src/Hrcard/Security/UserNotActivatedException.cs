using System.Security.Authentication;

namespace MyCompany.Security {
    public class UserNotActivatedException : AuthenticationException {
        public UserNotActivatedException(string message) : base(message)
        {
        }
    }
}
