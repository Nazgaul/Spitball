﻿using System;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Security
{

    public interface IAccountService
    {
        Task<bool> ChangePasswordAsync(Guid accountId, string oldPassword, string newPassword);
        Task<Guid?> CreateUserAsync(string email, string password);
        Task<bool> ChangeEmailAsync(Guid accountId, string newEmail);
    }
}
