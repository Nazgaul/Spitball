﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;

namespace Cloudents.Web.Framework
{
    /// <summary>
    /// Represents an <see cref="ActionResult"/> that when executed will
    /// execute a callback to write the file content out as a stream.
    /// </summary>
    public class FileCallbackResult : FileResult
    {
        private Func<Stream, ActionContext, Task> _callback;

        /// <summary>
        /// Creates a new <see cref="FileCallbackResult"/> instance.
        /// </summary>
        /// <param name="contentType">The Content-Type header of the response.</param>
        /// <param name="callback">The stream with the file.</param>
        public FileCallbackResult(string contentType, Func<Stream, ActionContext, Task> callback)
            : this(MediaTypeHeaderValue.Parse(contentType), callback)
        {
        }

        /// <summary>
        /// Creates a new <see cref="FileCallbackResult"/> instance.
        /// </summary>
        /// <param name="contentType">The Content-Type header of the response.</param>
        /// <param name="callback">The stream with the file.</param>
        public FileCallbackResult(MediaTypeHeaderValue contentType, Func<Stream, ActionContext, Task> callback)
            : base(contentType?.ToString())
        {
            Callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        /// <summary>
        /// Gets or sets the callback responsible for writing the file content to the output stream.
        /// </summary>
        private Func<Stream, ActionContext, Task> Callback
        {
            get => _callback;
            set
            {
                _callback = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <inheritdoc />
        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var executor = new FileCallbackResultExecutor(context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>());
            return executor.ExecuteAsync(context, this);
        }

        private sealed class FileCallbackResultExecutor : FileResultExecutorBase
        {
            public FileCallbackResultExecutor(ILoggerFactory loggerFactory)
                : base(CreateLogger<FileCallbackResultExecutor>(loggerFactory))
            {
            }

            [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
            public async Task ExecuteAsync(ActionContext context, FileCallbackResult result)
            {
                //https://github.com/aspnet/AspNetCore/issues/7644
                var syncIoFeature = context.HttpContext.Features.Get<IHttpBodyControlFeature>();
                if (syncIoFeature != null)
                {
                    syncIoFeature.AllowSynchronousIO = true;
                }
                SetHeadersAndLog(context, result, null, true);
                
                await result.Callback(context.HttpContext.Response.Body, context);
                //await context.HttpContext.Response.Body.FlushAsync();
            }
        }
    }
}