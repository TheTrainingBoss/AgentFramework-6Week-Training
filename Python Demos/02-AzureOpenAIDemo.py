from dotenv import load_dotenv
from agent_framework.openai import OpenAIChatClient
from agent_framework import Agent
import asyncio
import os

async def main():
    load_dotenv()

    client = OpenAIChatClient(
        api_key=os.getenv('AZURE_OPENAI_API_KEY'),
        model=os.getenv('AZURE_OPENAI_MODEL'),
        azure_endpoint=os.getenv('AZURE_OPENAI_ENDPOINT'),
    )

    agent = Agent(
        client=client,
        name="MyAgent",
        instructions="You are a helpful assistant.",
    )
    response = await agent.run("Why is the sky blue?")
    print(response)

if __name__ == "__main__":
    asyncio.run(main())