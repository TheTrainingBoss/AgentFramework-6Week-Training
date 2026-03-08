The content of the project file is below"

```C#
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <EnablePreviewFeatures>True</EnablePreviewFeatures>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Azure.AI.Projects" Version="2.0.0-beta.1" />
    <PackageReference Include="Azure.Identity" Version="1.18.0" />
    <PackageReference Include="Microsoft.Agents.AI.AzureAI" Version="1.0.0-rc3" />
    <PackageReference Include="Azure.AI.OpenAI" Version="2.8.0-beta.1" />
    <PackageReference Include="Microsoft.Agents.AI.OpenAI" Version="1.0.0-rc3" />
  </ItemGroup>
</Project>
```


The content fo the `program.cs` file is below:

```C#
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
```