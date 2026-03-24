using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using OpenAI.Chat;
using Microsoft.Extensions.Configuration;
using System.ClientModel;
using System.Text;
using Microsoft.Extensions.AI;
using OpenAI.Responses;
using System.Diagnostics;
using System.ComponentModel;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string endpoint = config["endpoint"]!;
string apikey = config["apikey"]!;

AzureOpenAIClient client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apikey));

// This DOES NOT work with Hosted tools

// ChatClientAgent agent = client.GetChatClient("gpt-5").AsAIAgent(
//     instructions:"You are an AI assistant that helps people find information and you have access to the Web Search tool so use it for information that occured after your date of training.", 
//     name:"LinoBot", 
//     description:"General current info Bot",
//     tools: 
//         [ 
//             AIFunctionFactory.Create(GetDateTimeUtc),
//             new HostedWebSearchTool()
//         ]);
 
// We have to use GetResponsesClient for Hosted tools to work but it is not 100% yet, a bit buggy as of Feb 2nd 2026
#pragma warning disable OPENAI001
    ChatClientAgent agent = client
            .GetResponsesClient("gpt-5-mini")
#pragma warning restore OPENAI001
            .AsAIAgent(
                instructions:"You are an AI assistant that helps people find information and you have access to the Web Search tool so use it for information that occured after your date of training.", 
                tools:
                [
                    AIFunctionFactory.Create(GetDateTimeUtc),
                    new HostedWebSearchTool()
                ],
                name:"LinoBot", 
                description:"General current info Bot"
            );

Stopwatch stopwatch = Stopwatch.StartNew();

AgentResponse response = await agent.RunAsync("What is the score between Liverpool and Brighton in the Premier League that was played on March 21st 2026?, always include today's date at the top of your answers");
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

[Description("Gets the current date and time in UTC format")]
static DateTime GetDateTimeUtc()
{
    return DateTime.UtcNow;
}
