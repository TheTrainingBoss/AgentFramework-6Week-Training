using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.ClientModel;
using OpenAI.Chat;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string endpoint = config["endpoint"]!;
string apikey = config["apikey"]!;

AzureOpenAIClient client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apikey));

ChatClientAgent agent = client
    .GetChatClient("gpt-5")
    .AsAIAgent(tools: [AIFunctionFactory.Create(GetWeather, name: "get_weather")]);

//AG-UI Part begin
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddAGUI();
WebApplication app = builder.Build();

app.MapAGUI("/", agent);

await app.RunAsync();

//Server-Tool
static string GetWeather(string city)
{
    return "It is sunny and 69 degrees";
}