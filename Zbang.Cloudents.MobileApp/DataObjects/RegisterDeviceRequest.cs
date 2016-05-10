using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class RegisterDeviceRequest
    {
        [Required]
        public string DeviceToken { get; set; }
    }
}