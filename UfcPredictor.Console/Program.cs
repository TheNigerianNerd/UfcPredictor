using Spectre.Console;
using UfcPredictor.Lib;

// 1. Dependency Injection setup (Manual for now)
IWebLoader webLoader = new HtmlWebLoader();
IDataRepository repository = new FileRepository(); // New Persistent Storage

// 2. Services now receive BOTH the loader and the repository
EventService eventService = new EventService(webLoader, repository);
FightService fightService = new FightService(webLoader, repository);

bool isRunning = true;

while (isRunning)
{
    // The service handles the "Check Cache -> If Null -> Scrape" logic internally
    var events = await eventService.GetUpcomingEvents("http://ufcstats.com/statistics/events/upcoming");

    var eventChoice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[yellow]Select an Event (or Exit):[/]")
            .PageSize(10) // Better UX for long lists
            .AddChoices("[red]EXIT[/]")
            .AddChoices(events.Select(e => e.Name!)));

    if (eventChoice == "[red]EXIT[/]") break;
    var selectedEvent = events.First(e => e.Name == eventChoice);

    bool inEvent = true;
    while (inEvent)
    {
        // FightService also uses the new cached logic
        var fights = await fightService.GetUpcomingFightsAsync(selectedEvent.Url!);

        var fightChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[blue]{selectedEvent.Name}[/] - Select a Fight:")
                .AddChoices("[red]<-- BACK[/]")
                .AddChoices(fights.Select(f => f.ToString())));

        if (fightChoice == "[red]<-- BACK[/]") { inEvent = false; continue; }
        var selectedFight = fights.First(f => f.ToString() == fightChoice);

        // LEVEL 3: Tale of the Tape (Now lightning fast if cached!)
        await ShowTaleOfTheTape(fightService, selectedFight);

        AnsiConsole.MarkupLine("\n[grey]Press any key to return to the fight card...[/]");
        Console.ReadKey(true);
    }
}

async Task ShowTaleOfTheTape(FightService service, Fight fight)
{
    Fighter? f1 = null, f2 = null;

    // 1. Data Fetching (with the Status spinner for UX)
    await AnsiConsole.Status()
        .Spinner(Spinner.Known.Dots)
        .StartAsync("[yellow]Retrieving Tale of the Tape...[/]", async ctx =>
        {
            var t1 = service.GetFighterWithCacheAsync(fight.FighterOneUrl!, fight.FighterOne!);
            var t2 = service.GetFighterWithCacheAsync(fight.FighterTwoUrl!, fight.FighterTwo!);
            await Task.WhenAll(t1, t2);
            f1 = t1.Result;
            f2 = t2.Result;
        });

    // 2. Defensive Check (Senior QA Style)
    if (f1 == null || f2 == null)
    {
        AnsiConsole.MarkupLine("[red]Error: Could not load fighter data.[/]");
        return;
    }

    // 3. Table Construction
    var table = new Table()
        .Border(TableBorder.Rounded)
        .BorderColor(Color.Grey)
        .Title($"[yellow]TALE OF THE TAPE[/]")
        .Centered();

    // Columns for Fighter 1, the Label, and Fighter 2
    table.AddColumn(new TableColumn($"[blue]{f1.Name}[/]").Centered());
    table.AddColumn(new TableColumn("[grey]VS[/]").Centered());
    table.AddColumn(new TableColumn($"[red]{f2.Name}[/]").Centered());

    // Add Rows (Record, Height, Reach, Stance)
    table.AddRow(f1.Record ?? "--", "[bold]Record[/]", f2.Record ?? "--");
    table.AddRow(f1.Height ?? "--", "[bold]Height[/]", f2.Height ?? "--");
    table.AddRow(f1.Reach ?? "--", "[bold]Reach[/]", f2.Reach ?? "--");
    table.AddRow(f1.Stance ?? "--", "[bold]Stance[/]", f2.Stance ?? "--");

    // 4. Render to Console
    AnsiConsole.Write(table);

    // Summary line for flavor
    var dataSource = f1.IsFromCache ? "[green]Local Cache[/]" : "[cyan]Live Scrape[/]";
    AnsiConsole.MarkupLine($"\n[italic grey]Data source: {dataSource}[/]");
}