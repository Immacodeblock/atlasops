# Troubleshooting

## Queue messages go to `workitem-checks-poison`
Meaning:
- A queue message failed processing 5 times (default MaxDequeueCount) and was moved to poison.

### Common causes
1) Azurite not running
   - Error resembles connection refused to 127.0.0.1:10001

2) Message decoding failed
   - Host log: "Message decoding has failed! Check MessageEncoding settings."
   - Fix: ensure QueueClient uses Base64 encoding (see ADR-0004).

3) Payload deserialization failure
   - Example: JSON is `{"workItemId":10}` but C# expects `WorkItemId` with case-sensitive options.
   - Fix: set `PropertyNameCaseInsensitive=true` or use `[JsonPropertyName]`.

### How to debug
- Run:
  ```powershell
  func start --verbose