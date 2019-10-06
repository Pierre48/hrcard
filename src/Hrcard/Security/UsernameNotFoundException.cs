using System.Security.Authentication;

namespace MyCompany.Security {
    public class UsernameNotFoundException : AuthenticationException {
        public UsernameNotFoundException(string message) : base(message)
        {
        }
    }
}
