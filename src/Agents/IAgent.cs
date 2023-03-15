using TicTacToe.Engine;

namespace TicTacToe.Agents;

public interface IAgent
{
    GridPosition Play(GameState.InProgress inProgress);
}
