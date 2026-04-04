from dotenv import load_dotenv
from agent_framework.openai import OpenAIChatClient
from agent_framework import Agent
from agent_framework._types import AgentResponse
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
        name="MySimpleAgent",
        instructions="Speak like a Pirate",
        default_options={
            "max_tokens": 1000,
            "reasoning": {"effort": "low"},
        }
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

if __name__ == "__main__":
    asyncio.run(main())