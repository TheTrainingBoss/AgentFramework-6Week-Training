using System.ClientModel;
using Microsoft.Agents.AI;
using Microsoft.AI.Foundry.Local;
using OpenAI;
using System.Diagnostics;
using OpenAI.Chat;

string modelAlias = "phi-4-mini";
Console.WriteLine($"Starting AI Model '{modelAlias}'. If not already started / cached this might take a while...");

FoundryLocalManager manager = await FoundryLocalManager.StartModelAsync(modelAlias);
ModelInfo? modelInfo = await manager.GetModelInfoAsync(modelAlias);

OpenAIClient client = new(new ApiKeyCredential("NO_API_KEY"), new OpenAIClientOptions
{
    Endpoint = manager.Endpoint
});
ChatClientAgent agent = client.GetChatClient(modelInfo!.ModelId).AsAIAgent();

Stopwatch stopwatch = Stopwatch.StartNew();

AgentResponse response = await agent.RunAsync("Why is the sky blue?");

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
