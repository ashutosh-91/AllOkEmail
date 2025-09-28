using System.Net;
using AllOkEmail.Enums;

namespace AllOkEmail.Extensions
{
    public class ApiResponse
    {
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }

        public string Address { get; set; }
        public string Reason { get; set; }
        public string Domain { get; set; }
        
        public Status Status { get; set; }

        public SubStatus SubStatus { get; set; }


        public ApiResponse() { }

        public ApiResponse(bool isSuccessful , string message)
        {
            IsSuccess = isSuccessful;
            Message = message;
        }

        public ApiResponse(bool isSuccessful, string message, HttpStatusCode code)
        {
            IsSuccess = isSuccessful;
            Message = message;
            StatusCode = code;

        }
        public ApiResponse(bool isSuccessful, string message, HttpStatusCode code,Status status,SubStatus subStatus)
        {
            IsSuccess = isSuccessful;
            Message = message;
            StatusCode = code;
            Status = status;
            SubStatus = subStatus;
        }
        public ApiResponse(bool isSuccessful, HttpStatusCode code,string message, string reason,string address,string domain)
        {
            IsSuccess = isSuccessful;
            StatusCode = code;
            Reason = reason;
            Domain = domain;
            Address = address;
            Message= message;

        }

    }
}
