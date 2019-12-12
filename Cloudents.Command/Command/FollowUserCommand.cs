namespace Cloudents.Command.Command
{
    public class FollowUserCommand : ICommand
    {
        public FollowUserCommand(long followedId, long followerId)
        {
            FollowedId = followedId;
            FollowerId = followerId;
        }
        public long FollowedId { get; set; }
        public long FollowerId { get; set; }
    }
}
