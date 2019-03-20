//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Autofac;
//using Cloudents.Command.Command;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Interfaces;
//using Cloudents.Core.Storage;

//namespace ConsoleApp
//{
//    public class CreateMessageCommand : IRequest
//    {
//        public CreateMessageCommand(string message, long userSendingId, IEnumerable<long> usersId,
//            Guid? chatRoomId, string blob)
//        {
//            Message = message;
//            UserSendingId = userSendingId;
//            ToUsersId = usersId;
//            ChatRoomId = chatRoomId;
//            Blob = blob;
//        }

//        public Guid? ChatRoomId { get; }

//        public string Message { get; }
//        public long UserSendingId { get; }

//        public string Blob { get; }

//        public IEnumerable<long> ToUsersId { get; }
//    }

//    //public class CreateRam2 : IRequest
//    //{

//    //}

//    //public class Pong1 : INotificationHandler<CreateRam>
//    //{
//    //    public Task Handle(CreateRam notification, CancellationToken cancellationToken)
//    //    {
//    //        Console.WriteLine("NotificationRam1");
//    //        return Task.CompletedTask;
//    //    }
//    //}
//    //public class Pong2 : INotificationHandler<CreateRam>
//    //{
//    //    public Task Handle(CreateRam notification, CancellationToken cancellationToken)
//    //    {
//    //        Console.WriteLine("NotificationRam2");
//    //        return Task.CompletedTask;
//    //    }
//    //}

//    public class CreateRamHandler : AsyncRequestHandler<CreateMessageCommand>
//    {
//        private readonly IChatRoomRepository _chatRoomRepository;
//        private readonly IRegularUserRepository _userRepository;
//        private readonly IRepository<ChatUser> _chatUserRepository;
//        private readonly IRepository<ChatMessage> _chatMessageRepository;
//        private readonly IChatDirectoryBlobProvider _blobProvider;

//        public CreateRamHandler(IChatRoomRepository chatRoomRepository, IRegularUserRepository userRepository, IRepository<ChatMessage> chatMessageRepository, IRepository<ChatUser> chatUserRepository, IChatDirectoryBlobProvider blobProvider)
//        {
//            _chatRoomRepository = chatRoomRepository;
//            _userRepository = userRepository;
//            _chatMessageRepository = chatMessageRepository;
//            _chatUserRepository = chatUserRepository;
//            _blobProvider = blobProvider;
//        }
//        protected override async Task Handle(CreateMessageCommand message, CancellationToken token)
//        {
//            var users = message.ToUsersId.ToList();
//            users.Add(message.UserSendingId);
//            ChatRoom chatRoom;
//            if (message.ChatRoomId.HasValue)
//            {
//                chatRoom = await _chatRoomRepository.LoadAsync(message.ChatRoomId.Value, token);
//            }
//            else
//            {
//                chatRoom = await _chatRoomRepository.GetChatRoomAsync(users, token);
//            }

//            if (chatRoom == null)
//            {
//                chatRoom = new ChatRoom(users.Select(s => _userRepository.Load(s)));
//                await _chatRoomRepository.AddAsync(chatRoom, token);
//            }

//            var chatUser = chatRoom.Users.FirstOrDefault(f => f.User.Id == message.UserSendingId);
//            var chatMessage = new ChatMessage(chatUser, message.Message, message.Blob);
//            await _chatMessageRepository.AddAsync(chatMessage, token);
//            chatRoom.UpdateTime = DateTime.UtcNow;
//            await _chatRoomRepository.UpdateAsync(chatRoom, token);
//            //var t = Task.CompletedTask;
//            foreach (var user in chatRoom.Users)
//            {
//                if (message.UserSendingId != user.User.Id && !user.User.Online)
//                {
//                    //TODO: need to send an email or something
//                }
//                if (user.User.Id == message.UserSendingId)
//                {
//                    user.Unread = 0;
//                }
//                else
//                {
//                    user.Unread++;
//                }
//                await _chatUserRepository.UpdateAsync(user, token);
//            }

//            if (!string.IsNullOrEmpty(message.Blob))
//            {
//                var id = chatMessage.Id;
//                await _blobProvider.MoveAsync(message.Blob, id.ToString(), token);
//            }
//        }
//        //protected override Task Handle(CreateMessageCommand request, CancellationToken cancellationToken)
//        //{
//        //    Console.WriteLine("Ram1");
//        //    return Task.CompletedTask;
//        //}
//    }

//    //public class CreateRamHandler2 : AsyncRequestHandler<CreateRam2>
//    //{
//    //    protected override Task Handle(CreateRam2 request, CancellationToken cancellationToken)
//    //    {
//    //        Console.WriteLine("Ram2");
//    //        return Task.CompletedTask;
//    //    }
//    //}

//    //public class DecoratorAutofac<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
//    //{
//    //    private readonly ILifetimeScope _container;

//    //    public DecoratorAutofac(ILifetimeScope container)
//    //    {
//    //        _container = container;
//    //    }

//    //    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
//    //    {
//    //        using (var child = _container.BeginLifetimeScope())
//    //        {
//    //           return await next();
//    //        }

//    //    }
//    //}


//    public class CommitUnitOfWork<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
//    {
//        private readonly IUnitOfWork _unitOfWork;

//        public CommitUnitOfWork(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }


//        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
//        {
//            var response = await next();
//            await _unitOfWork.CommitAsync(cancellationToken);
//            return response;

//        }
//    }

//    //public class Decorator2<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : CreateRam2
//    //{
//    //    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
//    //    {
//    //        Console.WriteLine("Decorator2B");
//    //        var response = await next();
//    //        Console.WriteLine("Decorator2A");
//    //        return response;

//    //    }
//    //}
//}