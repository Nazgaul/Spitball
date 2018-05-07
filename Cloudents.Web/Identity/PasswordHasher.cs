using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Microsoft.AspNetCore.Identity;

namespace Cloudents.Web.Identity
{
    public class PasswordHasher : IPasswordHasher<User>
    {
        public string HashPassword(User user, string password)
        {
            return password; // TODO: hash to do public key
        }

        public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            throw new NotImplementedException();
        }
    }
}
