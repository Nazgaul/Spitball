namespace Cloudents.Web.Models
{
    public class ReturnSignUserResponse
    {
        public ReturnSignUserResponse(NextStep step, bool isNew)
        {
            Step = step;
            IsNew = isNew;
        }

        public NextStep Step { get; set; }
        public bool IsNew { get; set; }

        public string ReturnUrl { get; set; }

    }
}