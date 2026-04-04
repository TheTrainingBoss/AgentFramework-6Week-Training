from dotenv import load_dotenv
from agent_framework.anthropic import AnthropicClient
from agent_framework import Agent
import asyncio
import os

async def main():
    load_dotenv()

    client = AnthropicClient(
        api_key=os.getenv('ANTHROPIC_API_KEY'),
        model=os.getenv('ANTHROPIC_MODEL'),
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