using System.Runtime.Serialization;

namespace Cloudents.Core.DTOs
{
    public class TutorListDto
    {
        [DataMember] public long UserId { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public string Image { get; set; }
        [DataMember] public string Bio { get; set; }
        [DataMember] public decimal Price { get; set; }
        [DataMember] public int Score { get; set; }
        [DataMember] public float? Rate { get; set; }
    }
}
