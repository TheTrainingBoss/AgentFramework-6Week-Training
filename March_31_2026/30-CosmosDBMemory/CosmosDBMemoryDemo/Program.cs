using Microsoft.Agents;
using Microsoft.Agents.AI;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Azure.AI.OpenAI;
using System.ClientModel;
using Microsoft.Azure.Cosmos;
using Microsoft.Agents.AI.Compaction;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

var cosmosClient = new CosmosClient(
    "<enter-your-cosmos-endpoint-here>",
    "<enter-your-cosmos-account-key-here>"
);

var database = cosmosClient.GetDatabase("AgentFramework");
var container = database.GetContainer("chathistory");

string conversationId = "LinoConversation"; //This is where you will have to decide on the conversationId strategy that best fits your scenario, it can be a fixed value for single conversation scenarios or something more dynamic like a session id or a combination of tenantId+userId for multi-tenant/multi-user scenarios. The important thing is that this value is used consistently across calls so that the chat history can be retrieved correctly.

var historyProvider = new CosmosChatHistoryProvider(
    cosmosClient: cosmosClient,
    databaseId: database.Id,
    containerId: container.Id,
    stateInitializer: _ => new CosmosChatHistoryProvider.State(
        conversationId: conversationId,
        tenantId: "default-tenant",
        userId: "default-user"
    ),
    ownsClient: false
);

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string endpoint = config["endpoint"]!;
string apikey = config["apikey"]!;

AzureOpenAIClient client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apikey));

#pragma warning disable MAAI001
var compactionStrategy = new SummarizationCompactionStrategy(
    chatClient: client.GetChatClient("gpt-5-mini").AsIChatClient(),
    trigger: CompactionTriggers.MessagesExceed(8)
);
var compactionProvider = new CompactionProvider(compactionStrategy);
#pragma warning restore MAAI001

var agentOptions = new ChatClientAgentOptions();
agentOptions.ChatHistoryProvider = historyProvider;
agentOptions.AIContextProviders = [compactionProvider];

string question =  "What are the top 10 Movies according to IMDB?"; //"ok can you give the top 5 only";

AIAgent agent = client
    .GetChatClient("gpt-5")
    .AsAIAgent(agentOptions);

AgentSession session = await agent.CreateSessionAsync();

AgentResponse response = await agent.RunAsync(question, session);
Console.WriteLine(response);