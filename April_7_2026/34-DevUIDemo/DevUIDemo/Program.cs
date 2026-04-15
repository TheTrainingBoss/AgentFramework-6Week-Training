using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.AI; 
using Azure.AI.OpenAI;
using OpenAI;
using OpenAI.Chat;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.Hosting;
using Microsoft.Agents.AI.DevUI;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string endpoint = config["endpoint"]!;
string apikey = config["apikey"]!;

AzureOpenAIClient client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apikey));

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Register Services needed to run DevUI
builder.Services.AddChatClient(client.GetChatClient("gpt-5").AsIChatClient()); //You need to register a chat client for the dummy agents to use
builder.Services.AddOpenAIResponses();
builder.Services.AddOpenAIConversations();

// Register "dummy" Agent
builder.AddAIAgent("The Egyptian King", "You are an Egyptian Pharaoh and you speak like one") //Agents registered this way will be created on demand and will be transient. You can also specify a factory method if you want more control over creation
    .WithAITool(AIFunctionFactory.Create(GetWeather));

//Build a "normal" Agent
string realAgentName = "Real Agent";
AIAgent myAgent = client
    .GetChatClient("gpt-5")
    .AsAIAgent(name: realAgentName, instructions: "Speak like a pirate", tools: [AIFunctionFactory.Create(GetWeather)]);

builder.AddAIAgent(realAgentName, (serviceProvider, key) => myAgent); //Get registered as a keyed singleton so name on real agent and key must match

// Register sample workflows
IHostedAgentBuilder frenchTranslator = builder.AddAIAgent("french-translator", "Translate any text you get into French");
IHostedAgentBuilder arabicTranslator = builder.AddAIAgent("arabic-translator", "Translate any text you get into Arabic");

builder.AddWorkflow("translation-workflow-sequential", (sp, key) =>
{
    IEnumerable<AIAgent> agentsForWorkflow = new List<IHostedAgentBuilder>() { frenchTranslator, arabicTranslator }.Select(ab => sp.GetRequiredKeyedService<AIAgent>(ab.Name));
    return AgentWorkflowBuilder.BuildSequential(workflowName: key, agents: agentsForWorkflow);
}).AddAsAIAgent();

builder.AddWorkflow("translation-workflow-concurrent", (sp, key) =>
{
    IEnumerable<AIAgent> agentsForWorkflow = new List<IHostedAgentBuilder>() { frenchTranslator, arabicTranslator }.Select(ab => sp.GetRequiredKeyedService<AIAgent>(ab.Name));
    return AgentWorkflowBuilder.BuildConcurrent(workflowName: key, agents: agentsForWorkflow);
}).AddAsAIAgent();

WebApplication app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    //Needed for DevUI to function 
    app.MapOpenAIResponses();
    app.MapOpenAIConversations();
    app.MapDevUI();
}

app.Run();


static string GetWeather(string city)
{
    return "It is sunny and 72 degrees";
}
