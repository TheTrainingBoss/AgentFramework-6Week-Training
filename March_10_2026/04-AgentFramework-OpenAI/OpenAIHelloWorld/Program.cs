using Microsoft.Agents.AI;
using OpenAI;
using OpenAI.Chat;

string apikey = "Please enter your OpenAI API key here";  //You can get a new OpenAI API KEY from https://platform.openai.com/api-keys

OpenAIClient client = new OpenAIClient(apikey);

ChatClientAgent agent = client.GetChatClient("gpt-5-mini").AsAIAgent();

AgentResponse response = await agent.RunAsync("Why is the sky blue?");

Console.WriteLine(response);
