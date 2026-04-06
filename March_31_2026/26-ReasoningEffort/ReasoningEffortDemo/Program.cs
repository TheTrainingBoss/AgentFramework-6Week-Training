using System.ClientModel;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using OpenAI.Responses;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string endpoint = config["endpoint"]!;
string apikey = config["apikey"]!;

AzureOpenAIClient client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apikey), new AzureOpenAIClientOptions
{
    NetworkTimeout = TimeSpan.FromSeconds(60)
});

// This is set to the default of Medium for reasonning efforts
ChatClientAgent agent = client
        .GetChatClient("gpt-5-mini")
        .AsAIAgent();

AgentResponse response = await agent.RunAsync("Who is Bill Gates? and where was he born?");
Console.WriteLine(response);
Console.WriteLine("Input Token Count: " + response.Usage?.InputTokenCount);
Console.WriteLine("Output Token Count: " + response.Usage?.OutputTokenCount);
Console.WriteLine("Reasoning Token Count: " + response.Usage?.ReasoningTokenCount);

//Let's take control of the reasonning efforts

ChatClientAgent agentwithreasoningeffort = client
        .GetChatClient("gpt-5-mini")
        .AsAIAgent(
            //reasoningEffortLevel: ReasoningEffortLevel.Low  //Yeah no, not that easy :) 
            options: new ChatClientAgentOptions
            {
                ChatOptions = new ChatOptions
                {
#pragma warning disable OPENAI001
                    RawRepresentationFactory = _ => new ChatCompletionOptions
                    {
                        ReasoningEffortLevel = ChatReasoningEffortLevel.Minimal,                         
                    }
#pragma warning restore OPENAI001
                }
            }
        );

AgentResponse responseWithReasoningEffort = await agentwithreasoningeffort.RunAsync("Who is Bill Gates? and where was he born?");
Console.WriteLine(responseWithReasoningEffort);
Console.WriteLine("Input Token Count: " + responseWithReasoningEffort.Usage?.InputTokenCount);
Console.WriteLine("Output Token Count: " + responseWithReasoningEffort.Usage?.OutputTokenCount);
Console.WriteLine("Reasoning Token Count: " + responseWithReasoningEffort.Usage?.ReasoningTokenCount);