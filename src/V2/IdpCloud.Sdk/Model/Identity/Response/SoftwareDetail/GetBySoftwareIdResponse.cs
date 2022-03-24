namespace IdpCloud.Sdk.Model.Identity.Response.SoftwareDetail
{
    public class GetBySoftwareIdResponse : BaseResponse
    {
        public Identity.SoftwareDetail SoftwareDetail { get; set; }
        public Identity.Software Software { get; set; }
    }
}
