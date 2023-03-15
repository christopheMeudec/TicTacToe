using Spectre.Console;
using TicTacToe.Engine;

namespace TicTacToe.Agents;

public class Human : IAgent
{
    public GridPosition Play(GameState.InProgress inProgress)
    {
        var selectedMove = AnsiConsole.Prompt(new SelectionPrompt<string>()
           .Title("Choose a [green]position[/]:")
           .PageSize(9)
           .AddChoices(inProgress.History.GetGrid().EmptyCells.Select(x => x.ToString())));

        return Enum.Parse<GridPosition>(selectedMove);
    }
}