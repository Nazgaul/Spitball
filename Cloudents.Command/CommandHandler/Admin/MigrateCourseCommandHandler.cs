using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class MigrateCourseCommandHandler : ICommandHandler<MigrateCourseCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IRegularUserRepository _regularUserRepository;
        public MigrateCourseCommandHandler(ICourseRepository courseRepository, IDocumentRepository documentRepository,
                IQuestionRepository questionRepository, IRegularUserRepository regularUserRepository)
        {
            _courseRepository = courseRepository;
            _documentRepository = documentRepository;
            _questionRepository = questionRepository;
            _regularUserRepository = regularUserRepository;
        }

        public async Task ExecuteAsync(MigrateCourseCommand message, CancellationToken token)
        {
            var CourseToRemove = await _courseRepository.LoadAsync(message.CourseToRemove, token);
            var CourseToKeep = await _courseRepository.LoadAsync(message.CourseToKeep, token);

            //await _documentRepository.UpdateCourse(CourseToKeep, message.CourseToRemove, token);
            //await _questionRepository.UpdateCourse(CourseToKeep, message.CourseToRemove, token);

           foreach(var doc in  CourseToRemove.Documents)
            {
                doc.Course = CourseToKeep;
                await _documentRepository.UpdateAsync(doc, token);
            }

            foreach (var question in CourseToRemove.Questions)
            {
                question.Course = CourseToKeep;
                await _questionRepository.UpdateAsync(question, token);
            }

            foreach (var user in CourseToRemove.Users)
            {
                user.Courses.Remove(CourseToRemove);
                user.Courses.Add(CourseToKeep);

                await _regularUserRepository.UpdateAsync(user, token);
            }
            
            //await _courseRepository.DeleteAsync(CourseToRemove, token);
        }
    }
}
