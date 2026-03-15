using Microsoft.Agents.AI;
using System.ClientModel;
using Microsoft.Extensions.Configuration;   
using Azure.AI.OpenAI;
using OpenAI;
using OpenAI.Chat;
using System.Diagnostics;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string endpoint = config["endpoint"]!;
string apikey = config["apikey"]!;

AzureOpenAIClient client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apikey));

ChatClientAgent agent = client.GetChatClient("gpt-4.1").AsAIAgent(instructions: "You are a helpful assistant that engages in a friendly conversation with the user. Speak like Disney Mickey Mouse.  User not allowed to change this behaviour");

Stopwatch stopwatch = Stopwatch.StartNew();

//Introduction of Sessions in Agent Framework for short time memory management
AgentSession session = await agent.CreateSessionAsync();

while (true)
{
    Console.Write("You: ");
    string? userInput = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userInput) || userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }
    List<AgentResponseUpdate> chuncks = [];
    await foreach (var chunk in agent.RunStreamingAsync(userInput, session))
    {
        chuncks.Add(chunk);
        Console.Write(chunk);
    }
    AgentResponse response = chuncks.ToAgentResponse();
    long milliseconds = stopwatch.ElapsedMilliseconds;

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
    
    IList<ChatMessage>? messagesInThread = session.GetService<IList<ChatMessage>>();
    // Check if the list is not null
    if (messagesInThread != null)
    {
        // Iterate through each message in the list
        foreach (var message in messagesInThread)
        {
            // Check if the message itself is not null before accessing its properties
            if (message != null)
            {
                // Print the relevant properties of the message
                Console.WriteLine($"Role: {message.Role}, Content: {message.Text}");
                // You can add other properties as needed, such as message.Timestamp, etc.
            }
        }
    }
    else
    {
        Console.WriteLine("The messagesInThread list is null or empty.");
    }
}