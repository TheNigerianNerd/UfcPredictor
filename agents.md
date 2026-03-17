Here is the updated AGENTS.md reflecting our shift to .NET 10, the completion of the Repository Pattern, and our refined caching logic. This version is primed for a Senior QA profile—clean, structured, and focused on the "Source of Truth."
🎯 Project Mission

A .NET 10 CLI tool for UFC event discovery and fight prediction, utilizing a Repository-backed "Tale of the Tape" deep-scrape architecture.
🏗️ Technical Stack

    Language/Runtime: C# 14 / .NET 10.0

    UI Framework: Spectre.Console (SelectionPrompt, Tables, Status/Spinners).

    HTML Parsing: HtmlAgilityPack with specialized Clean() regex/entity decoding.

    Persistence: JSON-based local caching via System.Text.Json.

    Testing: xUnit + Moq + FluentAssertions.

📜 Established Patterns & Constraints

    Repository Pattern: IDataRepository acts as the gatekeeper. Services (EventService, FightService) coordinate between the IWebLoader (Network) and the IDataRepository (Disk).

    Cache-Aside Pattern: 1. Check Cache.
    2. If missing/expired, Scrape Web.
    3. Update Cache.

    Testable Scrapers: Constructor Injection is mandatory. All services require IWebLoader and IDataRepository. Tests must verify that web calls are skipped when valid cache exists.

    Data Resiliency: The Clean() helper handles &nbsp;, &amp;, and messy whitespace via WebUtility.HtmlDecode and Regex \s+.

    Parallel Execution: Task.WhenAll is used in the UI layer to hydrate multiple fighter profiles simultaneously.

🔍 Active XPaths (Source: ufcstats.com)

    Upcoming Events: //tr[contains(@class, 'b-statistics__table-row')]

    Event Fights: //tr[contains(@class, 'b-fight-details__table-row')]

    Fighter Profile Name: //span[@class='b-content__title-highlight']

    Fighter Stats List: //li[contains(@class, 'b-list__box-list-item')] (Note: Updated from info-box to box-list).

🚀 Roadmap Progress

    [x] Sprint 3: Comprehensive Test Suite & TDD.

    [x] Sprint 4: Local JSON Caching & Repository Pattern.

    [ ] Sprint 5: Prediction Engine (Algorithm-based "Winner" selection).

    [ ] Sprint 6: Refactor XPaths into appsettings.json.

    [ ] Sprint 7: Integration Test suite for live site monitoring.

💡 Agent Instructions

    Namespace: UfcPredictor.Lib, UfcPredictor.Console, UfcPredictor.Tests.

    Persona: User is a Senior .NET QA Engineer. Prioritize "Fail-fast" logic, defensive coding, and verifiable test counts.

    Context: Align with Mark J. Price’s .NET concepts regarding Dependency Injection and File System I/O (Chapters 5 & 9).

    Cache Policy: Fighter and Event data expires after 7 days (configured via CacheExpirationDays).