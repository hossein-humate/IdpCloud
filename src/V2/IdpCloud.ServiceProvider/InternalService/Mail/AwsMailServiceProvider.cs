using IdpCloud.Common.Settings;
using Microsoft.Extensions.Options;
using System;

namespace IdpCloud.ServiceProvider.InternalService.Mail
{
    ///<inheritdoc/>
    public class AwsMailServiceProvider : IAwsMailServiceProvider
    {
        private readonly MailServiceSetting _emailSettings;
        private readonly AwsSetting _awsSetting;

        /// <summary>
        /// Instantiates a new instance of <see cref="AwsMailServiceProvider"/>.
        /// </summary>
        /// <param name="emailOption">An instance of <see cref="IOptions{EmailSetting}<"/> to inject.</param>
        /// <param name="awsOption">An instance of <see cref="IOptions{AwsSetting}"/> to inject.</param>
        public AwsMailServiceProvider(IOptions<MailServiceSetting> emailOption,
          IOptions<AwsSetting> awsOption)
        {
            _awsSetting = awsOption == null ? throw new ArgumentNullException(nameof(awsOption)) : awsOption.Value;
            _emailSettings = emailOption == null ? throw new ArgumentNullException(nameof(emailOption)) : emailOption.Value;

        }

        ///<inheritdoc/>
        public IMailService Create()
        {
            //var client = new AmazonSimpleEmailServiceClient(_awsSetting.AccessKey, _awsSetting.SecretKey, RegionEndpoint.GetBySystemName(_awsSetting.Region));
            //var awsEmailService = new AwsMailService(client, _emailSettings);
            return default;
        }
    }
}
