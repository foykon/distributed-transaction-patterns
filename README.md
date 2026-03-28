# 🔁 Distributed Transaction Patterns (.NET)

This project explores distributed transaction management strategies in microservice architectures using .NET.

It focuses on implementing and comparing **Saga (Orchestration & Choreography)** and **Two-Phase Commit (2PC)** patterns to understand consistency trade-offs in distributed systems.

---

## 🎯 Purpose

The goal of this project is to gain hands-on experience with:

- Distributed transaction management  
- Event-driven architecture  
- Strong vs eventual consistency trade-offs  
- Service-to-service communication patterns  

---

## 🧠 Implemented Patterns

### 1. Saga Pattern

#### 🔹 Choreography-based Saga
- Services communicate via events  
- No central coordinator  
- Each service reacts to events and triggers the next step  

✔ Loosely coupled  
✔ Eventually consistent  

---

#### 🔹 Orchestration-based Saga
- A central orchestrator manages the transaction flow  
- Controls which service executes next  

✔ Better control  
✔ Easier error handling  

---

### 2. Two-Phase Commit (2PC)

A centralized transaction coordination mechanism that ensures **strong consistency**

#### Flow:
1. **Prepare Phase** → Services vote (Yes / No)  
2. **Commit Phase** → Commit or rollback  

✔ Atomic transactions  
❌ Less scalable  

---

## ⚙️ Architecture

The system is built with multiple services communicating via:

- Asynchronous messaging (event-driven)  
- Synchronous calls for coordination  

Each service manages its own data and reacts based on the chosen pattern.

---

## 🔄 Consistency Models

| Pattern | Consistency | Coupling | Complexity |
|--------|------------|----------|------------|
| Saga (Choreography) | Eventual | Low | Medium |
| Saga (Orchestration) | Eventual | Medium | Medium |
| 2PC | Strong | High | High |

---

## 🛠️ Technologies

- .NET (ASP.NET Core)  
- Microservices Architecture  
- REST APIs  
- Messaging concepts  
- Docker (optional)  

---

## 🧪 Key Learnings

- Trade-offs between consistency, scalability, and complexity  
- Designing reliable distributed workflows  
- Handling failure scenarios and rollback strategies  
- Understanding real-world system design decisions  

---

## 📦 Project Structure/Order.API

/Services
/Orchestrator
/Shared
