// Fighter.cs
using System.Text.Json.Serialization;

public class Fighter
{
    public string Name { get; set; } = "";
    public string? Height { get; set; }
    public string? Reach { get; set; }
    public string? Stance { get; set; }
    public string? Record { get; set; }

    [JsonIgnore] // We don't want to save this state to the JSON file
    public bool IsFromCache { get; set; } = false;
}