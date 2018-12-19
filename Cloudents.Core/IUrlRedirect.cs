namespace Cloudents.Application
{
    public interface IUrlRedirect
    {
        string Url { get; set; }
        string Source { get; }
    }
}