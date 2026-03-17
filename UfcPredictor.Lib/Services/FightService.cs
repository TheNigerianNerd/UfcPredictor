using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;

namespace UfcPredictor.Lib;

public class FightService
{
    private readonly IWebLoader _webLoader;
    private readonly IDataRepository _repository;

    public FightService(IWebLoader webLoader, IDataRepository repository)
    {
        _webLoader = webLoader;
        _repository = repository;
    }

    public async Task<List<Fight>> GetUpcomingFightsAsync(string url)
    {
        // 1. Check Cache (using the URL as a unique key for the event's fight list)
        var cachedFights = await _repository.GetFightsAsync(url);
        if (cachedFights.Any()) return cachedFights;

        // 2. Scrape if not cached
        List<Fight> fights = new();
        HtmlDocument doc = await _webLoader.LoadFromWebAsync(url);
        var rows = doc.DocumentNode.SelectNodes("//tr[contains(@class, 'b-fight-details__table-row')]");

        foreach (var row in rows ?? Enumerable.Empty<HtmlNode>())
        {
            var fighterNodes = row.SelectNodes(".//a[@class='b-link b-link_style_black']");
            if (fighterNodes?.Count >= 2)
            {
                fights.Add(new Fight
                {
                    FighterOne = Clean(fighterNodes[0].InnerText),
                    FighterOneUrl = fighterNodes[0].GetAttributeValue("href", ""),
                    FighterTwo = Clean(fighterNodes[1].InnerText),
                    FighterTwoUrl = fighterNodes[1].GetAttributeValue("href", "")
                });
            }
        }

        // 3. Persist
        if (fights.Any()) await _repository.SaveFightsAsync(url, fights);
        
        return fights;
    }

    public async Task<Fighter> GetFighterWithCacheAsync(string url, string name)
    {
        // 1. Try Cache
        var cached = await _repository.GetFighterAsync(name);
        if (cached != null) return cached;

        // 2. Scrape
        HtmlDocument doc = await _webLoader.LoadFromWebAsync(url);
        var fighter = new Fighter
        {
            Name = Clean(doc.DocumentNode.SelectSingleNode("//span[@class='b-content__title-highlight']")?.InnerText) ?? "Unknown",
            Record = Clean(doc.DocumentNode.SelectSingleNode("//span[@class='b-content__title-record']")?.InnerText)
        };

        var infoNodes = doc.DocumentNode.SelectNodes("//li[contains(@class, 'b-list__box-list-item')]");
        if (infoNodes != null)
        {
            foreach (var node in infoNodes)
            {
                var labelNode = node.SelectSingleNode(".//i");
                if (labelNode == null) continue;

                string label = labelNode.InnerText.Trim().ToLower();
                string value = Clean(node.InnerText.Replace(labelNode.InnerText, string.Empty));

                if (label.Contains("height")) fighter.Height = value;
                if (label.Contains("reach")) fighter.Reach = value;
                if (label.Contains("stance")) fighter.Stance = value;
            }
        }

        // 3. Save to local storage
        await _repository.SaveFighterAsync(fighter);
        return fighter;
    }

    private string Clean(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        string decoded = WebUtility.HtmlDecode(input);
        return Regex.Replace(decoded, @"\s+", " ").Trim();
    }
}