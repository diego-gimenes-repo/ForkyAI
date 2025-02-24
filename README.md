# ForkyAI: A Scalable AI-Powered Microservices Framework with Semantic Kernel & Orleans

## Overview

ForkyAI provides a **scalable and modular architecture** integrating **Semantic Kernel**, **Orleans .NET**, **RabbitMQ (streaming)**, and **Qdrant (vector storage)** to build an AI-driven microservices ecosystem. Designed to support both **C# and Python**, it enables flexible AI workflows, real-time event processing, and distributed state management.

## Architectural Principles

This project follows **Clean Architecture**, **SOLID Principles**, **Clean Code**, and **CQRS (Command Query Responsibility Segregation)** to ensure maintainability, scalability, and separation of concerns.

## Motivation

When migrating a **20+ year-old** legacy system, we chose **gRPC** to connect different system contexts. At that moment, I strongly considered implementing **Orleans**, not because **Kubernetes** couldn't handle scaling, but because Orleans would allow the system to be **natively scalable** by design. However, in a project of this magnitude, innovating without time and budget in hand is risky—and sometimes, you simply **can’t afford** to reinvent the wheel.

Once that project was completed, a new challenge arose: **structuring the BI team and implementing AI** in my previous company. That’s when I encountered **Langchain**. Again, my instinct was to implement it using **Semantic Kernel**. Not because it was **fully mature** (since AI development moves at an insane speed), but at least I knew that **Microsoft** handles **breaking changes** well. Langchain’s stack, on the other hand, gave me **a massive headache**.

So, this project is **my attempt to bring both worlds together**: the **scalability and reactivity** of **Orleans**, the **proven best practices of traditional software development**, and a **simple yet performant and scalable** approach that avoids unnecessary complexity. 🚀

## Features

- **Scalable Actor Model** with Orleans .NET for distributed processing.
- **AI Automation** using Semantic Kernel for intelligent task execution.
- **Event-Driven Architecture** powered by RabbitMQ for real-time streaming.
- **Cloud-Native Storage** using Qdrant for high-performance vector search.
- **Cross-Language Support** enabling seamless integration of Python & C#.
- **Flexible Deployment** with Docker & Kubernetes for scalability.
- **CQRS Pattern** for a clear separation between read and write operations.

## System Architecture

### Observability

To enhance monitoring and tracing capabilities, **OpenTelemetry** will be integrated across all services, providing detailed insights into system performance. Logs, traces, and metrics will be collected and sent to **Microsoft Azure Monitor**, allowing real-time visibility, alerting, and analysis of distributed workloads.

### **1. Presentation Layer**

- **API Gateway (YARP)**: Routes client requests to the appropriate microservices.
- **Frontend (React.js)**: UI for creating, managing grains, and visualizing data.

### **2. Application Layer**

- **Command Handlers**: Implements business logic for writing operations (CQRS).
- **Query Handlers**: Handles data retrieval separately from command execution.
- **Use Cases**: Encapsulates application-specific business logic.

### **3. Domain Layer**

- **Entities & Aggregates**: Represents core business models.
- **Domain Services**: Encapsulates complex business logic that doesn’t belong in an entity.
- **Domain Events**: Captures changes within aggregates.

### **4. Infrastructure Layer**

- **Observability (OpenTelemetry & Azure Monitor)**: Ensures real-time monitoring, tracing, and performance analysis across all microservices.
- **Orleans .NET Cluster**: Manages distributed actor-based workflows.
- **RabbitMQ (Message Broker)**: Handles real-time event-driven messaging, providing reliable queuing and message distribution for microservices.
- **Workers (C# & Python)**: Consume RabbitMQ messages and process AI tasks.
- **Semantic Kernel**: Enables AI-powered automation and inference. A memory layer is integrated to enhance AI capabilities and improve contextual responses.
- **Qdrant (Vector Database)**: Handles vector storage for AI-related indexing and similarity search.
- **MongoDB (NoSQL Database)**: Stores metadata, grain state, and structured data, complementing Qdrant for AI processing and enabling grain visualization.

## Folder Structure

```plaintext
/Project
│── ApiGateway/            # API Gateway (YARP)
│──  OrleansCluster        # Orleans .NET actors and state management
│── Application/           # CQRS commands, queries, and use cases
│── Domain/                # Entities, Aggregates, Domain Services, and Events
│── Infrastructure/        # Data Persistence, RabbitMQ, Qdrant, MongoDB, and Orleans
│── Workers/
│   │── CSharpWorker/      # Worker service in C#
│   │── PythonWorker/      # Worker service in Python
│── SemanticKernel/        # AI processing and automation
│── RabbitMQ/              # Streaming configuration
│── Qdrant/                # Vector storage for AI indexing and retrieval
│── MongoDB/               # NoSQL database for grain state and metadata
│── Frontend/
│   │── ReactApp/          # React.js UI for grain visualization and management
│   │── Components/        # UI components
│   │── Services/          # API communication and state management
│   │── Pages/             # Application views and routing
│   │── Assets/            # Static assets (images, styles, etc.)
│── Database/              # Database schema & migrations
│── Deploy/                # Docker & Kubernetes configurations
│── Scripts/               # Utility scripts for automation and setup
│── docker-compose.yml     # Orchestration for local testing
│── README.md              # Documentation
```

## Setup & Installation

### Prerequisites

- .NET 7+
- Python 3.9+
- Docker & Docker Compose
- Kubernetes (Optional for production)

### Running Locally

1. Clone the repository:
   ```sh
   git clone https://github.com/your-repo/project.git
   cd project
   ```
2. Start all services using Docker Compose:
   ```sh
   docker-compose up -d
   ```
3. Access API Gateway at `http://localhost:5000`

## Deployment with Kubernetes

For production environments, deploy using Kubernetes:

```sh
docker build -t your-api-gateway ./ApiGateway
kubectl apply -f ./Deploy/k8s-config.yaml
```

## Contributing

Feel free to submit issues and pull requests to improve the project. Contributions are always welcome! 🚀

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

