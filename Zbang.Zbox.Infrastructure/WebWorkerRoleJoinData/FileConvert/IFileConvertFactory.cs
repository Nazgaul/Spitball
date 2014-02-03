
namespace Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.FileConvert
{
    public interface IFileConvertFactory
    {
        Convertor GetConvertor(string fileName);
        bool CanConvertFile(string fileName);
    }
}
