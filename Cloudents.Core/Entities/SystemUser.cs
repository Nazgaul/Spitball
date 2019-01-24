namespace Cloudents.Core.Entities
{
    public class SystemUser : User
    {
        public override int Score { get; protected set; }
    
        public override void MakeTransaction(TransactionType2 transaction, Question question = null, Document document = null)
        {
           //we do nothing
        }

        

        public override decimal Balance => 0;
    }
}