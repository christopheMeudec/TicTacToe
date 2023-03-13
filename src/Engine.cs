namespace TicTacToe.Engine;

using History = IEnumerable<GridPosition>;

public enum Players { X, O }

public static class Ext
{
    public static Grid GetGrid(this History history) => new(history);

    public static Players Switch(this Players currentPlayer) => currentPlayer == Players.X ? Players.O : Players.X;
}

public enum GridPosition
{
    TopLeft = 7,
    TopCenter = 8,
    TopRight = 9,
    CenterLeft = 4,
    Middle = 5,
    CenterRight = 6,
    BottomLeft = 1,
    BottomCenter = 2,
    BottomRight = 3,
}
public class Grid
{
    private Dictionary<GridPosition, Players?> _game = Enum.GetValues<GridPosition>().ToDictionary(x => x, x => (Players?)null);

    public Grid(History history)
    {
        var currentPlay = Players.X;
        foreach (var p in history)
        {
            _game[p] = currentPlay;
            currentPlay = currentPlay.Switch();
        }
    }

    public IEnumerable<GridPosition> EmptyCells => _game.Where(c => c.Value == null).Select(x => x.Key);

    private IEnumerable<Players?[]> Rows
    {
        get
        {
            yield return new[] { _game[GridPosition.TopLeft], _game[GridPosition.TopCenter], _game[GridPosition.TopRight] };
            yield return new[] { _game[GridPosition.CenterLeft], _game[GridPosition.Middle], _game[GridPosition.CenterRight] };
            yield return new[] { _game[GridPosition.BottomLeft], _game[GridPosition.BottomCenter], _game[GridPosition.BottomRight] };
        }
    }
    private IEnumerable<Players?[]> Columns
    {
        get
        {
            yield return new[] { _game[GridPosition.TopLeft], _game[GridPosition.CenterLeft], _game[GridPosition.BottomLeft] };
            yield return new[] { _game[GridPosition.TopCenter], _game[GridPosition.Middle], _game[GridPosition.BottomCenter] };
            yield return new[] { _game[GridPosition.TopRight], _game[GridPosition.CenterRight], _game[GridPosition.BottomRight] };
        }
    }
    private IEnumerable<Players?[]> Diagonals
    {
        get
        {
            yield return new[] { _game[GridPosition.TopLeft], _game[GridPosition.Middle], _game[GridPosition.BottomRight] };
            yield return new[] { _game[GridPosition.TopRight], _game[GridPosition.Middle], _game[GridPosition.BottomLeft] };
        }
    }

    public Players? HasWinner()
    {
        var all = Rows.Concat(Columns).Concat(Diagonals);
        foreach (var x in all)
        {
            if (x.Distinct().Count() == 1)
                return x.First();
        }
        return null;
    }
}

public abstract record GameState
{
    public sealed record InProgress(History History, Players CurrentPlayer) : GameState();
    public sealed record Finished(History History, GameOutcome Outcome) : GameState();

    public T Switch<T>(Func<InProgress, T> inProgress, Func<Finished, T> finished)
    {
        return this switch
        {
            GameState.Finished f => finished(f),
            GameState.InProgress i => inProgress(i),
            _ => throw new NotImplementedException()
        };
    }
}

public abstract record GameOutcome
{
    public sealed record Won(Players By) : GameOutcome();
    public sealed record Tie() : GameOutcome();
}

public enum MoveError { AlreadyFinished, PositionAlreadyUsed }

public class TicTacToe
{
    public GameState State { get; private set; } = default!;
    public enum MoveError { AlreadyFinished, PositionAlreadyUsed }

    public TicTacToe()
    {
        NewGame();
    }

    private void NewGame()
    {
        LoadHistory(Array.Empty<GridPosition>());
    }

    private void LoadHistory(History gridPositions)
    {
        var grid = new Grid(gridPositions);

        var winner = grid.HasWinner();
        var emptyCells = grid.EmptyCells.Any();
        var nextPlayer = gridPositions.Count() % 2 == 0 ? Players.X : Players.O;

        State = (winner, emptyCells) switch
        {
            (null, true) => new GameState.InProgress(gridPositions, nextPlayer),
            (null, false) => new GameState.Finished(gridPositions, new GameOutcome.Tie()),
            _ => new GameState.Finished(gridPositions, new GameOutcome.Won(winner.Value))
        };
    }

    public MoveError? IsValidMove(GridPosition position) => State.Switch(
       inProgress: i => i.History.Contains(position) ? MoveError.PositionAlreadyUsed : (MoveError?)null,
       finished: f => MoveError.AlreadyFinished
     );

    public MoveError? Play(GridPosition position)
    {
        var valid = IsValidMove(position);
        if (valid != null)
            return valid;

        var inProgress = (GameState.InProgress)State;

        LoadHistory(inProgress.History.Append(position));

        return null;
    }
}
