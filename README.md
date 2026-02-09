# MassTransit Delayed Redelivery Test

This project demonstrates the use of **MassTransit with RabbitMQ** in a **.NET 8 Worker Service** to explore:

- Short, immediate retries handled inside the consumer (Polly-style retries)
- Longer back-off retries using **MassTransit delayed redelivery**
- How these two mechanisms interact in practice
- How to observe and verify retry vs redelivery behaviour using **Seq** and **RabbitMQ**

This repo is intended as a **learning / repro project**, not a production template.

---

## Prerequisites

- .NET 8 SDK (only required if running outside Docker)
- Docker & Docker Compose
- RabbitMQ (provided via Docker)
- Seq (provided via Docker)

---

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/johnmchale/MassTransitDelayedRedeliveryTest.git
cd MassTransitDelayedRedeliveryTest
```

---

### Build and Run with Docker Compose

```bash
docker-compose up --build
```

This will start:

- The .NET 8 worker service
- RabbitMQ (with management UI)
- Seq (for structured logging)

---

## Service Endpoints (local)

### RabbitMQ
- **Management UI:** http://localhost:15672  
  Username / password: `guest` / `guest`

### Seq
- **Web UI (browser):** http://localhost:5342  
- **Ingestion endpoint (used by the app):** http://localhost:5341  

> Inside Docker, the worker service sends logs to `http://seq:5341`.

---

## Configuration

### RabbitMQ

The worker service connects to RabbitMQ using:

- Host: `rabbitmq`
- Username: `guest`
- Password: `guest`

These values are configured for Docker networking and **do not need changing** for local testing.

---

### Seq Logging

The application logs using **Serilog**, writing to:

- Console
- Seq ingestion API at `http://seq:5341` (inside Docker)

The Seq **web UI** is exposed on `http://localhost:5342`.

Seq is the easiest way to verify **retry timing and behaviour**.

---

## Environment Variables

### `DB_ACTIVE`

This variable is used to **simulate a downstream dependency** (e.g. a database):

- `DB_ACTIVE=true`  
  ? message processing succeeds

- `DB_ACTIVE=false`  
  ? consumer throws an exception, triggering retries / redelivery

---

## Project Structure

- **MassTransitDelayedRedeliveryTest.csproj**  
  Project file and NuGet dependencies

- **Program.cs**  
  Configures the Host, MassTransit, RabbitMQ, logging, and the consumer endpoint

- **TestMessage.cs**  
  Message contract

- **TestMessagePublisherService.cs**  
  Publishes a test message when the worker service starts

- **TestMessageConsumer.cs**  
  Consumes messages and deliberately fails when `DB_ACTIVE=false`  
  This is where retry and delayed redelivery behaviour is exercised

- **Dockerfile**  
  Builds and runs the worker service

- **docker-compose.yml**  
  Orchestrates the worker service, RabbitMQ, and Seq

---

## How the Demo Works

### Message Publishing

When the worker service starts, `TestMessagePublisherService` publishes a single test message to RabbitMQ.

No manual publishing is required.

---

### Message Consumption & Retry Behaviour

The consumer is designed to demonstrate **two distinct retry layers**:

1. **Short retries (inside the consumer)**  
   These represent quick retries for transient failures (e.g. a brief DB blip).

2. **Delayed redelivery (MassTransit)**  
   If failures persist, MassTransit schedules the message for redelivery after a longer delay.

Conceptually:

> **Polly handles “try again quickly”**  
> **MassTransit handles “come back later”**

---

## Verifying Retry vs Delayed Redelivery

### Using Seq (recommended)

1. Open Seq at: http://localhost:5342
2. Filter logs by:
   - Consumer name
   - Message ID / Correlation ID
3. Observe timestamps between attempts:
   - Short retries appear close together
   - Delayed redelivery attempts appear later

### Using RabbitMQ Management UI

You can also inspect queues and exchanges to confirm messages are not being immediately reprocessed after delayed redelivery is scheduled.

---

## Customisation

You can experiment by changing:

- Retry timing and counts in `TestMessageConsumer.cs`
- Delayed redelivery intervals in the MassTransit configuration
- Logging detail to make timing more obvious in Seq

This repo is intentionally small so these changes are easy to reason about.

---

## Why This Repo Exists

Delayed redelivery can sometimes **appear** to behave like immediate retry if misconfigured or misunderstood.

This project exists to:

- Make retry vs redelivery behaviour visible
- Provide a minimal repro when investigating timing issues
- Act as a reference when reasoning about Polly vs MassTransit responsibilities

---

## Contributing

If you improve clarity (logging, comments, documentation, timing visibility), PRs are welcome.

---

## License

MIT
