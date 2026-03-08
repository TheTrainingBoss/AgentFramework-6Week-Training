## Getting Grok4 apikey from xAI

Head over to this [link](https://console.x.ai/) and login or create a new account to esablish the ability to create an api key.

The content of the project file in below:

```C#
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.Agents.AI.OpenAI" Version="1.0.0-rc3" />
  </ItemGroup>
</Project>
```

The content of the `program.cs` file is below:

```C#
using System.ClientModel;
using System.Diagnostics;
using Microsoft.Agents.AI;
using OpenAI;
using OpenAI.Chat;

OpenAIClient client = new (new ApiKeyCredential("xai-LFAYlBktn3NWdaLbalASh4aRJb5ycSpVulIdeH8tmicm... please use your own key"), new OpenAIClientOptions
{
    Endpoint = new Uri("https://api.x.ai/v1")
});

ChatClientAgent agent = client.GetChatClient("grok-4-fast-non-reasoning").AsAIAgent();

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