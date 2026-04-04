### This is the KQL query to run in App Insights to view the openTelemetry info including the sensitive info

```code
dependencies
| extend cd = parse_json(customDimensions)
|project 
    timestamp, 
    id, 
    duration, 
    InTokens = cd["gen_ai.usage.input_tokens"],
    OutTokens = cd["gen_ai.usage.output_tokens"],
    AgentName = cd["gen_ai.agent.name"],
    AIModel = cd["gen_ai.response.model"],
    InputData = cd["gen_ai.input.messages"],
    OutputData = cd["gen_ai.output.messages"]
```