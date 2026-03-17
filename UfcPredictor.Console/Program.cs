using Spectre.Console;
using UfcPredictor.Lib;

//Production loader for a HTML web type
IWebLoader webLoader = new HtmlWebLoader();
//Services which return upcoming events and fights, and fighter details. They depend on the loader to get the raw HTML data, but they handle the parsing and data extraction logic themselves
EventService eventService = new EventService(webLoader);
FightService fightService = new FightService(webLoader);

bool isRunning = true;

while (isRunning)
{
    // LEVEL 1: Select Event
    var events = await eventService.GetUpcomingEvents("http://ufcstats.com/statistics/events/upcoming");
    var eventChoice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[yellow]Select an Event (or Exit):[/]")
            .AddChoices("[red]EXIT[/]")
            .AddChoices(events.Select(e => e.Name!)));

    if (eventChoice == "[red]EXIT[/]") break;
    var selectedEvent = events.First(e => e.Name == eventChoice);

    bool inEvent = true;
    while (inEvent)
    {
        // LEVEL 2: Select Fight
        var fights = await fightService.GetUpcomingFightsAsync(selectedEvent.Url!);
        var fightChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[blue]{selectedEvent.Name}[/] - Select a Fight:")
                .AddChoices("[red]<-- BACK[/]")
                .AddChoices(fights.Select(f => f.ToString())));

        if (fightChoice == "[red]<-- BACK[/]") { inEvent = false; continue; }
        var selectedFight = fights.First(f => f.ToString() == fightChoice);

        // LEVEL 3: Show Fighter Details (Tale of the Tape)
        await ShowTaleOfTheTape(fightService, selectedFight);
        AnsiConsole.MarkupLine("\n[grey]Press any key to return to the fight card...[/]");
        Console.ReadKey(true);
    }
}

async Task ShowTaleOfTheTape(FightService fightService, Fight fight)
{
    Fighter? f1 = null, f2 = null;
    await AnsiConsole.Status().StartAsync("Loading stats...", async ctx => {
        var t1 = fightService.GetFighterDetailsAsync(fight.FighterOneUrl!);
        var t2 = fightService.GetFighterDetailsAsync(fight.FighterTwoUrl!);
        await Task.WhenAll(t1, t2);
        f1 = t1.Result; f2 = t2.Result;
    });

    var table = new Table().Border(TableBorder.Rounded).Centered();
    table.AddColumn($"[blue]{f1!.Name}[/]");
    table.AddColumn("[yellow]VS[/]");
    table.AddColumn($"[red]{f2!.Name}[/]");
    table.AddRow(f1.Record ?? "--", "Record", f2.Record ?? "--");
    table.AddRow(f1.Height ?? "--", "Height", f2.Height ?? "--");
    table.AddRow(f1.Reach ?? "--", "Reach", f2.Reach ?? "--");
    table.AddRow(f1.Stance ?? "--", "Stance", f2.Stance ?? "--");
    AnsiConsole.Write(table);
}