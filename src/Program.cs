using Amazon.Runtime;
using WebApiHandsOn.Modules.AWSSecretsManager;
using WebApiHandsOn.Modules.Features;
using WebApiHandsOn.Modules.Swagger;

using Amazon;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAwsSecretsManagers(builder.Configuration); // Adding and register multiple AWS Secrets Manager clients using this extension method

// Load secrets from AWS Secrets Manager with Kralizek

var awsAccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
var awsSecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");

var awsCredentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);

// Add SecretsManager to configuration
builder.Configuration.AddSecretsManager(
    credentials: awsCredentials,
    region: RegionEndpoint.USEast1,
    configurator: options =>
    {

        options.SecretFilter = entry => entry.Name.StartsWith("ElPillazo-Api-Marketing-ASM");

        options.KeyGenerator = (secret, key) =>
            key.Replace("__", ":").Replace("ElPillazo-Api-Marketing-ASM:", "");
    });

/* for debugging purposes
foreach (var kvp in builder.Configuration.AsEnumerable())
{
    Console.WriteLine($"[Config] {kvp.Key} = {kvp.Value}");
}*/

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwagger();                       
builder.Services.AddFeature(builder.Configuration); 

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) {

    // Ensure Swagger is available for both environments (Development and Production)
    app.MapOpenApi();

    
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger UI Modified V.3");
        c.RoutePrefix = string.Empty;

        
        var vPersonalised = Convert.ToBoolean(builder.Configuration["CustomSwaggerUi:Personalised"]);
        if (vPersonalised) 
        { 
            c.DocumentTitle = builder.Configuration["CustomSwaggerUi:DocTitle"]; 
            c.HeadContent = builder.Configuration["CustomSwaggerUi:HeaderImg"];  
            c.InjectStylesheet(builder.Configuration["CustomSwaggerUi:PathCss"]); 
        }; //https://cutt.ly/ZKbPeDm
    });
//}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();