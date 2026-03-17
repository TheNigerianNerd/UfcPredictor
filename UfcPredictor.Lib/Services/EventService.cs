using System.Diagnostics.Contracts;
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
                        Name = linkNode.InnerText.Trim(),
                        Url = linkNode.GetAttributeValue("href", ""),
                        Date = dateNode?.InnerText.Trim(),
                        Location = locationNode?.InnerText.Trim()
                    });
                }
            }
        }
        return events;
    }
}