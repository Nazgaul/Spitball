namespace Cloudents.Core.DTOs
{
    public class HookedDto : System.IEquatable<HookedDto>
    {
        public string Id { get; set; }

        public bool Equals(HookedDto other)
        {
            return Id == other?.Id;
        }
    }

    //public class AddressDto
    //{
    //    public AddressDto(string address)
    //    {
    //        Address = address;
    //    }

    //    public string Address { get;  }
    //}
}