using UfcPredictor.Lib;

ScraperService scraper = new();
string url = "http://ufcstats.com/statistics/events/upcoming";

Console.WriteLine("--- UFC Event Explorer ---");

// Chapter 2: Using await for the asynchronous task
List<Event> upcomingEvents = await scraper.GetUpcomingEvents(url);

foreach (var upcEvent in upcomingEvents)
{
    Console.WriteLine($" -> {upcEvent}");
}

Console.WriteLine("Enter event number to pull all associated events");
if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0 && choice < upcomingEvents.Count) 
{ 
    var selectedEvent = upcomingEvents[choice];
    Console.WriteLine($"Fetching fights for: {selectedEvent.Name}");

    // Step 3: Fetch fights for that specific URL
    // We can reuse your existing scraping logic here
    var fights = await scraper.GetUpcomingFightsAsync(selectedEvent.Url!);

    foreach (var fight in fights)
    {
        Console.WriteLine($"{fight}");
    }
}
else
{
    Console.WriteLine("Invalid selection.");
}