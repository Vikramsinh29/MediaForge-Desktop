# AI_RULES.md

## Project

MediaForge Desktop

## Architecture Rules

- Never redesign the architecture.
- Follow the existing MVVM architecture.
- Reuse existing services, models and helpers.
- Do not duplicate logic.
- Keep the project buildable after every change.
- One sprint = One feature = One build = One commit.
- Make the smallest possible change.
- Preserve backward compatibility.
- Use existing naming conventions.
- Use CommunityToolkit.Mvvm patterns already present.
- Follow existing dependency injection and project structure.
- Never modify unrelated files.

## Development Workflow

Before making changes:

1. Inspect the current implementation.
2. Reuse existing architecture.
3. Identify only the required files.
4. Modify only those files.
5. Build the solution.
6. Fix compilation errors.
7. Run tests if available.
8. Produce a single atomic Git commit.

## Coding Standards

- Keep methods small.
- Keep classes focused.
- Avoid magic strings.
- Use async/await where appropriate.
- Handle exceptions gracefully.
- Log meaningful errors.
- Keep UI responsive.
- Prefer readability over cleverness.

## Output Requirements

- Do not generate placeholder implementations.
- Do not hallucinate files or classes.
- Do not rename existing public APIs unless required.
- Preserve existing functionality.
- Explain any assumptions before modifying code if repository state is unclear.

## Build

Always verify:

dotnet build MediaForge.slnx

before considering a task complete.