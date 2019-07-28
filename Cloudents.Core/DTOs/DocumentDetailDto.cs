using System;
using System.IO;
using System.Runtime.Serialization;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class DocumentDetailDto
    {
      //  private string _name;
      [DataMember]
        public long Id { get; set; }

        //public string Name
        //{
        //    get => Path.GetFileNameWithoutExtension(_name);
        //    set => _name = value;
        //}

      [DataMember]
        public string Name { get; set; }

      [DataMember]
        public DateTime Date { get; set; }

      [DataMember]
        public string University { get; set; }

      [DataMember]
        public string Course { get; set; }

      [DataMember]
        public string Professor { get; set; }

      [DataMember]
        public TutorCardDto User { get; set; }

        public long UploaderId { get; set; }

      [DataMember]
        public string Type { get; set; }

      [DataMember]
        public int Pages { get; set; }

      [DataMember]
        public int Views { get; set; }

//        public int Downloads { get; set; }

      [DataMember]
        public decimal? Price { get; set; }

      [DataMember]
        public bool IsPurchased { get; set; }

        
        //  public int PageCount { get; set; }

    }

    //public class DocumentUserDto 
    //{
    //    public long Id { get; set; }
    //    public string Name { get; set; }
    //    public string Image { get; set; }
    //    public int Score { get; set; }
    //    public string Courses { get; set; }
    //    public decimal Price { get; set; }

    //    public float? Rate { get; set; }

    //    public string Bio { get; set; }

    //    public int ReviewsCount { get; set; }
    //    public bool IsTutor { get; set; }
    //}
}
