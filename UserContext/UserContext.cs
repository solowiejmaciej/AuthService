﻿using System.Security.Claims;

namespace AuthService.UserContext
{
    public interface IUserContext
    {
        CurrentUser GetCurrentUser();
    }

    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public CurrentUser GetCurrentUser()
        {
            var user = _httpContextAccessor?.HttpContext?.User;

            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            {
                return null!;
            }

            var userName = user.Identity.Name!;
            var userId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
            var userRole = user.FindFirst(c => c.Type == ClaimTypes.Role)!.Value;

            return new CurrentUser(userId, userName, userRole);
        }
    }
}