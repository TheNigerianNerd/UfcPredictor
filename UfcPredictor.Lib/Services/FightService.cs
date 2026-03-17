using HtmlAgilityPack;
using UfcPredictor.Lib;

public class FightService
{
    private readonly IWebLoader _webLoader;

    public FightService(IWebLoader webLoader)
    {
        _webLoader = webLoader;
    }
    // This method scrapes the fight details from the given URL and returns a list of Fight objects
    public async Task<List<Fight>> GetUpcomingFightsAsync(string url)
    {
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
                    FighterOne = fighterNodes[0].InnerText.Trim(),
                    FighterOneUrl = fighterNodes[0].GetAttributeValue("href", ""),
                    FighterTwo = fighterNodes[1].InnerText.Trim(),
                    FighterTwoUrl = fighterNodes[1].GetAttributeValue("href", "")
                });
            }
        }
        return fights;
    }
    // This method scrapes the fighter details from the given URL and returns a Fighter object
    public async Task<Fighter> GetFighterDetailsAsync(string url)
    {
        HtmlDocument doc = await _webLoader.LoadFromWebAsync(url);
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