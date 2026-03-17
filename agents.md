🎯 Project Mission

A .NET 8 CLI tool for UFC event discovery and fight prediction, utilizing a "Tale of the Tape" deep-scrape architecture.
🏗️ Technical Stack

    Language/Runtime: C# 12 / .NET 8.0

    UI Framework: Spectre.Console (SelectionPrompt, Tables, Status/Spinners).

    HTML Parsing: HtmlAgilityPack using XPath selectors.

    Architecture: * UfcPredictor.Lib: Core models (Event, Fight, Fighter) and ScraperService.

        UfcPredictor.Console: UI and Navigation State Machine.

        UfcPredictor.Tests: xUnit + Moq + FluentAssertions.

📜 Established Patterns & Constraints

    Testable Scraper: Always use the IWebLoader interface and Dependency Injection. Never instantiate HtmlWeb directly inside FightService or EventService.

    Navigation State: The UI uses nested while loops with a [Back] option to manage navigation depth (Events -> Fights -> Fighter Details).

    Parallel Execution: Use Task.WhenAll when fetching multiple fighter details to optimize performance.

    Data Integrity: Use .Trim() and .Replace() during scraping to sanitize HTML whitespace.

    Test-Driven Design/Development: Cover services with valid tests, ensuring edge cases such as &nbsp and &amp are adequately covered by application logic to avoid breaking logic

    Repository Pattern: UI components must call IDataRepository, which handles the logic between Scraped data and Cached data.

    Cache Policy: Scraped data is considered "Stale" after 48 hours.

🔍 Active XPaths (Source: ufcstats.com)

    Upcoming Events: //tr[contains(@class, 'b-statistics__table-row')]

    Event Fights: //tr[contains(@class, 'b-fight-details__table-row')]

    Fighter Profile Name: //span[@class='b-content__title-highlight']

    Fighter Stats List: //li[contains(@class, 'b-list__info-box-item')]

🚀 Pending Roadmap

    [ ] Sprint 4: Implement XML/JSON Caching (Local Data Source).

    [ ] Sprint 5: Build the Prediction Logic (Algorithm-based "Winner" selection).

    [ ] Sprint 6: Refactor XPaths into appsettings.json.

    [ ] Sprint 7: Integration Test suite for live site monitoring.

💡 Agent Instructions

When resuming this project:

    Read this file to align with the existing namespace structure (UfcPredictor).

    Assume the user is a Senior .NET QA Automation Engineer—focus on code quality, resiliency, and testability.

    Reference Mark J. Price's .NET book concepts (Chapters 1-5) for educational context.