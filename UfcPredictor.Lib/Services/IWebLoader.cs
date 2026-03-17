using HtmlAgilityPack;

namespace UfcPredictor.Lib;

public interface IWebLoader
{
    Task<HtmlDocument> LoadFromWebAsync(string url);
}