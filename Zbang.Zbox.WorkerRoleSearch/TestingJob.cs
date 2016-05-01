﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Zbang.Zbox.WorkerRoleSearch.Mail;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class TestingJob : IJob
    {
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var x = new List<Task<bool>>();
            var process = Infrastructure.Ioc.IocFactory.IocWrapper.TryResolve<IMailProcess>("digestEveryChange");
            if (process != null)
            {
                x.Add(process.ExecuteAsync(0, p => Task.FromResult(true), cancellationToken));
            }

            //process = Infrastructure.Ioc.IocFactory.IocWrapper.TryResolve<IMailProcess>("likesReport");
            //if (process != null)
            //{
            //    x.Add( process.ExecuteAsync(0, p =>
            //    {
            //    }, cancellationToken));
            //}
            //process = Infrastructure.Ioc.IocFactory.IocWrapper.TryResolve<IMailProcess>("universityLowActivity");
            //if (process != null)
            //{
            //    x.Add( process.ExecuteAsync(0, p =>
            //    {
            //    }, cancellationToken));
            //}

            //process = Infrastructure.Ioc.IocFactory.IocWrapper.TryResolve<IMailProcess>("followLowActivity");
            //if (process != null)
            //{
            //    x.Add( process.ExecuteAsync(0, p =>
            //    {
            //    }, cancellationToken));
            //}

            await Task.WhenAll(x);
            var result  =  x.All(a => a.Result);

        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
