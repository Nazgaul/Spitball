﻿
namespace Zbang.Zbox.Infrastructure.Security
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

    // ReSharper disable InconsistentNaming
    public class FacebookUserData2
    {
        public long id { get; set; }

        public Education[] education { get; set; }

        public string email { get; set; }
        public string first_name { get; set; }

        public string middle_name { get; set; }
        public string gender { get; set; }
        public string last_name { get; set; }
        public string link { get; set; }
        public string locale { get; set; }
        public string name { get; set; }
        public float timezone { get; set; }
        public bool verified { get; set; }

        public string Image { get; set; }
        public string LargeImage { get; set; }

        public bool GetGender()
        {
            if (string.IsNullOrEmpty(gender))
            {
                return true;
            }
            return gender.ToLower() == "male";
        }
    }

    public class Education
    {
        public School school { get; set; }
        public string type { get; set; }
    }

    public class School
    {
        public string id { get; set; }
        public string name { get; set; }

        public override string ToString()
        {
            return string.Format("id: {0} name: {1}", id, name);
        }
    }
    // ReSharper restore InconsistentNaming

}
