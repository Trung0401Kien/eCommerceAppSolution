﻿using eCommerceApp.Domain.Entities.Identity;

namespace eCommerceApp.Domain.Interfaces.Authentication
{
    public interface IRoleManagement
    {
        Task<string> GetUserRole(string userEmail);
        Task<bool> AddUserrole(AppUser user, string roleName);
    }
}
