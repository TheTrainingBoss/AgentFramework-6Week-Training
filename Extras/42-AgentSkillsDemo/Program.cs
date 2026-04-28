using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;
using Azure.AI.OpenAI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string endpoint = config["endpoint"]!;
string apikey = config["apikey"]!;

AzureOpenAIClient client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apikey));
#pragma warning disable MAAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var path = Path.Combine(Directory.GetCurrentDirectory(), "AgentSkills");
Console.WriteLine($"Loading skills from: {path}");
var skillsProvider = new AgentSkillsProvider(path);
#pragma warning restore MAAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
AIAgent agent = client.GetChatClient("gpt-5-deploy")
    .AsAIAgent(new ChatClientAgentOptions
    {
        Name = "Mickey Mouse",
        ChatOptions = new()
        {
            Instructions = "You are a helpful assistant. Answer questions and complete tasks for the user to the best of your ability. If you don't know the answer to a question, say you don't know.",
        },
        AIContextProviders = [skillsProvider],
    });
#pragma warning restore OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

AgentResponse response = await agent.RunAsync("Why is the sky blue? use the skill available to you");
Console.WriteLine($"Agent: {response.Text}");