from dotenv import load_dotenv
from agent_framework.openai import OpenAIChatCompletionClient
from agent_framework import Agent
import asyncio
import os

async def main():
    load_dotenv()

    client = OpenAIChatCompletionClient(
        api_key=os.getenv('GOOGLE_API_KEY'),
        model=os.getenv('GOOGLE_MODEL'),
        base_url="https://generativelanguage.googleapis.com/v1beta/openai/",
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