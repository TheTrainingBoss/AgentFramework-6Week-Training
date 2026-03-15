## Getting the Mistral API KEY to use with Agent Framework

Head over to this [link](https://console.mistral.ai/) to establish an account and create a new API KEY

The content of project file is below:

```C#
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.Agents.AI" Version="1.0.0-rc4" />
    <PackageReference Include="Microsoft.Extensions.AI" Version="10.4.0" />
    <PackageReference Include="Mistral.SDK" Version="2.3.1" />
  </ItemGroup>
</Project>

```

The Content of the `program.cs` file is below:

```C#
using System.Diagnostics;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Mistral.SDK;

MistralClient mistralClient = new MistralClient(new APIAuthentication("W4Yh27UbBBsT6PPS4zs9.... please use your own key"));

AIAgent agent = mistralClient.Completions.AsAIAgent(new ChatClientAgentOptions
{
    ChatOptions = new ChatOptions
    {
        ModelId = "mistral-small-2506"
    }
});

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
```