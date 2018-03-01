using System;
//using Cloudents.Core.Storage;

namespace Cloudents.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class LogAttribute : Attribute
    {
    }

    //public class QueueName2Attribute : Attribute
    //{
    //    public QueueName2Attribute(QueueName name)
    //    {
    //        Name = name;
    //    }

    //    public QueueName Name { get; private set; }
    //}
}