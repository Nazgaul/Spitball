using Cloudents.Core.Enum;

namespace Cloudents.Core
{
    public static class Privileges
    {
        public const int Post = 150;
        public const int Flag = 400;
        //public const int UpVote = 200;
        //public const int DownVote = 250;

        public static ItemState GetItemState(int score)
        {
            if (score < Post)
            {
                return ItemState.Pending;
            }

            return ItemState.Ok;
        }

        //public static bool CanVote(int score, VoteType vote)
        //{
        //    switch (vote)
        //    {
        //        case VoteType.Down:
        //            return score >= DownVote;
        //        case VoteType.None:
        //            return true;
        //        case VoteType.Up:
        //            return score >= UpVote;
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(vote), vote, null);
        //    }
        //}

        public static bool CanFlag(int score)
        {
            return score >= Flag;
        }
    }
}