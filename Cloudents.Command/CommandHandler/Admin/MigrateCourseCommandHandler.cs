﻿//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Command.Command.Admin;
//using Cloudents.Core.Interfaces;

//namespace Cloudents.Command.CommandHandler.Admin
//{
//    public class MigrateCourseCommandHandler : ICommandHandler<MigrateCourseCommand>
//    {
//        private readonly ICourseRepository _courseRepository;
//        public MigrateCourseCommandHandler(ICourseRepository courseRepository)
//        {
//            _courseRepository = courseRepository;
//        }

//        public Task ExecuteAsync(MigrateCourseCommand message, CancellationToken token)
//        {
//            return _courseRepository.MigrateCourseAsync(message.CourseToKeep, message.CourseToRemove, token);
//        }
//    }
//}