using HtmlAgilityPack;

namespace UfcPredictor.Lib;

public class ScraperService
{
    private readonly HtmlWeb _web = new();

    public async Task<List<Fight>> GetUpcomingFightsAsync(string url)
    {
        List<Fight> fights = new();
        HtmlDocument doc = await _web.LoadFromWebAsync(url);

        // Chapter 3/4: Use SelectSingleNode and handle nulls safely
        var nextEventNode = doc.DocumentNode.SelectSingleNode("//td[@class='b-statistics__table-col']/i/a");

        if (nextEventNode is null) return fights;

        string eventUrl = nextEventNode.GetAttributeValue("href", "");
        HtmlDocument eventDoc = await _web.LoadFromWebAsync(eventUrl);

        var rows = eventDoc.DocumentNode.SelectNodes("//tr[contains(@class, 'b-fight-details__table-row')]");

        foreach (var row in rows ?? Enumerable.Empty<HtmlNode>())
        {
            var fighters = row.SelectNodes(".//a[@class='b-link b-link_style_black']");
            if (fighters?.Count >= 2)
            {
                // Chapter 5: Instantiating our custom type
                fights.Add(new Fight
                {
                    FighterOne = fighters[0].InnerText.Trim(),
                    FighterTwo = fighters[1].InnerText.Trim()
                });
            }
        }
        return fights;
    }
}
