﻿using System;
using Microsoft.Azure.WebJobs.Description;

namespace Cloudents.MailGunFunctions.Di
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
    }
}
