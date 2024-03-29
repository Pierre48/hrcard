using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCompany.Infrastructure;
using MyCompany.Models;
using MyCompany.Service.Dto;
using MyCompany.Service.Utilities;
using MyCompany.Web.Rest.Problems;
using LanguageExt.UnitsOfMeasure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MyCompany.Service {
    public interface IUserService {
        Task<User> CreateUser(UserDto userDto);
        IEnumerable<string> GetAuthorities();
        Task DeleteUser(string login);
        Task<User> UpdateUser(UserDto userDto);
        Task<User> CompletePasswordReset(string newPassword, string key);
        Task<User> RequestPasswordReset(string mail);
        Task ChangePassword(string currentPassword, string newPassword);
        Task<User> ActivateRegistration(string key);
        Task<User> RegisterUser(UserDto userDto, string password);

        Task UpdateUser(string firstName, string lastName, string email, string langKey, string imageUrl);
        Task<User> GetUserWithUserRoles();
    }

    public class UserService : IUserService {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserService> _log;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public UserService(ILogger<UserService> log, UserManager<User> userManager,
            IPasswordHasher<User> passwordHasher, RoleManager<Role> roleManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _log = log;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User> CreateUser(UserDto userDto)
        {
            var user = new User {
                UserName = userDto.Login.ToLower(),
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email.ToLower(),
                ImageUrl = userDto.ImageUrl,
                LangKey = userDto.LangKey ?? Constants.DefaultLangKey,
                PasswordHash = _userManager.PasswordHasher.HashPassword(null, RandomUtil.GeneratePassword()),
                ResetKey = RandomUtil.GenerateResetKey(),
                ResetDate = DateTime.Now,
                Activated = true
            };
            await _userManager.CreateAsync(user);
            await CreateUserRoles(user, userDto.Roles);
            _log.LogDebug($"Created Information for User: {user}");
            return user;
        }

        public async Task<User> UpdateUser(UserDto userDto)
        {
            //TODO use Optional
            var user = await _userManager.FindByIdAsync(userDto.Id);
            user.Login = userDto.Login.ToLower();
            user.UserName = userDto.Login.ToLower();
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Email = userDto.Email;
            user.ImageUrl = userDto.ImageUrl;
            user.Activated = userDto.Activated;
            user.LangKey = userDto.LangKey;
            await _userManager.UpdateAsync(user);
            await UpdateUserRoles(user, userDto.Roles);
            return user;
        }

        public async Task<User> CompletePasswordReset(string newPassword, string key)
        {
            _log.LogDebug($"Reset user password for reset key {key}");
            var user = _userManager.Users.SingleOrDefault(it => it.ResetKey == key);
            if (user == null || user.ResetDate <= DateTime.Now.Subtract(86400.Seconds())) return null;
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            user.ResetKey = null;
            user.ResetDate = null;
            await _userManager.UpdateAsync(user);
            return user;
        }

        public async Task<User> RequestPasswordReset(string mail)
        {
            var user = await _userManager.FindByEmailAsync(mail);
            if (user == null) return null;
            user.ResetKey = RandomUtil.GenerateResetKey();
            user.ResetDate = DateTime.Now;
            await _userManager.UpdateAsync(user);
            return user;
        }

        public async Task ChangePassword(string currentClearTextPassword, string newPassword)
        {
            var userName = _userManager.GetUserName(_httpContextAccessor.HttpContext.User);
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null) {
                var currentEncryptedPassword = user.PasswordHash;
                var isInvalidPassword =
                    _passwordHasher.VerifyHashedPassword(user, currentEncryptedPassword, currentClearTextPassword) !=
                    PasswordVerificationResult.Success;
                if (isInvalidPassword) throw new InvalidPasswordException();

                var encryptedPassword = _passwordHasher.HashPassword(user, newPassword);
                user.PasswordHash = encryptedPassword;
                await _userManager.UpdateAsync(user);
                _log.LogDebug($"Changed password for User: {user}");
            }
        }

        public async Task<User> ActivateRegistration(string key)
        {
            _log.LogDebug($"Activating user for activation key {key}");
            var user = _userManager.Users.SingleOrDefault(it => it.ActivationKey == key);
            if (user == null) return null;
            user.Activated = true;
            user.ActivationKey = null;
            await _userManager.UpdateAsync(user);
            _log.LogDebug($"Activated user: {user}");
            return user;
        }


        public async Task<User> RegisterUser(UserDto userDto, string password)
        {
            var existingUser = await _userManager.FindByNameAsync(userDto.Login.ToLower());
            if (existingUser != null) {
                var removed = await RemoveNonActivatedUser(existingUser);
                if (!removed) throw new LoginAlreadyUsedException();
            }

            existingUser = _userManager.Users.SingleOrDefault(it =>
                string.Equals(it.Email, userDto.Email, StringComparison.CurrentCultureIgnoreCase));
            if (existingUser != null) {
                var removed = await RemoveNonActivatedUser(existingUser);
                if (!removed) throw new EmailAlreadyUsedException();
            }

            var newUser = new User {
                Login = userDto.Login,
                // new user gets initially a generated password
                PasswordHash = _passwordHasher.HashPassword(null, password),
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email.ToLowerInvariant(),
                ImageUrl = userDto.ImageUrl,
                LangKey = userDto.LangKey,
                // new user is not active
                Activated = false,
                // new user gets registration key
                ActivationKey = RandomUtil.GenerateActivationKey()
                //TODO manage authorities
            };
            await _userManager.CreateAsync(newUser);
            _log.LogDebug($"Created Information for User: {newUser}");
            return newUser;
        }

        public async Task UpdateUser(string firstName, string lastName, string email, string langKey, string imageUrl)
        {
            var userName = _userManager.GetUserName(_httpContextAccessor.HttpContext.User);
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null) {
                user.FirstName = firstName;
                user.LastName = lastName;
                user.Email = email;
                user.LangKey = langKey;
                user.ImageUrl = imageUrl;
                await _userManager.UpdateAsync(user);
                _log.LogDebug($"Changed Information for User: {user}");
            }
        }

        public async Task<User> GetUserWithUserRoles()
        {
            var userName = _userManager.GetUserName(_httpContextAccessor.HttpContext.User);
            if (userName == null) return null;
            return await getUserWithUserRolesByName(userName);
        }

        public IEnumerable<string> GetAuthorities()
        {
            return _roleManager.Roles.Select(it => it.Name).AsQueryable();
        }

        public async Task DeleteUser(string login)
        {
            var user = await _userManager.FindByNameAsync(login);
            if (user != null) {
                await DeleteUserRoles(user);
                await _userManager.DeleteAsync(user);
                _log.LogDebug("Deleted User: {user}");
            }
        }

        private async Task<User> getUserWithUserRolesByName(string name)
        {
            return await _userManager.Users
                .Include(it => it.UserRoles)
                .ThenInclude(r => r.Role)
                .SingleOrDefaultAsync(it => it.UserName == name);
        }

        private async Task<bool> RemoveNonActivatedUser(User existingUser)
        {
            if (existingUser.Activated) return false;

            await _userManager.DeleteAsync(existingUser);
            return true;
        }

        private async Task CreateUserRoles(User user, IEnumerable<string> roles)
        {
            if (roles == null || !roles.Any()) return;

            foreach (var role in roles) await _userManager.AddToRoleAsync(user, role);
        }

        private async Task UpdateUserRoles(User user, IEnumerable<string> roles)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = userRoles.Except(roles).ToArray();
            var rolesToAdd = roles.Except(userRoles).Distinct().ToArray();
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            await _userManager.AddToRolesAsync(user, rolesToAdd);
        }

        private async Task DeleteUserRoles(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
        }
    }
}
