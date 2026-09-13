# AutoCommendYoutube

A Windows Forms desktop app that automates YouTube search and commenting in Chrome with Selenium WebDriver 4 (beta).

[![CICD AutoCommend](https://github.com/khangzxrr/AutoCommendYoutube/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/khangzxrr/AutoCommendYoutube/actions/workflows/dotnet-desktop.yml)
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![.NET Framework](https://img.shields.io/badge/.NET_Framework-4.7.2-512BD4?logo=dotnet&logoColor=white)
![Selenium](https://img.shields.io/badge/Selenium-4.0_beta-43B02A?logo=selenium&logoColor=white)

> **Disclaimer:** this is an old learning/experiment project (November 2020 to April 2021), shared for educational purposes only. It is not maintained and depends on YouTube page selectors and a Chrome version from that time. Automating YouTube accounts and posting automated comments violates YouTube's Terms of Service and spam policies. Don't use it against real accounts or channels.

## Overview

The project was a hands-on exercise in browser automation with .NET. A WinForms front end drives a Chrome instance through Selenium WebDriver.
The app searches YouTube for keywords and scrolls the result page to collect video links. It then opens each video, waits for the comment section and fills in and submits a comment from a local text file.
The repository also served as practice for setting up CI with GitHub Actions for a classic .NET Framework solution.

## Features

- **WinForms control panel**
  - Shows how many login profiles, keywords and comments were loaded.
  - Settings: number of videos per keyword, delay per video (ms), a "Today" upload-date search filter and multi-line comment parsing.
- **Selenium automation library** (`AutoYoutube.Core`)
  - `GetVideoUrls` builds the YouTube search URL and scrolls until it has collected the requested number of `a#video-title` links.
  - `LoadVideoAndComment` waits for the elements with `WebDriverWait` + `ExpectedConditions`, fills in the comment box through JavaScript and clicks submit.
- **Request-header profiles** (`LoginHeaders`)
  - Reads JSON header profiles and writes them into the ModHeader Chrome extension's storage.
  - Can compute a `SAPISIDHASH` authorization value (SHA-1).
- **Resilience**
  - Retries creating the Chrome driver, with an auto-closing error dialog.
  - Kills leftover `chrome` and `chromedriver` processes on start and on exit.
  - Recreates the driver after a timeout.
- **CI:** a GitHub Actions workflow on `windows-latest` restores NuGet packages, runs the unit tests and builds with MSBuild, then uploads the build output as an artifact.

## Tech stack

| Area       | Technology                                                        |
| ---------- | ----------------------------------------------------------------- |
| Language   | C#                                                                |
| UI         | Windows Forms (.NET Framework 4.7.2)                              |
| Automation | Selenium.WebDriver 4.0.0-beta2, ChromeDriver 89, DotNetSeleniumExtras.WaitHelpers |
| Other libs | Newtonsoft.Json, UI Automation (to close Chrome windows)          |
| Testing    | MSTest / NUnit (a startup smoke test)                             |
| CI         | GitHub Actions (`setup-msbuild`, `setup-nuget`, `upload-artifact`) |

## Project structure

```
.
├── CommentAutomatic.sln
├── AutoYoutube/              # WinForms app (CommentAutomatic.csproj)
│   ├── Form1.cs              # UI and the automation loop
│   ├── AutoClosingMessageBox.cs
│   └── App.config            # default_video_count
├── AutoYoutube.Core/         # Selenium extension methods (search, scroll, comment)
│   ├── WebDriver.cs
│   └── Constants/
├── LoginHeaders/             # Header-profile parsing and injection into ModHeader
├── AutoCommendTest/          # Unit test project
└── .github/workflows/dotnet-desktop.yml
```

## Getting started

### Prerequisites

- Windows with Visual Studio 2019 (the .NET desktop development workload)
- .NET Framework 4.7.2
- Google Chrome matching the bundled ChromeDriver (version 89)

### Build

```powershell
git clone https://github.com/khangzxrr/AutoCommendYoutube.git
cd AutoCommendYoutube
nuget restore CommentAutomatic.sln
msbuild CommentAutomatic.sln
```

You can also open `CommentAutomatic.sln` in Visual Studio and build it there.

### Configuration

The app reads these files from its working directory:

| File           | Purpose                                                           |
| -------------- | ----------------------------------------------------------------- |
| `key.txt`      | Search keywords, one per line                                     |
| `comment.txt`  | Comment text. In multi-line mode, comments are separated by `---` |
| `cookies.txt`  | One JSON request-header profile per line (`{"headers": {...}}`)   |
| `App.config`   | `default_video_count`                                             |

> Never commit real session cookies or authorization headers.

## CI

Every push or pull request to `main` triggers `.github/workflows/dotnet-desktop.yml`, which does the following:

1. Restores NuGet packages for `CommentAutomatic.sln`.
2. Runs the tests with `dotnet test`.
3. Builds with `msbuild`.
4. Uploads `AutoYoutube\bin\Debug` as the `AutoCommendYoutube` artifact.
