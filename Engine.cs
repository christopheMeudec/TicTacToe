namespace TicTacToe.Engine;

using History = IEnumerable<GridPosition>;

public enum Player
{
    X,
    O
}

public static class Ext
{
    public static Player Switch(this Player currentPlayer) => currentPlayer == Player.X ? Player.O : Player.X;
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
    private Dictionary<GridPosition, Player?> _game = Enum.GetValues<GridPosition>().ToDictionary(x => x, x => (Player?)null);

    public Grid(History history)
    {
        var currentPlay = Player.X;
        foreach (var p in history)
        {
            _game[p] = currentPlay;
            currentPlay = currentPlay.Switch();
        }
    }

    public IEnumerable<GridPosition> EmptyCells => _game.Where(c => c.Value == null).Select(x => x.Key);

    private IEnumerable<Player?[]> Rows
    {
        get
        {
            yield return new[] { _game[GridPosition.TopLeft], _game[GridPosition.TopCenter], _game[GridPosition.TopRight] };
            yield return new[] { _game[GridPosition.CenterLeft], _game[GridPosition.Middle], _game[GridPosition.CenterRight] };
            yield return new[] { _game[GridPosition.BottomLeft], _game[GridPosition.BottomCenter], _game[GridPosition.BottomRight] };
        }
    }
    private IEnumerable<Player?[]> Columns
    {
        get
        {
            yield return new[] { _game[GridPosition.TopLeft], _game[GridPosition.CenterLeft], _game[GridPosition.BottomLeft] };
            yield return new[] { _game[GridPosition.TopCenter], _game[GridPosition.Middle], _game[GridPosition.BottomCenter] };
            yield return new[] { _game[GridPosition.TopRight], _game[GridPosition.CenterRight], _game[GridPosition.BottomRight] };
        }
    }
    private IEnumerable<Player?[]> Diagonals
    {
        get
        {
            yield return new[] { _game[GridPosition.TopLeft], _game[GridPosition.Middle], _game[GridPosition.BottomRight] };
            yield return new[] { _game[GridPosition.TopRight], _game[GridPosition.Middle], _game[GridPosition.BottomLeft] };
        }
    }

    public Player? HasWinner()
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
    public sealed record InProgress(History History, Player CurrentPlayer) : GameState();
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
    public abstract record GameOutcome
    {
        public sealed record Won(Player By) : GameOutcome();
        public sealed record Tie() : GameOutcome();
    }
}

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
