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

    // BUG FIX: Updated class name to match actual HTML: b-list__box-list-item
    var infoNodes = doc.DocumentNode.SelectNodes("//li[contains(@class, 'b-list__box-list-item')]");

    if (infoNodes != null)
    {
        foreach (var node in infoNodes)
        {
            // We look for the <i> child which contains the label
            var labelNode = node.SelectSingleNode(".//i");
            if (labelNode == null) continue;

            string label = labelNode.InnerText.Trim().ToLower();
            
            // The value is the text of the <li> MINUS the text of the <i>
            // .Replace() is fine here, but we ensure we are targeting the right strings
            string fullText = node.InnerText;
            string value = fullText.Replace(labelNode.InnerText, string.Empty);

            value = value.Trim();

            if (label.Contains("height")) fighter.Height = value;
            if (label.Contains("reach")) fighter.Reach = value;
            if (label.Contains("stance")) fighter.Stance = value;
        }
    }
    return fighter;
}
}