using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.Dto
{
   public class PagedDto<T> where T : class
    {
        public PagedDto(IEnumerable<T> elements, int count)
        {
            Elem = elements;
            Count = count;
        }

        public PagedDto()
        {
            Elem = new List<T>();
        }

        public IEnumerable<T> Elem { get; set; }

        public int Count { get; set; }
    }
}
