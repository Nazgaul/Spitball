﻿
using System;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Threading;
using System.Web;

namespace Zbang.Cloudents.MobileApp.Extensions
{
    public static class HttpRequestMessageExtensions
    {
        public static string GetClientIp(this HttpRequestMessage request)
        {
            if (request.IsLocal())
            {
                return "86.143.189.86";
            }
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                var prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            if (request.Properties.ContainsKey("MS_OwinContext"))
            {
                dynamic owinContext = request.Properties["MS_OwinContext"];
                if (owinContext != null)
                {
                    return owinContext.Request.RemoteIpAddress;
                }
            }

            return null;
        }


        public static CancellationToken GetCancellationToken(this HttpRequestMessage request)
        {
            return HttpContext.Current.Response.ClientDisconnectedToken;
        }
    }
}