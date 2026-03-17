namespace UfcPredictor.Lib;

public class Fight
{
    public string? FighterOne { get; set; }
    public string? FighterOneUrl { get; set; }
    public string? FighterTwo { get; set; }
    public string? FighterTwoUrl { get; set; }

    public override string ToString() => $"{FighterOne} vs {FighterTwo}";
}