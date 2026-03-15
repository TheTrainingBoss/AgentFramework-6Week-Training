## Getting an apikey from Anthropic

Use [link](https://platform.claude.com/) to establish an apikey for working with Anthropic models

The content of the project file is below:

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
    <PackageReference Include="Microsoft.Agents.AI.Anthropic" Version="1.0.0-rc4" />
    <PackageReference Include="Microsoft.Extensions.AI" Version="10.4.0" />
  </ItemGroup>
</Project>
```


The content of the `program.cs` file is below:

```C#
using Anthropic;
using Anthropic.Core;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Diagnostics;

var client = new AnthropicClient(new ClientOptions
{
    ApiKey = "sk-ant-api03-JOhjimepH0gNUIPQSiRqge4P... please use your own key"
});

ChatClientAgent agent = client.AsIChatClient().AsAIAgent(new ChatClientAgentOptions
{
    ChatOptions = new ChatOptions
    {
        MaxOutputTokens = 2000,
        ModelId="claude-sonnet-4-5-20250929"
    }
});

Stopwatch stopwatch = Stopwatch.StartNew();

AgentResponse response = await agent.RunAsync("Why is the sky blue?");

long milliseconds = stopwatch.ElapsedMilliseconds;

Console.WriteLine(response);
Console.WriteLine();

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