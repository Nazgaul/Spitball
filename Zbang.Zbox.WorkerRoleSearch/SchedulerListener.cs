using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Autofac.Features.Indexed;
using Cloudents.Core.Extension;
using Microsoft.WindowsAzure.Scheduler.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class SchedulerListener : IJob, IDisposable
    {
        private readonly IQueueProviderExtract _queueProviderExtract;
        private readonly IIndex<string, ISchedulerProcess> _scope;
        //private readonly ILifetimeScope m_LifetimeScope;
        private readonly XmlSerializer _dcs = new XmlSerializer(typeof(StorageQueueMessage));
        private readonly SemaphoreSlim _criticalCode = new SemaphoreSlim(1);
        private readonly ILogger _logger;

        public SchedulerListener(IQueueProviderExtract queueProviderExtract,  ILogger logger, IIndex<string, ISchedulerProcess> scope)
        {
            _queueProviderExtract = queueProviderExtract;
            _logger = logger;
            _scope = scope;
        }

        public string Name => nameof(SchedulerListener);

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var queueName = new SchedulerQueueName();

                    await _queueProviderExtract.RunQueueAsync(queueName, async msg =>
                    {
                        StorageQueueMessage message;
                        using (var xmlStream = new MemoryStream(Encoding.Unicode.GetBytes(msg.AsString)))
                        {
                            message = (StorageQueueMessage)_dcs.Deserialize(xmlStream);
                        }
                        _logger.Info($"{Name} found message {msg.AsString}");
                        var messageContent = JObject.Parse(message.Message);
                        var properties = messageContent.Properties();
                        var list = new List<Task<bool>>();
                        foreach (var property in properties)
                        {
                            var t = (int?)property;
                            _scope.TryGetValue(property.Name, out var process);
                            if (process != null)
                            {
                                list.Add(process.ExecuteAsync(t ?? 0, async (progress,visible) =>
                                {
                                    property.Value = progress;
                                    message.Message = JsonConvert.SerializeObject(messageContent);
                                    using (var memoryStream = new MemoryStream())
                                    {
                                        await _criticalCode.WaitAsync(cancellationToken).ConfigureAwait(false);
                                        try
                                        {
                                            _dcs.Serialize(memoryStream, message);
                                            msg.SetMessageContent(memoryStream.ToArray());
                                            await _queueProviderExtract.UpdateMessageAsync(queueName, msg, visible, cancellationToken).ConfigureAwait(false);
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.Exception(ex, new Dictionary<string, string> {["service"] = Name });
                                        }
                                        finally
                                        {
                                            _criticalCode.Release();
                                        }
                                    }
                                }, cancellationToken));
                            }
                            else
                            {
                                list.Add(TaskCompleted.CompletedTaskFalse);
                                _logger.Warning($"can't resolve {property.Name}");
                            }
                        }
                        await Task.WhenAll(list).ConfigureAwait(false);
                        return list.All(a => a.Result);
                    }, TimeSpan.FromMinutes(15), 3, cancellationToken).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Exception(ex, new Dictionary<string, string> {["service"] = Name });
                }
                await Task.Delay(TimeSpan.FromMinutes(2), cancellationToken).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            _criticalCode?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
