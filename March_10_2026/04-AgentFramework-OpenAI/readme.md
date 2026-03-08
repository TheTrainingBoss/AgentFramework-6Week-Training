## Creating a new dotnet project - console app

```powershell
dotnet new console -n OpenAIHelloWorld
```

You will need to bring in two Nuget packages for this first Hello World sample to work:

- `Azure.AI.OpenAI` it is still in beta, please use version `2.8.0-beta.1`
- `Microsoft.Agent.AI.OpenAI` it is a release candidate, please use version `1.0.0-rc3`

This is what your project file should look like:

```C#
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Azure.AI.OpenAI" Version="2.8.0-beta.1" />
    <PackageReference Include="Microsoft.Agents.AI.OpenAI" Version="1.0.0-rc3" />
  </ItemGroup>
</Project>
```

The `program.cs` file should contain the following:

```C#
using Microsoft.Agents.AI;
using OpenAI;
using OpenAI.Chat;

string apikey = "sk-proj-ztUwszoSUugVpqDPaNvfunVCz7iuZOH...."; //Please use your own apikey by registering at https://platform.openai.com

OpenAIClient client = new OpenAIClient(apikey);

ChatClientAgent agent = client.GetChatClient("gpt-5-mini").AsAIAgent();

AgentResponse response = await agent.RunAsync("Why is the sky blue?");

Console.WriteLine(response);
```

## Running the application

- Open a terminal
- Change directory to where your project is located
- Execute `dotnet run`


