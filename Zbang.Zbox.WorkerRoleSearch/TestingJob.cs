using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.WorkerRoleSearch.DomainProcess;
using Zbang.Zbox.WorkerRoleSearch.Mail;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class TestingJob : IJob
    {
        public async Task RunAsync(CancellationToken cancellationToken)
        {

            
            var y = Infrastructure.Ioc.IocFactory.IocWrapper.Resolve<IDomainProcess>(Infrastructure.Transport.DomainProcess.UpdateResolver);
            await y.ExecuteAsync(new UpdateData(878781, 136460, questionId: Guid.Parse("32978dbe-d31c-4ac3-92d0-a5cb00e8549a")), cancellationToken);

            //await y.GetUnsubscribersAsync(1, cancellationToken);

            //var x = new List<Task<bool>>();
            //var process = Infrastructure.Ioc.IocFactory.IocWrapper.TryResolve<IMailProcess>("digestEveryChange");
            //if (process != null)
            //{
            //    x.Add(process.ExecuteAsync(0, p => Task.FromResult(true), cancellationToken));
            //}

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

            //await Task.WhenAll(x);
            //var result = x.All(a => a.Result);

        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
