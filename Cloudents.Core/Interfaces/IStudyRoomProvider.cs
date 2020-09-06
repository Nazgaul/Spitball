using System;
using System.Threading.Tasks;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Interfaces
{
    public interface IStudyRoomProvider
    {
        Task CreateRoomAsync(string id, bool needRecord, Uri callBack, StudyRoomTopologyType studyRoomType);
        Task CloseRoomAsync(string id);

        Task<bool> GetRoomAvailableAsync(string id);

        string CreateRoomToken(string roomName, long userId, string name);
    }
}