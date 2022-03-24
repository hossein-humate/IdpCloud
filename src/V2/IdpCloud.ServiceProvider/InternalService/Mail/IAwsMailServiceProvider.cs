namespace IdpCloud.ServiceProvider.InternalService.Mail
{
    /// <summary>
    /// Interface of AWS Email service Provider
    /// </summary>
    public interface IAwsMailServiceProvider
    {
        /// <summary>
        /// Constructs Amazon Simple Email Service client when given valid AWS Access key, Secret Key and Region.
        /// and </summary>
        /// <returns>A <see cref="AwsMailService"/>Returns the AwsMailService instance</returns>
        IMailService Create();
    }
}
