using System;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Infrastructure.Url
{
    public class InviteLinkProvider : IInviteLinkDecrypt, IInviteLinkGenerator
    {
        
        private readonly IEncryptObject m_EncryptObject;

        const string InviteCryptHash = "InviteKeyHash";
        public InviteLinkProvider(IEncryptObject encryptObject)
        {
            m_EncryptObject = encryptObject;
        }

        public string GenerateInviteUrl(Guid id, string boxUrl, long senderId, string recipientEmail)
        {
            var inviteLinkData = new InviteLinkData(id, boxUrl, DateTime.UtcNow.AddMonths(1), senderId, recipientEmail);

            return m_EncryptObject.EncryptElement(inviteLinkData, recipientEmail, InviteCryptHash);
        }

        public InviteLinkData DecryptInviteUrl(string token, string recipientEmail)
        {
            return m_EncryptObject.DecryptElement<InviteLinkData>(token, recipientEmail, InviteCryptHash);
        }

    }

    public interface IInviteLinkGenerator
    {
        string GenerateInviteUrl(Guid id, string boxUrl, long senderId, string recipientEmail);
    }
    public interface IInviteLinkDecrypt
    {
        InviteLinkData DecryptInviteUrl(string token, string recipientEmail);
    }

}
