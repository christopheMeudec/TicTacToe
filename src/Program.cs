using Spectre.Console;
using TicTacToe.Agents;
using TicTacToe.Engine;

public class Program
{

    private static readonly IAgent agentX = new Human();
    private static readonly IAgent agentY = new RandomAI();

    public static void Main()
    {
        var game = new TicTacToe.Engine.TicTacToe();
        RenderGame(game);
    }

    private static void RenderGame(TicTacToe.Engine.TicTacToe game)
    {
        while (game.State is GameState.InProgress inProgress)
        {
             RenderInProgress(inProgress);
             var currentAgent = inProgress.CurrentPlayer == Players.X ? agentX : agentY;
            var move = currentAgent.Play(inProgress);
            game.Play(move);
        }

        if (game.State is GameState.Finished finished)
        {
            var playNewGame = RenderFinished(finished);
            if (playNewGame)
            {
                game.NewGame();
                RenderGame(game);
            }
        }
    }

    private static bool RenderFinished(GameState.Finished finished)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Markup($"[bold]Finished [/]").Centered());
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();
        RenderGrid(finished.History.GetGrid());

        if (finished.Outcome is GameOutcome.Tie tie)
        {
            AnsiConsole.Write(new Markup($"[bold]Tie : [/]").Centered());
        }
        if (finished.Outcome is GameOutcome.Won won)
        {
            AnsiConsole.Write(new Markup($"[bold]Player {GetPlayerName(won.By)} Win :[/]").Centered());
        }

        AnsiConsole.WriteLine();

        var newGame = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                   .Title("Play new game ?")
                   .AddChoices(new[] { "Yes", "No" }));

        return newGame.Equals("Yes");
    }

    private static void RenderInProgress(GameState.InProgress inProgress)
    {
        var grid = inProgress.History.GetGrid();

        AnsiConsole.Clear();
        RenderCurrentPlayer(inProgress.CurrentPlayer);
        RenderGrid(grid);
    }

    private static void RenderGrid(TicTacToe.Engine.Grid grid)
    {
        var table = new Table()
            .Centered()
            .Border(TableBorder.Square)
            .BorderColor(Color.DarkOrange);

        table.AddColumn(new TableColumn(
            GetPlayerName(grid[GridPosition.TopLeft]))
            .Footer(GetPlayerName(grid[GridPosition.BottomLeft]))
            .Centered());

        table.AddColumn(new TableColumn(
            GetPlayerName(grid[GridPosition.TopCenter]))
            .Footer(GetPlayerName(grid[GridPosition.BottomCenter]))
            .Centered());

        table.AddColumn(new TableColumn(
            GetPlayerName(grid[GridPosition.TopRight]))
            .Footer(GetPlayerName(grid[GridPosition.BottomRight]))
            .Centered());

        table.AddRow(
        GetPlayerName(grid[GridPosition.CenterLeft]),
        GetPlayerName(grid[GridPosition.Middle]),
        GetPlayerName(grid[GridPosition.CenterRight]));

        AnsiConsole.Write(table);
    }

    private static void RenderCurrentPlayer(Players currentPlayer)
    {
        AnsiConsole.Write(new Markup($"[bold]Turn of player [/]").Centered());
        AnsiConsole.Write(new Markup(GetPlayerName(currentPlayer)).Centered());

        AnsiConsole.WriteLine();
    }

    private static string GetPlayerName(Players? currentPlayer)
    {
        return currentPlayer switch
        {
            Players.O => "[red]O[/]",
            Players.X => "[blue]X[/]",
            _ => " "
        };
    }
}