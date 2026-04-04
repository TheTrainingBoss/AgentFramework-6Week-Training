import asyncio
from agent_framework import Agent
from agent_framework.ollama import OllamaChatClient
from dotenv import load_dotenv
import os
from azure.identity import AzureCliCredential

async def main():
    load_dotenv()

    client = OllamaChatClient(
        model="gemma3:4b",
        host="http://localhost:11434"
    )

    agent = Agent(
        client=client,
        name="Ollama Agent",
        instructions="You are a helpful assistant.",
    )

    response = await agent.run("Why is the sky blue?")
    print(response)

if __name__ == "__main__":
    asyncio.run(main())