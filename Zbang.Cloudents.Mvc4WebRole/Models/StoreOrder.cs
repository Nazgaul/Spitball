using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Views.Shared.Resources;
using Zbang.Cloudents.Mvc4WebRole.Views.Store.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class StoreOrder
    {
        public int[] Features { get; set; }
        [Required]
       
        public long ProductId { get; set; }

        [Required]
        [Display(ResourceType = typeof(SharedResources), Name = "FirstName")]
        public string FirstName { get; set; }

        [Required]
        [Display(ResourceType = typeof(SharedResources), Name = "LastName")]
        public string LastName { get; set; }

        [Required]
        [Display(ResourceType = typeof(StoreResources), Name = "Telephone")]
        public string Phone1 { get; set; }

        [Display(ResourceType = typeof(StoreResources), Name = "Telephone")]
        public string Phone2 { get; set; }
        [Required]
        [Display(ResourceType = typeof(SharedResources), Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(ResourceType = typeof(StoreResources), Name = "City")]
        public string City { get; set; }
        [Required]
        [Display(ResourceType = typeof(StoreResources), Name = "Address")]
        public string Address { get; set; }
        [Required]
        [Display(ResourceType = typeof(StoreResources), Name = "IdentityNo")]
        public string IdentityNumber { get; set; }
        [Required]
        [Display(ResourceType = typeof(StoreResources), Name = "CreditCardNo")]
        public string CreditCardNumber { get; set; }

        [Required]
        //TODO: add this to form
        public int ExpirationYear { get; set; }

        [Required]
        //TODO: add this to form
        public int ExpirationMonth { get; set; }

        [Required]
        [Display(ResourceType = typeof(StoreResources), Name = "ThreeDigits")]
        public string SecurityCode { get; set; }

        [Required]
        [Display(ResourceType = typeof(StoreResources), Name = "CardOwnerName")]
        public string CreditCardOwnerName { get; set; }
        [Required]
        [Display(ResourceType = typeof(StoreResources), Name = "CardOwnerNo")]
        public string CreditCardOwnerId { get; set; }

        [Display(ResourceType = typeof(SharedResources), Name = "Comments")]
        public string Comments { get; set; }

        //[Display(ResourceType = typeof(StoreResources), Name = "IReadIt")]
        //public bool AcceptTermOfService { get; set; }

        [Required]
        
        //TODO: add to form
        public int NumberOfPayments { get; set; }
    }
}