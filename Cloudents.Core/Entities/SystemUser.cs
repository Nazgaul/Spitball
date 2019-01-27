namespace Cloudents.Core.Entities
{
    public class SystemUser : User
    {
        public override int Score { get; protected set; }

        public override void MakeTransaction(Transaction transaction)
        {
           //we do nothing
        }

        

        public override decimal Balance => 0;
    }
}