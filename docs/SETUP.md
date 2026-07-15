# MediaForge Desktop Setup

## Requirements

- Windows 10/11
- .NET 10 SDK
- Visual Studio 2022

## Clone

```powershell
git clone https://github.com/Vikramsinh29/MediaForge-Desktop.git
```

## Download FFmpeg

Download the latest full build from:

https://www.gyan.dev/ffmpeg/builds/

Extract the following files into:

```
tools/
└── ffmpeg/
    ├── ffmpeg.exe
    └── ffprobe.exe
```

The FFmpeg binaries are intentionally **not stored in Git** because GitHub rejects files larger than 100 MB.

## Build

```powershell
dotnet restore
dotnet build
```

## Run

```powershell
dotnet run --project .\src\MediaForge
```

## Verify

```powershell
.\tools\ffmpeg\ffprobe.exe -version
```

If the version is displayed, the setup is complete.