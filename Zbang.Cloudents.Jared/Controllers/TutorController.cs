﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class TutorController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<Tutor> Get()
        {
            string json;
            List<Tutor> tutors;
            using (StreamReader r = new StreamReader(System.AppContext.BaseDirectory + @"/DataObjects/tutor.json"))
            {
                json = r.ReadToEnd();
                tutors = JsonConvert.DeserializeObject<List<Tutor>>(json);
            }

            return tutors;
        }
        [Route("api/tutor/tutorbykeyword"), HttpGet]
        public IEnumerable<Tutor> GetTutorByKeyWords(string keywords)
        {
            var list = Get().OrderBy(t => -(t.KeyWords.IndexOf(keywords)+t.Subject.IndexOf(keywords)));
            return list;
        }


        public class Tutor
        {
            //public IEnumerable<string> KeyWords;
            public string Subject;
            private IEnumerable<string> keyList;
            public string KeyWords;
            public string Education;
            public string Description;
            public string Gender;
            public string Name;
            public string Image;
            public string Location;
            public int? Views;
            public int? Likes;
            public IEnumerable<string> getKeyList()
            {
                if (keyList == null || !keyList.Any())
                {
                    keyList = KeyWords.Split(',');
                }
                return keyList;
            }
        }


        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }
    }
}