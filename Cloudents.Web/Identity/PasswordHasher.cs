using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;

namespace Cloudents.Web.Identity
{
    //[UsedImplicitly]
    //public class PasswordHasher : IPasswordHasher<User>
    //{
    //    public string HashPassword(User user, string password)
    //    {
    //        return Infrastructure.BlockChain.BlockChainProvider.GetPublicAddress(password);
    //    }

    //    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    //    {
    //        //TODO: need to check if the password is valid hex
    //        var publicAddress = Infrastructure.BlockChain.BlockChainProvider.GetPublicAddress(providedPassword);

    //        if (hashedPassword == publicAddress)
    //        {
    //            return PasswordVerificationResult.Success;
    //        }

    //        return PasswordVerificationResult.Failed;
    //    }
    //}
}
