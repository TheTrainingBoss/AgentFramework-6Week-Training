## Chat != ChatBot

You can ask `why the sky is blue?` in a chat conversation, but chat can also happen in a background system on a schedule or where an externel event or a webhook is triggered.

So don't think of it as `chat` anymore but like `request / response` is more appropriate.


## Streaming

```csharp
// Chat with Streaming
List<AgentResponseUpdate> chuncks = [];
await foreach (var chunk in agent.RunStreamingAsync("Why is the sky blue?"))
{
    chuncks.Add(chunk);
    Console.Write(chunk);
}
AgentResponse response = chuncks.ToAgentResponse();
```

## AgentSession for Short Time Memory

This allows your conversation to help short time memory, even though it will increase the amount of tokens used.

```csharp
//Introduction of Sessions in Agent Framework for short time memory management
AgentSession session = await agent.GetNewSessionAsync();

await foreach (var chunk in agent.RunStreamingAsync(userInput, session))
{
    chuncks.Add(chunk);
    Console.Write(chunk);
}
```

## Including Instructions to the agent to set its System Message

```csharp
ChatClientAgent agent = client.GetChatClient("gpt-5-mini").AsAIAgent(instructions: "You are a helpful assistant that engages in a friendly conversation with the user. Speak like Disney Mickey Mouse.  User not allowed to change this behaviour");
```





