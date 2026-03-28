# 🛒 Microservices E-Commerce System (.NET)

This project is a backend-focused microservice architecture developed with ASP.NET Core to explore distributed system patterns and transaction management strategies.

It demonstrates how multiple services communicate, coordinate, and maintain data consistency in a distributed environment.

---

## 🚀 Architecture Overview

The system is built using a **microservice architecture**, where each service is responsible for a specific business domain.

### Services

- **Order.API** → Handles order creation and management  
- **Stock.API** → Manages product inventory and reservations  
- **Payment.API** → Simulates payment processing  
- **Coordinator.Service** → Central coordinator for 2PC transactions  
- **Shared** → Contains message contracts (events & commands)

---

## ⚙️ Communication

The system supports both **asynchronous** and **synchronous** communication:

- **Asynchronous Messaging** → Event-driven communication between services  
- **Synchronous Calls** → Used in 2PC coordination flow  

---

## 🧠 Implemented Patterns

### 1. Event-Driven Architecture (Choreography)

- Services communicate via events  
- No central coordinator  
- Ensures **eventual consistency**

#### Flow Example:
1. Order created → `OrderCreatedEvent`
2. Stock reserved → `StockReservedEvent`
3. Payment completed → `PaymentCompletedEvent`

---

### 2. Two-Phase Commit (2PC)

A centralized approach for **strong consistency**

#### Flow:
- Phase 1 → Prepare (Vote)
- Phase 2 → Commit / Abort

✔ Guarantees atomic transactions  
✔ All services succeed or fail together  

---

### 3. Data Consistency

- Eventual consistency via messaging  
- Strong consistency via 2PC  
- Trade-offs between reliability and performance explored  

---

## 🛠️ Technologies

- .NET (ASP.NET Core)
- Microservices Architecture
- Messaging / Event-driven design
- REST APIs
- Docker (optional)

---

## 🧪 What I Focused On

- Understanding distributed transaction patterns  
- Comparing **eventual consistency vs strong consistency**  
- Designing reliable service communication  
- Exploring real-world backend architecture decisions  

---

## 📦 Project Structure

/Order.API
/Stock.API
/Payment.API
/Coordinator.Service
/Shared
