using Cloudents.Core.Entities;

namespace Cloudents.Core.Query
{
    public class BlogQuery
    {
        private BlogQuery(Country country)
        {
            BlogName = "English";
            if (country == Country.Israel)
            {
                BlogName = "Hebrew";
            }
        }

        public static BlogQuery Blog(Country country)
        {
            return new BlogQuery(country)
            {
                Amount = 3,
                Category = "Teachers"

            };
        }

        public static BlogQuery Marketing(Country country)
        {
            return new BlogQuery(country)
            {
                Amount = 2,
                Category = "Self_Promote"

            };
        }
        public string Category  { get; private set; }
        public string BlogName { get;  }

        public int Amount { get; private set; }
    }
}
