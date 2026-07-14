# RabbitMQ Modified Sender — DocReader Demands

A local testing utility for microservice message queues. Contains a **Sender** (publishes a test message to any RabbitMQ queue) and a **Consumer** (listens on a queue and processes incoming messages). Useful for end-to-end testing of message-driven modules without needing the full system running.

---

## How It Works

1. The **Consumer** subscribes to a named RabbitMQ queue and waits for messages.
2. The **Sender** publishes a test message to that same queue.
3. Both components log activity to the console and to an **Elasticsearch** sink via Serilog.

---

## Tech Stack

| Component | Technology |
|-----------|------------|
| Sender    | C# (.NET — modern SDK-style project) |
| Consumer  | C# (.NET Framework — classic project) |
| Messaging | [MassTransit](https://masstransit.io/) over RabbitMQ |
| Logging   | Serilog + Elasticsearch sink |

---

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (for Sender)
- [.NET Framework 4.x](https://dotnet.microsoft.com/download/dotnet-framework) (for Consumer)
- [RabbitMQ](https://www.rabbitmq.com/download.html) running locally on `localhost:5672`
  - Default credentials: `guest` / `guest`
- *(Optional)* [Elasticsearch](https://www.elastic.co/downloads/elasticsearch) on `localhost:9200` for log shipping

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/vincent-capistrano/RabbitMq-Modified-Sender-DocReader-Demands.git
cd RabbitMq-Modified-Sender-DocReader-Demands
```

### 2. Open the solution

Open `BackendModule-LocalSender.sln` in Visual Studio.

### 3. Configure the queue name

Both projects use a `queueName` variable that must match the queue the consumer you're testing is subscribed to.

**Sender** (`Sender/Program.cs`):
```csharp
string queueName = "your-queue-name";
```

**Consumer** (`Consumer/Program.cs`):
```csharp
string queueName = "your-queue-name";
```

> **Important:** Queue names are case-sensitive. They must match exactly between Sender and Consumer.

---

## Running

### Start the Consumer first

Run the `Consumer` project. It will print:
```
[Consumer]: Listening for messages. Press Enter to exit.
```

### Then run the Sender

Run the `Sender` project to publish a test message to the queue. Check the Consumer console for the received message.

> **Tip:** If messages are not flowing, reset the queue state via the RabbitMQ Management UI at `http://localhost:15672`.

---

## Project Structure

```
RabbitMq-Modified-Sender-DocReader-Demands/
├── Sender/
│   ├── Program.cs        # Publishes a test message to the queue
│   └── Sender.csproj
├── Consumer/
│   ├── Program.cs        # Listens on the queue and processes messages
│   └── Consumer.csproj
└── BackendModule-LocalSender.sln
```

---

## Author

[@vincent-capistrano](https://github.com/vincent-capistrano)

