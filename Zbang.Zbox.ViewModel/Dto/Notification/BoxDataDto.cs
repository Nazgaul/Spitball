
using System.Collections.Generic;


namespace Zbang.Zbox.ViewModel.DTOs.Notification
{
    public class BoxDataDto
    {
        //Move to something more simple. this dto doesnt need all this data
        public IList<FileDto> Files { get; set; }
        
        public IList<CommentDto> Comments { get; set; }

        public long Id { get; protected set; }

        public string Name { get; protected set; }

        public string Owner { get; protected set; }     
    }

    
}
