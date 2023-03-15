using TicTacToe.Engine;

namespace TicTacToe.Agents;

public class RandomAI : IAgent
{
    public GridPosition Play(GameState.InProgress inProgress)
    {
        var randomDelay = Random.Shared.NextInt64(250, 1000);
        Thread.Sleep((int)randomDelay);

        var emptyCells = inProgress.History.GetGrid().EmptyCells.ToArray();
        var selectedPosition = Random.Shared.Next(emptyCells.Length);

        return emptyCells[selectedPosition];
    }
}