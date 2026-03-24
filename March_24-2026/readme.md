## Using Tools with Microsoft Agent framework

## AIFunctionFactory
The ability to enable functions to be consumed as Function Calls available to the LLM to choose from upon making a request.

## MCP Servers

[Microsoft Learn MCP](https://learn.microsoft.com/en-us/training/support/mcp)

[GitHub](https://github.com/github/github-mcp-server)

RunAsync the following question with and without the MCP Server of Microsoft Learn and prepare to laugh to the answer without the MCP Server, Mega Halucination 😄
`Create a simple sample in C# on how to create an agent in Agent Framework`

## Middleware

The right way to include tools for enterprise grade tooling and diagnostics

```csharp
static async ValueTask<object?> Middleware(AIAgent agent, FunctionInvocationContext context,
        Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next, CancellationToken cancellationToken)
{
    StringBuilder toolDetails = new();
    toolDetails.Append($"- Tool Call: '{context.Function.Name}'");
    if (context.Arguments.Count > 0)
    {
        toolDetails.Append($" (Args: {string.Join(",", context.Arguments.Select(x => $"[{x.Key} = {x.Value}]"))}");
    }

    Console.WriteLine(toolDetails.ToString());
    return await next.Invoke(context, cancellationToken);
}
```

```csharp
client
.GetChatClient("gpt-4.1")
.AsAIAgent(
.AsBuilder()
.Use(Middleware)  //This is where we include the Middleware to be associated with the agent.
.Build();
```
