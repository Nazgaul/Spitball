using Cloudents.Core.Enum;

namespace Cloudents.Core
{
    public static class Privileges
    {
        public const int Post = 2;
      

        public static ItemState GetItemState(int score)
        {
            if (score < Post)
            {
                return ItemState.Pending;
            }

            return ItemState.Ok;
        }

      
    }
}