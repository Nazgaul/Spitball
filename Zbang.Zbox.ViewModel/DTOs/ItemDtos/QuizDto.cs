using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.DTOs.ItemDtos
{
    public class QuizDto : IItemDto
    {
       private DateTime  m_Date;

       public long Id { get; set; }
       public long OwnerId { get; set; }
       public string Owner { get; set; }
       public string Name { get; set; }
       public bool Publish { get; set; }
       public double Rate { get; set; }
       public int NumOfViews { get; set; }
       public string Description { get; set; }
       public int CommentsCount { get; set; }
       public DateTime Date
       {
           get { return m_Date; }
           set
           {
               m_Date = DateTime.SpecifyKind(value, DateTimeKind.Utc);
           }
       }

       public string UserUrl { get; set; }
       public string Url { get; set; }

       public string Type { get { return "Quiz"; } }


       public string DownloadUrl
       {
           get
           {
               return null;
           }
           set
           {
               
           }
       }
    }
}
