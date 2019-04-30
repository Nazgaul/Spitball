using System.Runtime.Serialization;

namespace Cloudents.Core.DTOs
{
    public class TutorListDto
    {
       public long UserId { get; set; }
       public string Name { get; set; }
       public string Image { get; set; }
       public string Courses { get; set; }
       public decimal Price { get; set; }
       public int Score { get; set; }
       public float? Rate { get; set; }
    }
}
