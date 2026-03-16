using HtmlAgilityPack;

namespace UfcPredictor.Lib;

public class ScraperService
{
    private readonly HtmlWeb _web = new();

    public async Task<List<Fight>> GetUpcomingFightsAsync(string url)
    {
        List<Fight> fights = new();
        HtmlDocument doc = await _web.LoadFromWebAsync(url);

        // This targets the fight rows on a specific event page
        var fightRows = doc.DocumentNode.SelectNodes("//tr[contains(@class, 'b-fight-details__table-row')]");

        foreach (var row in fightRows ?? Enumerable.Empty<HtmlNode>())
        {
            var fighters = row.SelectNodes(".//a[@class='b-link b-link_style_black']");
            if (fighters?.Count >= 2)
            {
                fights.Add(new Fight
                {
                    FighterOne = fighters[0].InnerText.Trim(),
                    FighterTwo = fighters[1].InnerText.Trim()
                });
            }
        }
        return fights;
    }
    public async Task<List<Event>> GetUpcomingEvents(string url)
    {
        List<Event> events = new();
        HtmlDocument doc = await _web.LoadFromWebAsync(url);

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
                var locationNode = row.SelectSingleNode(".//td[contains(@class, 'b-statistics__table-col_style_margin-right')]");

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
