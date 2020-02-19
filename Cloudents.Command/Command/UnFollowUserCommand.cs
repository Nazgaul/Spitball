namespace Cloudents.Command.Command
{
    public class UnFollowUserCommand : ICommand
    {
        public UnFollowUserCommand(long followedId, long followerId)
        {
            FollowedId = followedId;
            FollowerId = followerId;
        }
        public long FollowedId { get; set; }
        public long FollowerId { get; set; }
    }
}
