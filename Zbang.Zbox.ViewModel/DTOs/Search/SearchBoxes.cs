using System;
using System.Collections.Generic;

namespace Zbang.Zbox.ViewModel.DTOs.Search
{
    public class SearchBoxes
    {
        private string m_Image;

        public string Image
        {
            get { return m_Image; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                   // var blobProvider = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity.Resolve<Zbang.Zbox.Infrastructure.Storage.IBlobProvider>();
                    m_Image = value;// blobProvider.GetThumbnailUrl(value);
                }
            }
        }
        public string Name { get; set; }
        public string Proffessor { get; set; }
        public string CourseCode { get; set; }
        public long Id { get; set; }
        public string Universityname { get; set; }

        public string Url { get; set; }
    }

    public class SearchBoxesComparer : IEqualityComparer<SearchBoxes>
    {
        public bool Equals(SearchBoxes x, SearchBoxes y)
        {
            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal. 
            return x.Id == y.Id;
        }

        public int GetHashCode(SearchBoxes obj)
        {
            if (Object.ReferenceEquals(obj, null)) return 0;

            //Get hash code for the Name field if it is not null. 

            //Calculate the hash code for the product. 
            return obj.Id.GetHashCode();;
        }
    }

}
