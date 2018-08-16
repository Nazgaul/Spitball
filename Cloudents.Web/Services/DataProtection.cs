using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using JetBrains.Annotations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Cloudents.Web.Services
{


    public class DataProtection : IDataProtect
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public DataProtection(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public string Protect(string purpose, string plaintext, DateTimeOffset expiration)
        {
            var dataProtector = _dataProtectionProvider.CreateProtector(purpose).ToTimeLimitedDataProtector();
            return dataProtector.Protect(plaintext, expiration);
        }

        public string Unprotect(string purpose, string protectedData)
        {
            var dataProtector = _dataProtectionProvider.CreateProtector(purpose).ToTimeLimitedDataProtector();
            return dataProtector.Unprotect(protectedData);
        }
    }
}
