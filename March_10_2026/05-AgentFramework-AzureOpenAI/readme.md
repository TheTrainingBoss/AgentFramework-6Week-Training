## dotnet user-secrets
- To initialize your user secrets in your project:

```cli
dotnet user-secrets init
```

- To list all the current secrets in the project:

```cli
dotnet user-secrets list
```

- To add a secret to the project:

```cli
dotnet user-secrets set "secret Name" "value"
```

- To remove a secret from the project:

```cli
dotnet user-secrets remove "Secret Name"
```

## Adding models to Microsoft Foundry
Head over to the Microsoft Foundry Project we created and add a couple of models to use in our experimentations with Agent Framework
- GPT-5-mini
- GPT-5-nano
- GPT-4.1
- Embedding-ada-002 (we will use later)

This is how your project file should look like:

```C#
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <UserSecretsId>b277d57f-cd7f-44e5-b65f-60a3e528b5a3</UserSecretsId>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Azure.AI.OpenAI" Version="2.9.0-beta.1" />
    <PackageReference Include="Microsoft.Agents.AI.OpenAI" Version="1.0.0-rc4" />
    <PackageReference Include="Microsoft.Extensions.Configuration" Version="10.0.5" />
    <PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="10.0.5" />
  </ItemGroup>
</Project>
```

This is how your `program.cs` file should look like:

```C#
using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.Configuration;   
using Azure.AI.OpenAI;
using OpenAI;
using OpenAI.Chat;
using System.Diagnostics;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string endpoint = config["endpoint"]!;
string apikey = config["apikey"]!;

AzureOpenAIClient client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apikey));

ChatClientAgent agent = client.GetChatClient("gpt-5-mini").AsAIAgent();

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