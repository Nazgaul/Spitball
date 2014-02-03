using System.Collections.Generic;


namespace Zbang.Zbox.ViewModel.DTOs
{
    public class PagedDto<T> where T : class
    {
        public PagedDto(IEnumerable<T> elements, long count)
        {
            Dto = elements;
            Count = count;
        }

        public PagedDto()
        {
            Dto = new List<T>();
        }
        public IEnumerable<T> Dto { get; set; }

        public long Count { get; set; }
    }

    public class PagedDto2<T> where T : class
    {
        public PagedDto2(IEnumerable<T> elements, int count)
        {
            Elem = elements;
            Count = count;
        }

        public PagedDto2()
        {
            Elem = new List<T>();
        }

        public IEnumerable<T> Elem { get; set; }

        public int Count { get; set; }
    }
}
