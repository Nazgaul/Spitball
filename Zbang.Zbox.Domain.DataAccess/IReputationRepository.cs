namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IReputationRepository 
    {
        int GetUserReputation(long userId);
        int CalculateReputation(long userId);
    }
}