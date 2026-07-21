# MediaForge Desktop - Architecture Guide

**Document Version:** 1.0

**Project:** MediaForge Desktop

**Framework:** .NET 10 WPF

**Architecture:** MVVM (Model–View–ViewModel)

**Status:** Production Foundation

---

# 1. Purpose

This document describes the internal architecture of MediaForge Desktop.

It is intended for:

- Developers joining the project
- Future AI assistants continuing development
- Contributors
- Code reviewers
- Architects

The goal is to explain **how the application is designed**, not simply how it works.

---

# 2. High-Level Architecture

MediaForge is divided into logical layers to maintain clear separation of concerns.

```
+------------------------------------------------------+
|                     User Interface                   |
|                     WPF Views                        |
+--------------------------▲---------------------------+
                           │ Data Binding
                           │
+--------------------------▼---------------------------+
|                    ViewModels (MVVM)                |
| Command Handling • State • UI Coordination          |
+--------------------------▲---------------------------+
                           │
                           │ Interfaces
                           │
+--------------------------▼---------------------------+
|                     Service Layer                   |
| File Dialog • FFprobe • FFmpeg • Queue • Thumbnail |
+--------------------------▲---------------------------+
                           │
                           │
+--------------------------▼---------------------------+
|                      Core Models                    |
| MediaInfo • ConversionRequest • Result • Formats   |
+--------------------------▲---------------------------+
                           │
                           │
+--------------------------▼---------------------------+
|                 External Dependencies               |
| FFmpeg • FFprobe • Windows File System             |
+-----------------------------------------------------+
```

Every layer has a single responsibility.

---

# 3. Design Principles

The project follows these principles throughout the codebase.

### Separation of Concerns

Views display information.

ViewModels manage UI logic.

Services perform business operations.

Models carry data.

Each layer communicates only through defined interfaces.

---

### Single Responsibility Principle

Each class should have one clear responsibility.

Examples:

- `ThumbnailService` generates thumbnails.
- `ConversionService` performs conversions.
- `FFprobeService` extracts metadata.

Avoid "God classes" that perform multiple unrelated tasks.

---

### Dependency Inversion

ViewModels should depend on interfaces rather than concrete implementations.

Example:

```csharp
private readonly IConversionService _conversionService;
```

instead of

```csharp
private readonly ConversionService _conversionService;
```

This keeps the application testable and flexible.

---

### Asynchronous Operations

All long-running operations must be asynchronous.

Examples:

- Metadata extraction
- Thumbnail generation
- Conversion
- Batch processing

Never block the UI thread.

Use `async` / `await` consistently.

---

# 4. Project Structure

```
MediaForge-Desktop
│
├── src
│   ├── MediaForge
│   │   ├── Views
│   │   ├── ViewModels
│   │   ├── Services
│   │   ├── Converters
│   │   ├── Commands
│   │   └── Resources
│   │
│   ├── MediaForge.Core
│   │   ├── Models
│   │   ├── Interfaces
│   │   ├── Enums
│   │   └── Helpers
│   │
│   └── MediaForge.Infrastructure
│       ├── FFmpeg
│       ├── FileSystem
│       └── Utilities
│
├── tests
│
├── tools
│
└── docs
```

Each project has a clear purpose.

Avoid cross-project dependencies that violate architecture boundaries.

---

# 5. MVVM Flow

The application follows the standard MVVM interaction pattern.

```
User Clicks Button
        │
        ▼
RelayCommand Executes
        │
        ▼
ViewModel Method
        │
        ▼
Service Interface
        │
        ▼
Service Implementation
        │
        ▼
FFmpeg / FFprobe
        │
        ▼
Result Returned
        │
        ▼
Observable Properties Updated
        │
        ▼
UI Refreshes Automatically
```

Business logic must never bypass the ViewModel.

---

# 6. View Layer

Responsibilities:

- Display information
- Data binding
- Styling
- Layout
- User interaction

Views should remain "thin."

Do not perform media processing inside code-behind.

---

# 7. ViewModel Layer

ViewModels coordinate the application.

Responsibilities include:

- Managing state
- Executing commands
- Calling services
- Updating observable properties
- Handling validation
- Managing queue state

They should never contain FFmpeg command construction or file-system specific logic.

---

# 8. Service Layer

Services encapsulate all business logic.

Current services include:

- FileDialogService
- FFprobeService
- ThumbnailService
- ConversionService
- BatchConversionService

Services should communicate through interfaces.

Future services can be added without modifying existing ViewModels.

---

# 9. Core Models

Models represent application data only.

Examples:

- MediaInfo
- ConversionRequest
- ConversionResult
- OutputFormat
- ConversionJob

Models should avoid business logic unless it is intrinsic to the data they represent.

---

# 10. Dependency Direction

Dependencies should always flow inward.

```
View
    ↓
ViewModel
    ↓
Interfaces
    ↓
Services
    ↓
Infrastructure
```

Never reference the UI layer from services.

Never reference Views from Core models.

This rule is fundamental to maintaining a clean, testable architecture.
# MediaForge Desktop - Architecture Guide

**Document Version:** 1.0

**Project:** MediaForge Desktop

**Framework:** .NET 10 WPF

**Architecture:** MVVM (Model–View–ViewModel)

**Status:** Production Foundation

---

# 1. Purpose

This document describes the internal architecture of MediaForge Desktop.

It is intended for:

- Developers joining the project
- Future AI assistants continuing development
- Contributors
- Code reviewers
- Architects

The goal is to explain **how the application is designed**, not simply how it works.

---

# 2. High-Level Architecture

MediaForge is divided into logical layers to maintain clear separation of concerns.

```
+------------------------------------------------------+
|                     User Interface                   |
|                     WPF Views                        |
+--------------------------▲---------------------------+
                           │ Data Binding
                           │
+--------------------------▼---------------------------+
|                    ViewModels (MVVM)                |
| Command Handling • State • UI Coordination          |
+--------------------------▲---------------------------+
                           │
                           │ Interfaces
                           │
+--------------------------▼---------------------------+
|                     Service Layer                   |
| File Dialog • FFprobe • FFmpeg • Queue • Thumbnail |
+--------------------------▲---------------------------+
                           │
                           │
+--------------------------▼---------------------------+
|                      Core Models                    |
| MediaInfo • ConversionRequest • Result • Formats   |
+--------------------------▲---------------------------+
                           │
                           │
+--------------------------▼---------------------------+
|                 External Dependencies               |
| FFmpeg • FFprobe • Windows File System             |
+-----------------------------------------------------+
```

Every layer has a single responsibility.

---

# 3. Design Principles

The project follows these principles throughout the codebase.

### Separation of Concerns

Views display information.

ViewModels manage UI logic.

Services perform business operations.

Models carry data.

Each layer communicates only through defined interfaces.

---

### Single Responsibility Principle

Each class should have one clear responsibility.

Examples:

- `ThumbnailService` generates thumbnails.
- `ConversionService` performs conversions.
- `FFprobeService` extracts metadata.

Avoid "God classes" that perform multiple unrelated tasks.

---

### Dependency Inversion

ViewModels should depend on interfaces rather than concrete implementations.

Example:

```csharp
private readonly IConversionService _conversionService;
```

instead of

```csharp
private readonly ConversionService _conversionService;
```

This keeps the application testable and flexible.

---

### Asynchronous Operations

All long-running operations must be asynchronous.

Examples:

- Metadata extraction
- Thumbnail generation
- Conversion
- Batch processing

Never block the UI thread.

Use `async` / `await` consistently.

---

# 4. Project Structure

```
MediaForge-Desktop
│
├── src
│   ├── MediaForge
│   │   ├── Views
│   │   ├── ViewModels
│   │   ├── Services
│   │   ├── Converters
│   │   ├── Commands
│   │   └── Resources
│   │
│   ├── MediaForge.Core
│   │   ├── Models
│   │   ├── Interfaces
│   │   ├── Enums
│   │   └── Helpers
│   │
│   └── MediaForge.Infrastructure
│       ├── FFmpeg
│       ├── FileSystem
│       └── Utilities
│
├── tests
│
├── tools
│
└── docs
```

Each project has a clear purpose.

Avoid cross-project dependencies that violate architecture boundaries.

---

# 5. MVVM Flow

The application follows the standard MVVM interaction pattern.

```
User Clicks Button
        │
        ▼
RelayCommand Executes
        │
        ▼
ViewModel Method
        │
        ▼
Service Interface
        │
        ▼
Service Implementation
        │
        ▼
FFmpeg / FFprobe
        │
        ▼
Result Returned
        │
        ▼
Observable Properties Updated
        │
        ▼
UI Refreshes Automatically
```

Business logic must never bypass the ViewModel.

---

# 6. View Layer

Responsibilities:

- Display information
- Data binding
- Styling
- Layout
- User interaction

Views should remain "thin."

Do not perform media processing inside code-behind.

---

# 7. ViewModel Layer

ViewModels coordinate the application.

Responsibilities include:

- Managing state
- Executing commands
- Calling services
- Updating observable properties
- Handling validation
- Managing queue state

They should never contain FFmpeg command construction or file-system specific logic.

---

# 8. Service Layer

Services encapsulate all business logic.

Current services include:

- FileDialogService
- FFprobeService
- ThumbnailService
- ConversionService
- BatchConversionService

Services should communicate through interfaces.

Future services can be added without modifying existing ViewModels.

---

# 9. Core Models

Models represent application data only.

Examples:

- MediaInfo
- ConversionRequest
- ConversionResult
- OutputFormat
- ConversionJob

Models should avoid business logic unless it is intrinsic to the data they represent.

---

# 10. Dependency Direction

Dependencies should always flow inward.

```
View
    ↓
ViewModel
    ↓
Interfaces
    ↓
Services
    ↓
Infrastructure
```

Never reference the UI layer from services.

Never reference Views from Core models.

This rule is fundamental to maintaining a clean, testable architecture.