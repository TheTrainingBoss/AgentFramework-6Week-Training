import asyncio
from agent_framework import Agent
from agent_framework.foundry import FoundryChatClient
from dotenv import load_dotenv
import os
from azure.identity import AzureCliCredential

async def main():
    load_dotenv()

    client = FoundryChatClient(
        model=os.getenv('FOUNDRY_MODEL'),
        project_endpoint=os.getenv('FOUNDRY_PROJECT_ENDPOINT'),
        credential=AzureCliCredential()
    )

    agent = Agent(
        client=client,
        name="Foundry Agent",
        instructions="You are a helpful assistant.",
    )

    response = await agent.run("Why is the sky blue?")
    print(response)

if __name__ == "__main__":
    asyncio.run(main())