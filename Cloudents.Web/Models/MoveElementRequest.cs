namespace Cloudents.Web.Models
{
    public class MoveElementRequest
    {
        public int OldPosition { get; set; }
        public int NewPosition { get; set; }

        public bool? VisibleOnly { get; set; }
    }
}