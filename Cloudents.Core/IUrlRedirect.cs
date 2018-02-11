namespace Cloudents.Core
{
    public interface IUrlRedirect
    {
        string Url { get; set; }
        string Source { get; }
    }
}