namespace IdpCloud.Sdk.Model
{
    public class BaseResponse
    {
        public string Message { get; set; } = "Request Completed Successfully.";
        public RequestResult ResultCode { get; set; } = 0;
    }
}
