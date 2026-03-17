using HtmlAgilityPack;
using UfcPredictor.Lib;

public class HtmlWebLoader : IWebLoader
{
    private readonly HtmlWeb _web = new();
    public async Task<HtmlDocument> LoadFromWebAsync(string url) => await _web.LoadFromWebAsync(url);
}