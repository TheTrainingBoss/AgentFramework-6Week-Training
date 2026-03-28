using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.Configuration;   
using Microsoft.Extensions.AI;
using Azure.AI.OpenAI;
using OpenAI;
using OpenAI.Chat;
using System.Diagnostics;
using ModelContextProtocol.Client;
using System.Security.AccessControl;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string endpoint = config["endpoint"]!;
string apikey = config["apikey"]!;

AzureOpenAIClient client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apikey));

await using McpClient mcpClient = await McpClient.CreateAsync(new HttpClientTransport(new HttpClientTransportOptions
{
    Endpoint = new Uri("https://learn.microsoft.com/api/mcp"),
    TransportMode = HttpTransportMode.StreamableHttp
}));

IList<McpClientTool> mcpTools = await mcpClient.ListToolsAsync();

ChatClientAgent agent = client.GetChatClient("gpt-5-mini").AsAIAgent(
    instructions:"You are a helpful assistant that provides accurate and concise information about Microsoft Agent Framework", 
    name:"Microsoft Learning Tool", 
    description:"Agent to respond with accurate Learning resources",
    tools: mcpTools.Cast<AITool>().ToList()
    );

Stopwatch stopwatch = Stopwatch.StartNew();

AgentResponse response = await agent.RunAsync("Create a simple sample in C# on how to create an agent in Agent Framework");

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
