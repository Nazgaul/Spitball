namespace Cloudents.Web.Models
{
    public class ReturnSignUserResponse
    {
        public ReturnSignUserResponse(NextStep step, bool isNew)
        {
            Step = step;
            IsNew = isNew;
        }

        public ReturnSignUserResponse(NextStep step, bool isNew, string returnUrl) : this(step, isNew)
        {
            ReturnUrl = returnUrl;
        }

        public NextStep Step { get; set; }
        public bool IsNew { get; set; }

        public string ReturnUrl { get; set; }

    }
}