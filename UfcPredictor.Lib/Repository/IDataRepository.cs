namespace UfcPredictor.Lib;

public interface IDataRepository
{
    Task SaveFighterAsync(Fighter fighter);
    Task<Fighter?> GetFighterAsync(string name);
    
    Task SaveEventsAsync(List<Event> events);
    Task<List<Event>> GetEventsAsync();

    // MISSING METHODS: Add these now
    Task SaveFightsAsync(string eventUrl, List<Fight> fights);
    Task<List<Fight>> GetFightsAsync(string eventUrl);
}