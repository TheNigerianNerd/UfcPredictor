using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
namespace UfcPredictor.Lib;

public class EventService
{
    private readonly IWebLoader _webLoader;

    //Dependency insjection allowes us to swap out the real loader for a mock one during testing
    public EventService(IWebLoader webLoader)
    {
        _webLoader = webLoader;
    }

    public async Task<List<Event>> GetUpcomingEvents(string url)
    {
        List<Event> events = new();
        HtmlDocument doc = await _webLoader.LoadFromWebAsync(url);

        // This XPath targets the rows in the main statistics table
        var eventRows = doc.DocumentNode.SelectNodes("//tr[contains(@class, 'b-statistics__table-row')]");

        if (eventRows != null)
        {
            // Chapter 3: Iterating through the collection
            foreach (var row in eventRows)
            {
                // The first <i> tag inside the row usually contains the link
                var linkNode = row.SelectSingleNode(".//i/a");
                var dateNode = row.SelectSingleNode(".//span[@class='b-statistics__date']");
                var locationNode = row.SelectSingleNode(".//td[contains(@class, 'b-statistics__table-col_style_big-top-padding')]");

                if (linkNode != null)
                {
                    // Chapter 5: Using Object Initializer syntax
                    events.Add(new Event
                    {
                        // Use a helper to scrub the text
                        Name = Clean(linkNode.InnerText),
                        Url = linkNode.GetAttributeValue("href", ""),
                        Date = Clean(dateNode?.InnerText),
                        Location = Clean(locationNode?.InnerText)
                    });
                }
            }
        }
        return events;
    }
    private string Clean(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        // 1. Decode any HTML entities like &nbsp; or &amp;
        string decoded = System.Net.WebUtility.HtmlDecode(input);

        // 2. Replace multiple whitespace/newlines with a single space
        // and trim the ends.
        return Regex.Replace(decoded, @"\s+", " ").Trim();
    }
}