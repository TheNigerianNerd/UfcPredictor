namespace UfcPredictor.Lib;

public class Fight 
{ 
    public string? FighterOne { get; set; }
    public string? FighterTwo { get; set; }
    public string? EventName { get; set; }

    public override string ToString() => $"{FighterOne} vs. {FighterTwo}";
}