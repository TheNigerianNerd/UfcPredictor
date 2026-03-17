🥊 UFC Predictor CLI

A high-performance .NET 10 console application designed for UFC event discovery and fighter analysis. This project utilizes a local-first, cache-aside architecture to scrape, persist, and compare fighter statistics for upcoming events.
🏛️ Architecture & Patterns

This solution is built with a focus on testability and separation of concerns:

    Repository Pattern: Decouples the service logic from the file system. UI components interact with IDataRepository, which orchestrates data between the live web and local JSON storage.

    Cache-Aside Pattern: Implements a 7-day TTL (Time-To-Live) policy. Stale data is automatically invalidated and re-scraped to ensure "Tale of the Tape" accuracy.

    Dependency Injection: All services are constructor-injected, allowing for 100% mock coverage of network and disk I/O.

    Clean Scraping: Utilizes Regex and WebUtility decoding to handle inconsistent HTML entities (&nbsp;, &amp;) and whitespace from source providers.

🛠️ Tech Stack

    Runtime: C# 14 / .NET 10.0

    UI: Spectre.Console (Rich tables, live status spinners, and nested navigation).

    Parsing: HtmlAgilityPack with XPath optimization.

    Testing: xUnit, Moq, and FluentAssertions.

    Persistence: System.Text.Json with case-insensitive mapping.

🚀 Getting Started

    Clone the repo: git clone https://github.com/your-repo/UfcPredictor

    Restore Dependencies: dotnet restore

    Run Tests: dotnet test (Verify cache-hit logic and scraper resiliency)

    Launch CLI: dotnet run --project UfcPredictor.Console

📅 Roadmap

    [x] Sprint 1: Web Scraping MVP (HtmlAgilityPack)

    [x] Sprint 2: Interactive CLI Navigation (Spectre.Console)

    [x] Sprint 3: TDD & xUnit Test Suite (Moq/FluentAssertions)

    [x] Sprint 4: Local JSON Data Caching & Repository Pattern

    [ ] Sprint 5: Prediction Engine v1 (Reach/Stance/Record weight-based algorithms)

    [ ] Sprint 6: Web Client Integration (Blazor WebAssembly)