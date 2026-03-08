using System.ClientModel;
using Microsoft.Agents.AI;
using Azure.Identity;
using Azure.AI.Projects;
using Azure.AI.Projects.OpenAI;
using System.Diagnostics;

AIProjectClient client = new (new Uri("https://AgentFrameworkCourse.services.ai.azure.com/api/projects/AgentframeworkProject"), new AzureCliCredential());

// Define the agent you want to create. (Prompt Agent in this case)
AgentVersionCreationOptions options = new(new PromptAgentDefinition("gpt-5-mini") { Instructions = "You are a helpful assistant." });

AgentVersion createdAgentVersion = client.Agents.CreateAgentVersion(agentName: "MyAgent", options);

// You can use an AIAgent with an already created server side agent version.
AIAgent existingAgent = client.AsAIAgent(createdAgentVersion);

Stopwatch stopwatch = Stopwatch.StartNew();

AgentResponse response = await existingAgent.RunAsync("Why is the sky blue?");

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