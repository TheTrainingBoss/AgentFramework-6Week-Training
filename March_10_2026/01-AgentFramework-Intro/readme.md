## Agent Framework Introduction

In this first introduction you will learn how to use the Microsoft Agent Framework to chat with an LLM using the following Large Language Models:
- OpenAI
- Azure OpenAI
- Anthropic (Claude)
- Foundry AI (gpt-4.1)
- Google (gemini-3-flash-preview)
- Grok (grok-4-fast-non-reasoning")
- Mistral (mistral-small-2506)

We will demonstrate the use of local models in March 17th session
- Ollama (gemma3:4b)
- FoundryLocal (Phi-4-mini)

You will also learn how to count the tokens:
- Input Tokens
- Output tokens
- Cached Tokens
- Reasoning Tokens
- Total tokens

Token pricing gets confusing fast, and `cached tokens` sounds like marketing fluff until you know what’s actually happening under the hood. Let’s demystify it. 😄

First: What’s a “token” anyway?
In LLM land, tokens are chunks of text (not exactly words).

Roughly:
- "hello" → 1 token
- "ChatGPT is awesome!" → ~4–5 tokens
- Long prompts + long responses = lots of tokens

You’re billed based on:
- Input tokens (what you send the model)
- Output tokens (what the model sends back)

## Cached Tokens
Cached tokens are input tokens that the model provider has already processed before and can reuse.
In practice, this usually applies to:
- Large system prompts
- Long instructions
- Repeated context (like your product docs, policies, or chatbot instructions)
- Reused embeddings / search context in AI Search + LLM patterns

Instead of reprocessing the same text over and over, the provider:

**Stores a computed representation of those tokens and reuses it.**

That saves:

⏱️ Latency (faster responses)

💰 Cost (cheaper than fresh input tokens)

⚡ Compute (less GPU work)


Finally, you will also learn how to calculate the time it takes to run your query in milliseconds.

## How Tokens are counted
![Tokens Counter](../media/Token_Counter.jpg)

## dotnet user-secrets
- To initialize your user secrets in your project:

```cli
dotnet user-secrets init
```

- To list all the current secrets in the project:

```cli
dotnet user-secrets list
```

- To add a secret to the project:

```cli
dotnet user-secrets set "secret Name" "value"
```

- To remove a secret from the project:

```cli
dotnet user-secrets remove "Secret Name"
```