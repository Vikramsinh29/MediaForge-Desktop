# MediaForge Architecture

Version: 1.0
Status: Active

---

# Solution Overview

MediaForge is a Windows desktop application built using WPF and MVVM. The architecture is designed so that business logic can later be shared with Web, API and CLI clients.

Current Solution:

```
MediaForge.slnx
│
├── src
│   ├── MediaForge
│   ├── MediaForge.Core
│   └── MediaForge.Infrastructure
│
└── tests
```

---

# Project Responsibilities

## MediaForge

Presentation Layer

Responsibilities

- WPF UI
- MVVM
- ViewModels
- Commands
- Dependency Injection
- User Interaction

Must NOT contain

- FFmpeg logic
- Business logic
- Encoding logic

---

## MediaForge.Core

Domain Layer

Responsibilities

- Models
- Interfaces
- Contracts
- Shared types

Must NOT reference

- WPF
- FFmpeg
- Infrastructure

---

## MediaForge.Infrastructure

Infrastructure Layer

Responsibilities

- FFmpeg
- FFprobe
- Thumbnail generation
- Conversion engine
- External processes

Implements interfaces defined in Core.

---

# Dependency Rules

MediaForge
↓

MediaForge.Core

MediaForge
↓

MediaForge.Infrastructure

MediaForge.Infrastructure
↓

MediaForge.Core

Core never references other projects.

---

# Current Conversion Flow

User

↓

Open Files

↓

FFprobe

↓

Queue

↓

Select Output Format

↓

Conversion Service

↓

FFmpeg

↓

Output File

---

# Future Architecture

Desktop

↓

Application Layer

↓

Core

↓

Infrastructure

↓

External Engines

- FFmpeg
- ImageMagick
- Whisper
- OCR
- AI Providers

---

# Architectural Principles

- Keep UI independent of conversion engine.
- Business logic must be reusable.
- Infrastructure implements Core interfaces.
- Async operations for long-running work.
- No duplicate conversion logic.
- One responsibility per class.

---

Status

Stable

Next Review

Desktop v1.0