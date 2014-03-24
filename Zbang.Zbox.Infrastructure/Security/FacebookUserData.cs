
using System.Text;
namespace Zbang.Zbox.Infrastructure.Security
{
    public class FacebookUserData
    {
        /*
           *  "id": "100002578618647",
                "name": "Ruth Lewis",
                "first_name": "Ruth",
                "last_name": "Lewis",
                "link": "http://www.facebook.com/ruthl6898",
                "username": "ruthl6898",
                "gender": "female",
                "email": "ruthl6898\u0040gmail.com",
                "timezone": 2,
                "locale": "en_US",
                "updated_time": "2011-11-29T08:28:39+0000"
           */
        public long id { get; set; } //yes. the user id is of type long...dont use int
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string locale { get; set; }

        public string Image { get; set; }
        public string LargeImage { get; set; }

        private string gender { get; set; }
        public bool GetGender()
        {
            return gender.ToLower() == "male";
        }





        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("id=" + id);
            sb.AppendLine("first_name=" + first_name);
            sb.AppendLine("last_name=" + last_name);
            sb.AppendLine("name=" + name);

            sb.AppendLine("email=" + email);
            sb.AppendLine("locale=" + locale);
            sb.AppendLine("gender=" + gender);
            return sb.ToString();
        }
    }
}
