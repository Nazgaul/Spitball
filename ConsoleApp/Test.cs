

//namespace ConsoleApp
//{

//    public class OrderController : Controller
//    {
//        private readonly IRepository<Order> _orderRepository;
//        private readonly IEventPublisher _eventPublisher;

//        public OrderController(IRepository<Order> orderRepository,
//            IEventPublisher eventPublisher)
//        {
//            _orderRepository = orderRepository;
//            _eventPublisher = eventPublisher;
//        }

//        public ActionResult SubmitOrder(OrderViewModel viewModel)
//        {
//            try
//            {
//                if (ModelState.IsValid)
//                {
//                    var order = MapOrder(viewModel);
//                    _orderRepository.Add(order);
//                    _orderRepository.SaveChanges();

//                    _eventPublisher.Publish(new OrderSubmittedEvent { OrderId = order.Id });

//                    //Display success message
//                    //ViewInfo.AddSuccessMessage(Language.SubmitOrderSuccess);
//                }
//            }
//            catch
//            {
//                ModelState.AddModelError("__Form", Language.SubmitOrderError);
//            }

//            return View(viewModel);
//        }

//        //other
//    }
//    public interface IConsumer<T>
//    {
//        void Handle(T eventMessage);
//    }

   

//    public class NotifyWarehouse : IConsumer<OrderSubmittedEvent>
//    {
//        public void Handle(OrderSubmittedEvent eventMessage)
//        {
//            //notify warehouse
//        }
//    }

//    public class DeductOnHandInventory : IConsumer<OrderSubmittedEvent>
//    {
//        public void Handle(OrderSubmittedEvent eventMessage)
//        {
//            //deduct inventory
//        }
//    }


//    public class EmailOrderConfirmation : IConsumer<OrderSubmittedEvent>
//    {
//        private readonly IRepository<Order> _orderRepository;
//        private readonly ISmtpService _smtpService;
//        private readonly ILogger _logger;

//        public EmailOrderConfirmation(IRepository<Order> orderRepository,
//            ISmtpService smtpService,
//            ILogger logger)
//        {
//            _orderRepository = orderRepository;
//            _smtpService = smtpService;
//            _logger = logger;
//        }

//        public void Handle(OrderSubmittedEvent eventMessage)
//        {
//            var order = _orderRepository.Single(x => x.Id == eventMessage.OrderId);
//            var message = new SmtpMessage();
//            //get customer info from order  & populate message
//            _smtpService.SendMessage(message);
//        }
//    }

//    public interface ISubscriptionService
//    {
//        IEnumerable<IConsumer<T>> GetSubscriptions<T>();
//    }

//    public class EventSubscriptions : ISubscriptionService
//    {
//        public static void Add<T>()
//        {
//            var consumerType = typeof(T);

//            consumerType.GetInterfaces()
//                .Where(x => x.IsGenericType)
//                .Where(x => x.GetGenericTypeDefinition() == typeof(IConsumer<>))
//                .ToList()
//                .ForEach(x => IoC.Container.RegisterType(x,
//                    consumerType,
//                    consumerType.FullName));
//        }

//        public IEnumerable<IConsumer<T>> GetSubscriptions<T>()
//        {
//            var consumers = IoC.Container.ResolveAll(typeof(IConsumer<T>));
//            return consumers.Cast<IConsumer<T>>();
//        }
//    }

//    public interface IEventPublisher
//    {
//        void Publish<T>(T eventMessage);
//    }

//    public class EventPublisher : IEventPublisher
//    {
//        private readonly ISubscriptionService _subscriptionService;

//        public EventPublisher(ISubscriptionService subscriptionService)
//        {
//            _subscriptionService = subscriptionService;
//        }

//        public void Publish<T>(T eventMessage)
//        {
//            var subscriptions = _subscriptionService.GetSubscriptions<T>();
//            subscriptions.ToList().ForEach(x => PublishToConsumer(x, eventMessage));
//        }

//        private static void PublishToConsumer<T>(IConsumer<T> x, T eventMessage)
//        {
//            try
//            {
//                x.Handle(eventMessage);
//            }
//            catch (Exception e)
//            {
//                //log and handle internally
//            }
//            finally
//            {
//                var instance = x as IDisposable;
//                if (instance != null)
//                {
//                    instance.Dispose();
//                }
//            }
//        }
//    }
//}
