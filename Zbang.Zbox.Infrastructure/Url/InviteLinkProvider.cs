using System;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Infrastructure.Url
{
    public class InviteLinkProvider : IInviteLinkDecrypt, IInviteLinkGenerator
    {

        //public Guid Id { get; private set; }
        //public long BoxId { get; private set; }
        //public DateTime ExpireTime { get; private set; }
        //public long SenderId { get; private set; }
        //public string RecepientEmail { get; private set; }
        private readonly IEncryptObject m_EncryptObject;

        const string InviteCryptHash = "InviteKeyHash";
        public InviteLinkProvider(IEncryptObject encryptObject)
        {
            m_EncryptObject = encryptObject;
        }

        public string GenerateInviteUrl(Guid id, long boxId, long senderId, string recepientEmail)
        {
            var inviteLinkData = new InviteLinkData(id, boxId, DateTime.UtcNow.AddMonths(1), senderId, recepientEmail);

            return m_EncryptObject.EncryptElement(inviteLinkData, recepientEmail, InviteCryptHash);
        }

        public InviteLinkData DecryptInviteUrl(string token, string recepientEmail)
        {
            return m_EncryptObject.DecryptElement<InviteLinkData>(token, recepientEmail, InviteCryptHash);
        }

    }

    public interface IInviteLinkGenerator
    {
        string GenerateInviteUrl(Guid id, long boxId, long senderId, string recepientEmail);
    }
    public interface IInviteLinkDecrypt
    {
        InviteLinkData DecryptInviteUrl(string token, string recepientEmail);
    }

}
