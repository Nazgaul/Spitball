using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;

namespace Cloudents.Persistance.Repositories
{
    public class CourseRepository : NHibernateRepository<Course>, ICourseRepository
    {
        public CourseRepository(ISession session) : base(session)
        {
        }

        public async Task<Course> GetOrAddAsync(string name, CancellationToken token)
        {
            var id = Session.Query<Course>()
                .Where(w => w.Name.Equals(name)).Select(s => s.Id).FirstOrDefault();
                


            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            name = name.Trim();
            var course = await GetAsync(id, token);

            if (course == null)
            {

                course = new Course(name);
                await AddAsync(course, token);
            }

            return course;
        }
    }
}