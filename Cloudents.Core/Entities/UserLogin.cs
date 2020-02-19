using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class UserLogin
    {
        public UserLogin(string loginProvider, string providerKey, string providerDisplayName,
            User user)
        {
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
            ProviderDisplayName = providerDisplayName;
            User = user;
        }
        protected UserLogin()
        {

        }


        public virtual string LoginProvider { get; set; }
        public virtual string ProviderKey { get; set; }
        public virtual string ProviderDisplayName { get; set; }
        public virtual User User { get; set; }

        public override bool Equals(object obj)
        {
            return obj is UserLogin login &&
                   LoginProvider == login.LoginProvider &&
                   ProviderKey == login.ProviderKey;
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode", Justification = "Nhibernate")]
        public override int GetHashCode()
        {
            var hashCode = 1582216818;

            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LoginProvider);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ProviderKey);
            return hashCode;
        }
    }
}