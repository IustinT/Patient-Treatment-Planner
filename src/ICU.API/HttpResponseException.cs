
using System;
using System.Net;
using System.Net.Http;

namespace ICU.API
{
    public class HttpResponseException : Exception
    {
        public HttpResponseException(HttpResponseMessage httpResponseMessage)
        {
            HttpResponseMessage = httpResponseMessage;
            Status = httpResponseMessage.StatusCode;
            Value = httpResponseMessage.Content;
        }

        public HttpStatusCode Status { get; set; }

        public HttpResponseException(HttpStatusCode status, object value)
        {
            Status = status;
            Value = value;
        }

        public object Value { get; set; }
        public HttpResponseMessage HttpResponseMessage { get; }
    }
}