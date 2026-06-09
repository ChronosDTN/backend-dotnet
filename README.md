# Chronos DTN — API Backend .NET 8

> Gateway Financeiro e Roteador de Rede DTN (Delay-Tolerant Network) para comunicação Terra-Lua na nova Economia Espacial.

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Oracle](https://img.shields.io/badge/Oracle-21c-F80000?logo=oracle)](https://www.oracle.com/database/)
[![Entity Framework](https://img.shields.io/badge/EF_Core-8.0-512BD4)](https://docs.microsoft.com/ef/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

---

## 🚀 **O Problema**

No espaço, a internet não funciona como na Terra. Entre a Terra e a Lua existem:
- **Interrupções constantes** de sinal entre satélites
- **Atraso natural** de 1,28 segundos (velocidade da luz)
- **Dilatação temporal** relativística (tempo passa diferente na Lua)

## 💡 **A Solução: Chronos DTN**

Sistema baseado em **DTN (Delay-Tolerant Network)** que:
- Retém transações financeiras em buffer quando não há sinal
- Envia automaticamente quando a janela de comunicação abre
- Corrige matematicamente diferenças de fuso horário interplanetário
- Garante que dinheiro e dados **não se percam no espaço**

---

## 🏗️ **Arquitetura do Sistema**

```mermaid
graph TB
    subgraph "Client Layer"
        A[Swagger UI]
        B[Mobile App]
        C[ESP32 IoT]
    end
    
    subgraph "API Layer - ASP.NET Core"
        D[NodesController]
    end
    
    subgraph "Application Layer"
        E[INodeService]
        F[NodeService]
    end
    
    subgraph "Domain Layer"
        G[Node Entity]
        H[AssetBalance Entity]
        I[NodeLink Entity]
    end
    
    subgraph "Infrastructure Layer"
        J[ChronosDbContext]
        K[EF Core]
    end
    
    subgraph "Database Layer"
        L[(Oracle XE 21c)]
    end
    
    A --> D
    B --> D
    C --> D
    D --> E
    E --> F
    F --> G
    F --> H
    F --> I
    G --> J
    H --> J
    I --> J
    J --> K
    K --> L