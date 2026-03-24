using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.Configuration;   
using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using OpenAI;
using OpenAI.Chat;
using System.Diagnostics;
using System.ComponentModel;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string endpoint = config["endpoint"]!;
string apikey = config["apikey"]!;

AzureOpenAIClient client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apikey));

Weather weatherTool = new Weather();

ChatClientAgent agent = client.GetChatClient("gpt-5-mini").AsAIAgent(
    new ChatClientAgentOptions
    {
        Name = "Weather Bot",
        Description = "Weather Bot that provides current weather information",
        ChatOptions = new ChatOptions
        {
            Instructions = "You are a helpful bot that can call functions to get current weathers based on locations",
            Temperature = 0.2f,
            TopK = 5,
            TopP = 0.9f,
            PresencePenalty = 0.1f,
            FrequencyPenalty = 0.1f,
            MaxOutputTokens = 2500,
            StopSequences = ["Observation:"],
            Tools =
            [
                AIFunctionFactory.Create(weatherTool.GetCurrentWeather, "get_weather", "Get the current weather for a given location.")
            ]
        }
    });

Stopwatch stopwatch = Stopwatch.StartNew();

AgentResponse response = await agent.RunAsync("What is the current weather in New York?");

long milliseconds = stopwatch.ElapsedMilliseconds;

Console.WriteLine(response);
if (response.Usage != null)
{
    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine();
    Console.WriteLine($"Input Tokens: {response.Usage.InputTokenCount}");
    Console.WriteLine($"Cached Tokens: {response.Usage.CachedInputTokenCount ?? 0}");
    Console.WriteLine($"Output Tokens: {response.Usage.OutputTokenCount}");
    Console.WriteLine($"Reasoning Tokens: {response.Usage.ReasoningTokenCount ?? 0}");
    Console.WriteLine($"Total Tokens: {response.Usage.TotalTokenCount}");
    Console.ResetColor();
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"Time spent: {milliseconds} ms");
Console.ResetColor();

public class Weather()
{
    [Description("Get the current weather for a given location.")]
    public string GetCurrentWeather(string location)
    {
        // In a real implementation, this method would call a weather API.
        Console.WriteLine($"Fetching weather for {location}...");
        return $"The current weather in {location} is sunny with a temperature of 75°F.";
    }
}
