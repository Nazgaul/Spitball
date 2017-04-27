using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Models;
using System.Reflection;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class TutorController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<Tutor> Get(string q)
        {
            using (var r= new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Zbang.Cloudents.Jared.DataObjects.tutor.json")))
            {
                var json = r.ReadToEnd();
                var tutors = JsonConvert.DeserializeObject<List<Tutor>>(json);
                if (string.IsNullOrEmpty(q))
                {
                    return tutors;
                }
                var matches = tutors.FindAll(t => t.KeyWords.IndexOf(q, StringComparison.OrdinalIgnoreCase)>=0 || t.Subject.IndexOf(q, StringComparison.OrdinalIgnoreCase)>=0);
                var another = tutors.Except(matches).ToList();
                var rnd = new Random();
                var resNum = rnd.Next(20, 41);
                var listToTake=another.OrderBy(x => rnd.Next()).Take(resNum);
                return matches.Concat(listToTake);
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