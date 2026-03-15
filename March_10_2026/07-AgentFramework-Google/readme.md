## Getting your apikey from Google for the use of Gemini models

Head over to this [link](https://aistudio.google.com/) to get a Google API KEY

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
    <PackageReference Include="Google_GenerativeAI.Microsoft" Version="3.6.3" />
    <PackageReference Include="Microsoft.Agents.AI" Version="1.0.0-rc4" />
    <PackageReference Include="Microsoft.Extensions.AI" Version="10.4.0" />
  </ItemGroup>
</Project>
```

The content of the `program.cs` file is below:

```C#
using Microsoft.Extensions.AI;
using GenerativeAI.Microsoft;
using Microsoft.Agents.AI;
using System.Diagnostics;


IChatClient client = new GenerativeAIChatClient("AIzaSyBXZVdHRG6u8f... please use your own key", "gemini-3-flash-preview");
ChatClientAgent agent = new(client);

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