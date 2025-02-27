# MassTransit Delayed Redelivery Test

This project demonstrates the use of MassTransit with RabbitMQ for delayed redelivery and message retry mechanisms. It includes a .NET Worker Service that publishes and consumes messages with retry policies using Polly and MassTransit.

## Prerequisites

- .NET 8 SDK
- Docker
- RabbitMQ
- Seq (for logging)

## Getting Started

### Clone the Repository


### Build and Run with Docker Compose

1. **Build and run the Docker containers:**

docker-compose up --build



    This command will build the Docker images and start the containers for the application, RabbitMQ, and Seq.

### Configuration

- **RabbitMQ:** The application expects RabbitMQ to be running and accessible at `rabbitmq` with default credentials (`guest`/`guest`).
- **Seq:** The application logs to Seq at `http://seq:5341`.

### Environment Variables

- `DB_ACTIVE`: Set this environment variable to `true` or `false` to simulate the database being active or inactive.

### Project Structure

- **MassTransitDelayedRedeliveryTest.csproj:** Project file with dependencies.
- **Program.cs:** Entry point of the application, configures MassTransit and services.
- **TestMessage.cs:** Defines the message contract.
- **TestMessagePublisherService.cs:** Publishes messages to RabbitMQ.
- **TestMessageConsumer.cs:** Consumes messages from RabbitMQ with retry policies.
- **Dockerfile:** Dockerfile to build and run the application.
- **docker-compose.yml:** Docker Compose file to orchestrate the application, RabbitMQ, and Seq.

### Usage

1. **Publish a message:**

    The `TestMessagePublisherService` publishes a message to RabbitMQ when the service starts.

2. **Consume a message:**

    The `TestMessageConsumer` consumes the message from RabbitMQ. It uses Polly for retry policies and MassTransit for delayed redelivery.

### Logging

The application uses Serilog for logging. Logs are written to the console and Seq.

### Customization

- **Retry Policies:**

    You can customize the retry policies in `TestMessageConsumer.cs` by modifying the Polly and MassTransit configurations.

- **Message Handling:**

    Modify the `Consume` method in `TestMessageConsumer.cs` to change how messages are processed.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## License

This project is licensed under the MIT License.
 