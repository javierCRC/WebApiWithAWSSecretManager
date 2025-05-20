
using Amazon;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using WebApiHandsOn.Helpers;

namespace WebApiHandsOn.Modules.AWSSecretsManager
{
    public static class AwsSecretsManagerExtensions
    {
        public static IServiceCollection AddAwsSecretsManagers(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind configuration to a dictionary
            var awsSecretsManagers = configuration.GetSection("AwsSecretsManagers")
                .Get<Dictionary<string, AwsSecretsManagerSettings>>();

            if (awsSecretsManagers == null)
            {
                throw new InvalidOperationException("AWS Secrets Manager configuration is missing or invalid.");
            }

            // Create a dictionary to hold the clients
            var clients = new Dictionary<string, IAmazonSecretsManager>();

            // Register each Secrets Manager client
            foreach (var (key, settings) in awsSecretsManagers)
            {
                var awsCredentials = new BasicAWSCredentials(settings.AccessKey, settings.SecretKey);
                var region = RegionEndpoint.GetBySystemName(settings.Region);

                // Create and store the client
                clients[key] = new AmazonSecretsManagerClient(awsCredentials, region);
            }

            // Register the dictionary as a singleton
            services.AddSingleton(clients);

            // Return the IServiceCollection for chaining
            return services;
        }
    }
}