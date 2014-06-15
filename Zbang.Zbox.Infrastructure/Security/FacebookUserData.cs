
using System.Text;
namespace Zbang.Zbox.Infrastructure.Security
{
    public class FacebookUserData
    {
        /*
            "id": "100007262131639",
   "name": "Yang Zion Tomer",
   "first_name": "Yang",
   "middle_name": "Zion",
   "last_name": "Tomer",
   "link": "https://www.facebook.com/yang.tomer",
   "location": {
      "id": "106371992735156",
      "name": "Tel Aviv, Israel"
   },
   "education": [
      {
         "school": {
            "id": "106059432758368",
            "name": "Open University"
         },
         "type": "College"
      }
   ],
   "gender": "male",
   "email": "testingtomer1\u0040gmail.com",
   "timezone": 2,
   "locale": "he_IL",
   "updated_time": "2014-03-24T11:51:33+0000",
   "username": "yang.tomer"
           */
        public long id { get; set; } //yes. the user id is of type long...dont use int
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string locale { get; set; }

        public string Image { get; set; }
        public string LargeImage { get; set; }

        public string gender { get; set; }
        public bool GetGender()
        {
            if (string.IsNullOrEmpty(gender))
            {
                return true;
            }
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
