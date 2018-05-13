using System;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Cloudents.Web.Identity
{
    public class PasswordHasher : IPasswordHasher<User>
    {
        private readonly IBlockchainProvider _blockChainProvider;

        public PasswordHasher(IBlockchainProvider blockChainProvider)
        {
            _blockChainProvider = blockChainProvider;
        }

        public string HashPassword(User user, string password)
        {
            return _blockChainProvider.GetPublicAddress(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            throw new NotImplementedException();
        }
    }
}
