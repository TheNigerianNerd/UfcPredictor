using System.Text.Json;

namespace UfcPredictor.Lib;

public class FileRepository : IDataRepository
{
    private readonly string _cacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cache");
    private readonly string _eventsFile;
    private const int CacheExpirationDays = 7;

    public FileRepository()
    {
        if (!Directory.Exists(_cacheDir)) Directory.CreateDirectory(_cacheDir);
        _eventsFile = Path.Combine(_cacheDir, "upcoming_events.json");
    }

    public async Task SaveFighterAsync(Fighter fighter)
    {
        var fileName = Path.Combine(_cacheDir, $"fighter_{Sanitize(fighter.Name)}.json");
        var json = JsonSerializer.Serialize(fighter, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(fileName, json);
    }
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
    public async Task<Fighter?> GetFighterAsync(string name)
    {
        var fileName = Path.Combine(_cacheDir, $"fighter_{Sanitize(name)}.json");

        // 1. Check if the file exists at all
        if (!File.Exists(fileName)) return null;

        // 2. Freshness check: Is the data older than our policy (e.g., 7 days)?
        // If it's stale, we return null to trigger a fresh scrape.
        if (File.GetLastWriteTime(fileName) < DateTime.Now.AddDays(-CacheExpirationDays))
        {
            return null;
        }

        // 3. Read and Deserialize
        var json = await File.ReadAllTextAsync(fileName);
        var fighter = JsonSerializer.Deserialize<Fighter>(json, _options);

        // 4. Set the UI flag so the Console knows this came from disk
        if (fighter != null)
        {
            fighter.IsFromCache = true;
        }

        return fighter;
    }

    // Helper to ensure file names don't contain illegal characters
    private string Sanitize(string key)
    {
        // Remove protocol and illegal characters
        string safeKey = key.Replace("http://", "").Replace("https://", "").Replace("/", "_");
        return string.Join("_", safeKey.Split(Path.GetInvalidFileNameChars())).Trim('_');
    }

    // Implementation for SaveEventsAsync and GetEventsAsync would follow a similar pattern
    public async Task SaveEventsAsync(List<Event> events)
    {
        var json = JsonSerializer.Serialize(events, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_eventsFile, json);
    }
    public async Task<List<Event>> GetEventsAsync()
    {
        if (!File.Exists(_eventsFile)) return new List<Event>();

        // Optional: Check if the cache is older than our policy
        var lastModified = File.GetLastWriteTime(_eventsFile);
        if (lastModified < DateTime.Now.AddDays(-CacheExpirationDays))
        {
            return new List<Event>(); // Treat expired cache as "not found"
        }

        var json = await File.ReadAllTextAsync(_eventsFile);
        return JsonSerializer.Deserialize<List<Event>>(json) ?? new List<Event>();
    }
    public async Task SaveFightsAsync(string eventUrl, List<Fight> fights)
    {
        // Create a unique filename based on the event's URL
        var fileName = Path.Combine(_cacheDir, $"fights_{Sanitize(eventUrl)}.json");
        var json = JsonSerializer.Serialize(fights, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(fileName, json);
    }

    public async Task<List<Fight>> GetFightsAsync(string eventUrl)
    {
        var fileName = Path.Combine(_cacheDir, $"fights_{Sanitize(eventUrl)}.json");

        if (!File.Exists(fileName)) return new List<Fight>();

        // Cache check: Is the fight card older than 7 days?
        if (File.GetLastWriteTime(fileName) < DateTime.Now.AddDays(-7))
        {
            return new List<Fight>();
        }

        var json = await File.ReadAllTextAsync(fileName);
        return JsonSerializer.Deserialize<List<Fight>>(json) ?? new List<Fight>();
    }
}