using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;
using WebApiHandsOn.Entities;
using WebApiHandsOn.Helpers;

namespace WebApiHandsOn.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SecretsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, IAmazonSecretsManager> _secretsManagerClients;

        public SecretsController(IConfiguration configuration,Dictionary<string, IAmazonSecretsManager> secretsManagerClients)
        {
            _configuration = configuration;
            _secretsManagerClients = secretsManagerClients;
        }

        
        [HttpGet("manager1/JsonSecrets")]
        public async Task<IActionResult> GetSecretFromManager1()
        {
            var awsSecretsManagers = _configuration.GetSection("AwsSecretsManagers:manager1")
                .Get<AwsSecretsManagerSettings>();

            try
            {
                var manager1 = _secretsManagerClients["Manager1"];
                var secretValues = await GetSecretAsStringAsync(manager1, awsSecretsManagers.SecretManagerName);
                return Ok(new { JsonSecretValues = secretValues });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("manager1/{keyName}")]
        public async Task<IActionResult> GetSecretValueByKey(string keyName)
        {
            var awsSecretsManagers = _configuration.GetSection("AwsSecretsManagers:manager1")
                .Get<AwsSecretsManagerSettings>();

            try
            {
                var manager1 = _secretsManagerClients["Manager1"];
                var keyValue = await GetSecretValueByKeyAsync(manager1, awsSecretsManagers.SecretManagerName, keyName);

                // Return the key-value pair
                return Ok(new { Key = keyName, Value = keyValue });
            }
            catch (Exception ex) when (ex.Message.Contains($"The key '{keyName}' is missing"))
            {
                return NotFound(new { Error = $"Key '{keyName}' is missing in the secret." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("manager1/ObjectSecrets")]
        public async Task<IActionResult> GetAllSecretsFromManager1()
        {
            var awsSecretsManagers = _configuration.GetSection("AwsSecretsManagers:manager1")
                .Get<AwsSecretsManagerSettings>();

            try
            {
                var manager1 = _secretsManagerClients["Manager1"];
                var secretObject = await GetSecretAsObjectAsync<SecretValuesManager1>(manager1, awsSecretsManagers.SecretManagerName);

                return Ok(secretObject);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }


        [HttpGet("manager2/{secretName}")]
        public async Task<IActionResult> GetSecretFromManager2(string secretName)
        {
            try
            {
                var manager2 = _secretsManagerClients["Manager2"];
                var secretValue = await GetSecretAsStringAsync(manager2, secretName);
                return Ok(new { SecretValue = secretValue });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Helper method to retrieve a secret from AWS Secrets Manager.
        /// </summary>
        /// <param name="client">The AWS Secrets Manager client.</param>
        /// <param name="secretName">The name of the secret to retrieve.</param>
        /// <returns>The secret value as a string.</returns>
        private async Task<string> GetSecretAsStringAsync(IAmazonSecretsManager client, string secretName)
        {
            var request = new GetSecretValueRequest
            {
                SecretId = secretName
            };

            var response = await client.GetSecretValueAsync(request);

            // Return the secret value
            if (!string.IsNullOrEmpty(response.SecretString))
            {
                return response.SecretString;
            }
            else
            {
                // Handle binary secrets (if applicable)
                var memoryStream = response.SecretBinary;
                var reader = new System.IO.StreamReader(memoryStream);
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Helper method to retrieve a secret from AWS Secrets Manager.
        /// </summary>
        /// <param name="client">The AWS Secrets Manager client.</param>
        /// <param name="secretName">The name of the secret to retrieve.</param>
        /// <returns>The secret value as an object of generic type T.</returns>
        private async Task<T> GetSecretAsObjectAsync<T>(IAmazonSecretsManager client, string secretName)
        {
            var request = new GetSecretValueRequest
            {
                SecretId = secretName
            };

            var response = await client.GetSecretValueAsync(request);

            if (!string.IsNullOrEmpty(response.SecretString))
            {
                // Deserialize the JSON secret into the generic type T
                return JsonSerializer.Deserialize<T>(response.SecretString);
            }
            else
            {
                throw new Exception("Secret is not in JSON format.");
            }
        }

        /// <summary>
        /// Helper method to retrieves the secret values and dynamically extracts a key/value from a specific secret key name
        /// </summary>
        /// <param name="client">The AWS Secrets Manager client.</param>
        /// <param name="secretName">The name of the secret to retrieve.</param>
        /// <param name="keyName">The name of the specific secret key name to retrieve.</param>
        /// <returns>The secret value as a key/value object.</returns>
        private async Task<string> GetSecretValueByKeyAsync(IAmazonSecretsManager client, string secretName, string keyName)
        {
            var request = new GetSecretValueRequest
            {
                SecretId = secretName
            };

            var response = await client.GetSecretValueAsync(request);

            if (!string.IsNullOrEmpty(response.SecretString))
            {
                // Parse the JSON secret
                var secretJson = JsonNode.Parse(response.SecretString);

                // Extract the specified key
                if (secretJson != null && secretJson[keyName] != null)
                {
                    return secretJson[keyName]!.ToString();
                }
                else
                {
                    throw new Exception($"The key '{keyName}' is missing in the secret.");
                }
            }
            else
            {
                throw new Exception("Secret is not in JSON format.");
            }
        }

    }
}
