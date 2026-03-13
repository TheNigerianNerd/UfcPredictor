using UfcPredictor.Lib;

ScraperService scraper = new();
string url = "http://ufcstats.com/statistics/events/upcoming";

Console.WriteLine("--- UFC Fight Scraper ---");

// Chapter 2: Using await for the asynchronous task
List<Fight> upcomingFights = await scraper.GetUpcomingFightsAsync(url);

// Chapter 3: Iterating with a count
Console.WriteLine($"Found {upcomingFights.Count} fights on the next card:");

foreach (var fight in upcomingFights)
{
    Console.WriteLine($" -> {fight}");
}