﻿
namespace Zbang.Cloudents.Mvc2Jared.Helpers
{
    public class JsonResponse
    {
        public JsonResponse(bool success)
        {
            Success = success;

        }

        public JsonResponse(bool success, object payload)
            : this(success)
        {
            Payload = payload;
        }


        public bool Success { get; private set; }
        public object Payload { get; set; }
    }
}