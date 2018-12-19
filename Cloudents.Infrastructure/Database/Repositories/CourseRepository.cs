using Cloudents.Domain.Entities;
using NHibernate;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Interfaces;

namespace Cloudents.Infrastructure.Database.Repositories
{
    public class CourseRepository : NHibernateRepository<Course>, ICourseRepository
    {
        public CourseRepository(ISession session) : base(session)
        {
        }

        public async Task<Course> GetOrAddAsync(string name, CancellationToken token)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            name = name.Trim();
            var course = await GetAsync(name, token);

            if (course == null)
            {

                course = new Course(name);
                await AddAsync(course, token).ConfigureAwait(true);
            }

            return course;
        }
    }
}