using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Models;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class TutorController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<Tutor> Get(string q)
        {
            //string json;
            //List<Tutor> tutors;
            using (StreamReader r = new StreamReader(System.AppContext.BaseDirectory + @"/DataObjects/tutor.json"))
            {
                var json = r.ReadToEnd();
                var tutors = JsonConvert.DeserializeObject<List<Tutor>>(json);
                if (string.IsNullOrEmpty(q))
                {
                    return tutors;
                }
                else
                {
                    return tutors.Where(w => w.Name.Contains(q));
                }
            }

            //return tutors;
        }
        //[Route("api/tutor/tutorbykeyword"), HttpGet]
        //public IEnumerable<Tutor> GetTutorByKeyWords(string keywords)
        //{
        //    var list = Get().OrderBy(t => -(t.KeyWords.IndexOf(keywords)+t.Subject.IndexOf(keywords)));
        //    return list;
        //}


        
      


        // GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}
    }
}