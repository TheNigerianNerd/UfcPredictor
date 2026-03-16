using HtmlAgilityPack;

namespace UfcPredictor.Lib;

public class ScraperService
{
    private readonly HtmlWeb _web = new();

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
    public async Task<List<Fight>> GetUpcomingFightsAsync(string url)
    {
        List<Fight> fights = new();
        HtmlDocument doc = await _web.LoadFromWebAsync(url);
        var rows = doc.DocumentNode.SelectNodes("//tr[contains(@class, 'b-fight-details__table-row')]");

        foreach (var row in rows ?? Enumerable.Empty<HtmlNode>())
        {
            var fighterNodes = row.SelectNodes(".//a[@class='b-link b-link_style_black']");
            if (fighterNodes?.Count >= 2)
            {
                fights.Add(new Fight
                {
                    FighterOne = fighterNodes[0].InnerText.Trim(),
                    FighterOneUrl = fighterNodes[0].GetAttributeValue("href", ""),
                    FighterTwo = fighterNodes[1].InnerText.Trim(),
                    FighterTwoUrl = fighterNodes[1].GetAttributeValue("href", "")
                });
            }
        }
        return fights;
    }

    public async Task<Fighter> GetFighterDetailsAsync(string url)
    {
        HtmlDocument doc = await _web.LoadFromWebAsync(url);
        var fighter = new Fighter();

        fighter.Name = doc.DocumentNode.SelectSingleNode("//span[@class='b-content__title-highlight']")?.InnerText.Trim() ?? "Unknown";
        fighter.Record = doc.DocumentNode.SelectSingleNode("//span[@class='b-content__title-record']")?.InnerText.Trim();

        var infoNodes = doc.DocumentNode.SelectNodes("//li[contains(@class, 'b-list__info-box-item')]");
        foreach (var node in infoNodes ?? Enumerable.Empty<HtmlNode>())
        {
            string text = node.InnerText.Trim();
            if (text.Contains("Height:")) fighter.Height = text.Replace("Height:", "").Trim();
            if (text.Contains("Reach:")) fighter.Reach = text.Replace("Reach:", "").Trim();
            if (text.Contains("STANCE:")) fighter.Stance = text.Replace("STANCE:", "").Trim();
        }
        return fighter;
    }
}
