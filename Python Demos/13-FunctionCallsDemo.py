from datetime import datetime
from dotenv import load_dotenv
from agent_framework.openai import OpenAIChatClient
from agent_framework import Agent, tool
from agent_framework._types import AgentResponse
from typing import Annotated
from pydantic import Field
from random import randint
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
        name="My Weather Agent",
        instructions="You are a knowledgeable assistant that provides weather information. Always include the current date and time in your response.",
        tools=[get_weather, get_date_time]
    )

    while True:
        user_input = input("\nYou: ").strip()
        if user_input.lower() in {"exit", "quit"}:
            break

        print("Assistant: ", end="", flush=True)

        async def stream_and_print():
            async for chunk in agent.run(user_input, stream=True):
                if chunk.text:
                    print(chunk.text, end="", flush=True)
                yield chunk

        response = await AgentResponse.from_update_generator(stream_and_print())

        print()

        usage = response.usage_details
        if usage:
            print("\n--- Token Usage ---")
            print(f"Input Tokens: {usage.get('input_token_count', 'N/A')}")
            print(f"Output Tokens: {usage.get('output_token_count', 'N/A')}")
            print(f"Reasoning Tokens: {usage.get('openai.reasoning_tokens', 'N/A')}")
            print(f"Total Tokens: {usage.get('total_token_count', 'N/A')}")

@tool(approval_mode="never_require")
def get_weather(
    location: Annotated[str, Field(description="The location to get the weather for.")],
) -> str:
    """Get the weather for a given location."""
    conditions = ["sunny", "cloudy", "rainy", "stormy"]
    print(f"Getting weather for {location} from the tool...")
    return f"The weather in {location} is {conditions[randint(0, 3)]} with a high of {randint(60, 100)}°F."

@tool(approval_mode="never_require")
def get_date_time() -> str:
    """Get the current date and time."""
    print("Getting current date and time from the tool...")
    return f"The current date and time is {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}."

if __name__ == "__main__":
    asyncio.run(main())