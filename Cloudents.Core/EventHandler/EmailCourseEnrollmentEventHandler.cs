using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Storage;

namespace Cloudents.Core.EventHandler
{
    public class EmailCourseEnrollmentEventHandler : EmailEventHandler, IEventHandler<CourseEnrollmentEvent>
    {
        public EmailCourseEnrollmentEventHandler(IQueueProvider serviceBusProvider) : base(serviceBusProvider)
        {
        }

        public async Task HandleAsync(CourseEnrollmentEvent eventMessage, CancellationToken token)
        {
            var enrollment = eventMessage.Enrollment;
            var tutor = enrollment.Course.Tutor;
            var email = new EnrollCourseMessage(tutor.User.FirstName,enrollment.User.FirstName,enrollment.Course.Name,enrollment.User.Email);
            await SendEmailAsync(email, token);
        }
    }
}