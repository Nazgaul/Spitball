using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.DTOs.Search
{
    public class SearchBoxes
    {

        public string Image {get;set;}
        public string Name { get; set; }
        public string Proffessor { get; set; }
        public string CourseCode { get; set; }
        public long Id { get; set; }

        public string Url { get; set; }
    }

    public class SearchBoxesComparer : IEqualityComparer<SearchBoxes>
    {
        public bool Equals(SearchBoxes x, SearchBoxes y)
        {
            //Check whether the compared objects reference the same data. 
            if (ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal. 
            return x.Id == y.Id;
        }

        public int GetHashCode(SearchBoxes obj)
        {
            if (ReferenceEquals(obj, null)) return 0;

            //Get hash code for the Name field if it is not null. 

            //Calculate the hash code for the product. 
            return obj.Id.GetHashCode();
        }
    }

}
