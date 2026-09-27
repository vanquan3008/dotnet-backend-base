# .NET Backend Foundation

A reusable, production-oriented backend foundation built with **.NET 10**, **Domain-Driven Design (DDD)**, **CQRS**, and modular infrastructure adapters.

The project provides a consistent starting point for building REST APIs, internal services, and distributed backend systems without forcing every application to enable every infrastructure component.

The built-in `User` module is intentionally small and serves as the reference implementation for domain modeling, application use cases, persistence, messaging, validation, testing, and API exposure.

---

## Overview

The foundation is designed around four primary layers:

```text
┌──────────────────────────────────────────────────────────────┐
│                          API                                 │
│                                                              │
│ Controllers / Endpoints                                      │
│ Authentication                                               │
│ Middleware                                                   │
│ Problem Details                                              │
│ OpenAPI                                                      │
│ Health Checks                                                │
└──────────────────────────────┬───────────────────────────────┘
                               │
                               ▼
┌──────────────────────────────────────────────────────────────┐
│                      Application                             │
│                                                              │
│ Commands                                                     │
│ Queries                                                      │
│ Handlers                                                     │
│ Validators                                                   │
│ Pipeline Behaviors                                           │
│ Infrastructure Abstractions                                  │
│ Application Services                                         │
└──────────────────────────────┬───────────────────────────────┘
                               │
                               ▼
┌──────────────────────────────────────────────────────────────┐
│                         Domain                               │
│                                                              │
│ Aggregates                                                   │
│ Entities                                                     │
│ Value Objects                                                │
│ Domain Events                                                │
│ Domain Exceptions                                            │
│ Business Rules                                               │
└──────────────────────────────────────────────────────────────┘

                 ▲
                 │ implements Application ports
                 │
┌──────────────────────────────────────────────────────────────┐
│                    Infrastructure                            │
│                                                              │
│ PostgreSQL / EF Core                                         │
│ MongoDB                                                      │
│ Redis                                                        │
│ RabbitMQ / MassTransit                                       │
│ Brevo                                                        │
│ Local / S3 Storage                                           │
│ JWT Identity                                                 │
│ Hangfire                                                     │
│ External HTTP Clients                                        │
│ OpenTelemetry                                                │
└──────────────────────────────────────────────────────────────┘