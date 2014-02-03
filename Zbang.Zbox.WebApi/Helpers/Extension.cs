using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Zbang.Zbox.WebApi.Helpers
{
    public static class Extension
    {
        public static HttpResponseMessage CreateZboxErrorResponse(this HttpRequestMessage request, HttpStatusCode statusCode, string error)
        {
            HttpError myCustomError = new HttpError();
            myCustomError.Add("HttpStatus", (int)statusCode);
            myCustomError.Add("Error", error);
            myCustomError.Add("Data", string.Empty);

            return request.CreateErrorResponse(statusCode, myCustomError);
        }

        public static HttpResponseMessage CreateZboxErrorResponse(this HttpRequestMessage request, ModelStateDictionary modelState)
        {
            HttpError myCustomError = new HttpError();
            myCustomError.Add("HttpStatus", (int)HttpStatusCode.BadRequest);
            foreach (KeyValuePair<string, ModelState> keyModelStatePair in modelState)
            {
                ModelErrorCollection errors = keyModelStatePair.Value.Errors;
                if (errors != null && errors.Count > 0)
                {
                    myCustomError.Add("Error", errors.First().ErrorMessage);
                    break;
                }
            }

            myCustomError.Add("Data", string.Empty);
            return request.CreateErrorResponse(HttpStatusCode.BadRequest, myCustomError);
        }

        public static HttpResponseMessage CreateZboxOkResult<T>(this HttpRequestMessage request, T value,HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var retVal = new
            {
                HttpStatus = (int)statusCode,
                Data = value,
                Error = string.Empty
            };

            return request.CreateResponse(statusCode, retVal);
        }

       

    }
}