using System.Net;
using System.Runtime.Serialization;

namespace Zbang.Zbox.WcfRestService
{
    [DataContract]
    public class ResultDto<T> where T : class
    {
        private ResultDto(T data)
        {
            Data = data;
            HttpStatus = HttpStatusCode.OK;
        }
        private ResultDto(HttpStatusCode httpStatus, string error)
        {
            HttpStatus = httpStatus;
            Error = error;
        }

        [DataMember]
        public T Data { get; set; }
        [DataMember]
        public HttpStatusCode HttpStatus { get; set; }
        [DataMember]
        public string Error { get; set; }

        public static ResultDto<T> GetSuccessResult(T data)
        {
            return new ResultDto<T>(data);
        }

        public static ResultDto<T> GetErrorResult(HttpStatusCode httpStatus, string error)
        {
            return new ResultDto<T>(httpStatus, error);
        }
    }
}