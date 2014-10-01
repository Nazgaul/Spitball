using System;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class QuizDto
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
    }
}
