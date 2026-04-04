import asyncio
from agent_framework import Agent
from agent_framework_foundry_local import FoundryLocalClient
from dotenv import load_dotenv
import os

async def main():
    load_dotenv()
    
    client = FoundryLocalClient(
       model="phi-4-mini"
    )

    agent = Agent(
        client=client,
        name="Foundry Local Agent",
        instructions="You are a helpful assistant.",
    )

    response = await agent.run("Why is the sky blue?")
    print(response)

if __name__ == "__main__":
    asyncio.run(main())