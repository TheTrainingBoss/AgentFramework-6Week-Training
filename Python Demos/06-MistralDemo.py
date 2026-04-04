from dotenv import load_dotenv
from agent_framework.openai import OpenAIChatCompletionClient
from agent_framework import Agent
import asyncio
import os

async def main():
    load_dotenv()

    client = OpenAIChatCompletionClient(
        api_key=os.getenv('MISTRAL_API_KEY'),
        model=os.getenv('MISTRAL_MODEL'),
        base_url="https://api.mistral.ai/v1",
    )

    agent = Agent(
        client=client,
        name="Mistral Agent",
        instructions="You are a helpful assistant.",
    )

    response = await agent.run("Why is the sky blue?")
    print(response)

if __name__ == "__main__":
    asyncio.run(main())