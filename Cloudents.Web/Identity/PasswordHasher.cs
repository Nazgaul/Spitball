﻿using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Cloudents.Web.Identity
{
    [UsedImplicitly]
    public class PasswordHasher : IPasswordHasher<User>
    {
        private readonly IBlockChainProvider _blockChainProvider;

        public PasswordHasher(IBlockChainProvider blockChainProvider)
        {
            _blockChainProvider = blockChainProvider;
        }

        public string HashPassword(User user, string password)
        {
            return _blockChainProvider.GetPublicAddress(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            //TODO: need to check if the password is valid hex
            var publicAddress = _blockChainProvider.GetPublicAddress(providedPassword);

            if (hashedPassword == publicAddress)
            {
                return PasswordVerificationResult.Success;
            }

            return PasswordVerificationResult.Failed;
        }
    }
}
