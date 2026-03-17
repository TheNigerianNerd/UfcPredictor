using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;

namespace UfcPredictor.Lib;

public class EventService
{
    private readonly IWebLoader _webLoader;
    private readonly IDataRepository _repository; // New Dependency

    // Update constructor to take 2 arguments
    public EventService(IWebLoader webLoader, IDataRepository repository)
    {
        _webLoader = webLoader;
        _repository = repository;
    }

    public async Task<List<Event>> GetUpcomingEvents(string url)
    {
        // 1. Try Cache First
        var cachedEvents = await _repository.GetEventsAsync();
        if (cachedEvents.Any()) return cachedEvents;

        // 2. Scrape if cache is empty
        List<Event> events = new();
        HtmlDocument doc = await _webLoader.LoadFromWebAsync(url);

        var eventRows = doc.DocumentNode.SelectNodes("//tr[contains(@class, 'b-statistics__table-row')]");

        if (eventRows != null)
        {
            foreach (var row in eventRows)
            {
                var linkNode = row.SelectSingleNode(".//i/a");
                var dateNode = row.SelectSingleNode(".//span[@class='b-statistics__date']");
                var locationNode = row.SelectSingleNode(".//td[contains(@class, 'b-statistics__table-col_style_big-top-padding')]");

                if (linkNode != null)
                {
                    events.Add(new Event
                    {
                        Name = Clean(linkNode.InnerText),
                        Url = linkNode.GetAttributeValue("href", ""),
                        Date = Clean(dateNode?.InnerText),
                        Location = Clean(locationNode?.InnerText)
                    });
                }
            }
        }

        // 3. Save to Cache for next time
        if (events.Any()) await _repository.SaveEventsAsync(events);

        return events;
    }

    private string Clean(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        string decoded = WebUtility.HtmlDecode(input);
        return Regex.Replace(decoded, @"\s+", " ").Trim();
    }
}