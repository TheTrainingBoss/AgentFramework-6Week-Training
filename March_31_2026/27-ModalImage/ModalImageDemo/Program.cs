using System.ClientModel;
using Azure.AI.OpenAI;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Responses;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;


IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string endpoint = config["endpoint"]!;
string apikey = config["apikey"]!;
string openaiapikey = config["openaikey"]!;

AzureOpenAIClient client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apikey), new AzureOpenAIClientOptions
{
    NetworkTimeout = TimeSpan.FromSeconds(60)
});

ChatClientAgent agent = client
        .GetChatClient("gpt-5-mini")
        .AsAIAgent();

string myimage = "<enter the base64 of your image here>";  // You can use DevToys or any online tool to convert an image to base64

AgentResponse imageresponse = await agent.RunAsync(new ChatMessage(ChatRole.User,
[
    new TextContent("Can you analyze the image I am sending you and describe what you see in detail?"),
    new DataContent(myimage, "image/jpeg")

    //new UriContent(new Uri("https://example.com/image.jpg"), "image/jpeg") //You can also send the image as a Uri if it is hosted somewhere, just make sure the model has access to that location (e.g. public url or accessible with the credentials you provide)

]));
Console.WriteLine(imageresponse);

if (imageresponse.Usage != null)
{
    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine();
    Console.WriteLine($"Input Tokens: {imageresponse.Usage.InputTokenCount}");
    Console.WriteLine($"Cached Tokens: {imageresponse.Usage.CachedInputTokenCount ?? 0}");
    Console.WriteLine($"Output Tokens: {imageresponse.Usage.OutputTokenCount}");
    Console.WriteLine($"Reasoning Tokens: {imageresponse.Usage.ReasoningTokenCount ?? 0}");
    Console.WriteLine($"Total Tokens: {imageresponse.Usage.TotalTokenCount}");
    Console.ResetColor();
}
