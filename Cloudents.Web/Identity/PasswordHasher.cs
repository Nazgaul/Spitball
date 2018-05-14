using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Cloudents.Web.Identity
{
    [UsedImplicitly]
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
            if (user.PublicKey == hashedPassword)
            {
                return PasswordVerificationResult.Success;
            }

            return PasswordVerificationResult.Failed;
        }
    }
}
