namespace WebApiHandsOn.Helpers
{
    public class AwsSecretsManagerSettings
    {
        public string SecretManagerName { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Region { get; set; }
    }
}