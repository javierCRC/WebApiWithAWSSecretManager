using System.Text.Json.Serialization;

namespace WebApiHandsOn.Entities
{
    public class SecretValuesManager1
    {
        [JsonPropertyName("ConnectionToMySQLDBCloud")]
        public string ConnectionToMySQLDBCloud { get; set; }

        [JsonPropertyName("TestMyName")]
        public string TestMyName { get; set; }
    }
}