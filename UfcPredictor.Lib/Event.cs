public class Event 
{
    public string? Name { get; set; }
    public string? Date { get; set; }    
    public string? Url { get; set; }
    public string? Location { get; set; }

    public override string ToString() => $"{Name} on {Date:D} at {Location}.URL: {Url}";
}