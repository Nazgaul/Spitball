namespace Zbang.Zbox.Infrastructure.Mail
{
    public class Credentials : ICredentials
    {
        public string UserName => "cloudents";
        public string Password => "zbangitnow";
    }

    public class UsCredentials : ICredentials
    {
        public string UserName => "SpitballUS";
        public string Password => "9cloudents";
    }

    public interface ICredentials
    {
        string UserName { get; }
        string Password { get; }
    }
}
