using Microsoft.WindowsAzure.Mobile.Service;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class TodoItem : EntityData
    {
        public string Text { get; set; }

        public bool Complete { get; set; }
    }
}