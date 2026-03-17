# UFC Predictor CLI

A .NET 8 console application that scrapes upcoming UFC events and fighter statistics to provide a "Tale of the Tape" comparison.

## Tech Stack
- **Backend:** C# 12 / .NET 8
- **UI:** Spectre.Console
- **Parsing:** HtmlAgilityPack
- **Architecture:** Class Library (Lib) + Console App

## Getting Started
1. Clone the repo.
2. Run `dotnet restore`.
3. Run `dotnet run --project UfcPredictor.Console`.

## Roadmap
- [x] Web Scraping MVP
- [x] Interactive CLI Navigation
- [x] TDD & xUnit Test Suite
- [ ] Local JSON Data Caching (In Progress)
- [ ] Web Client (Blazor/React) Integration
- [ ] Prediction Engine (v1)