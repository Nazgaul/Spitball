using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Zbang.Zbox.ViewModel.Queries;
using Zbang.Zbox.ViewModel.DTOs;

namespace Zbang.Zbox.WCFServiceWebRole
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IZboxService" in both code and config file together.
    [ServiceContract]
    public interface IWcfZboxService
    {
        [OperationContract]
        IList<ItemDto> GetBoxItemsPaged(long boxId);



        [OperationContract]
        IList<UserBoxesDto> GetBoxes();
        [OperationContract]
        IList<ViewModel.DTOs.UserBoxesDto> GetSubscribedBoxes();
        [OperationContract]
        long UploadFile(long boxId, string FileName, Guid blobName);
        [OperationContract]
        bool DeleteBoxItem(long boxItemId, long boxId);
        [OperationContract]
        bool ChangeBoxName(long BoxId, string NewBoxName);
        [OperationContract]
        string GetBlobReadPermission(long itemid);
        [OperationContract]
        string GetWritePermission(string blobName);

    }
}
