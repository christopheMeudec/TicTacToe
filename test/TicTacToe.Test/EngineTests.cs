namespace TicTacToe.Test;

public class EngineTests
{
    [Fact]
    public void Check_DefaultGame()
    {
        var game = new TicTacToe.Engine.TicTacToe();
        Assert.IsAssignableFrom<TicTacToe.Engine.GameState.InProgress>(game.State);
        
        var inProgress = (TicTacToe.Engine.GameState.InProgress)game.State;
        Assert.Empty(inProgress.History);
        Assert.Equal(TicTacToe.Engine.Players.X, inProgress.CurrentPlayer);
    }

    [Fact]
    public void Check_PlayerX_Win_Game()
    {
        var game = new TicTacToe.Engine.TicTacToe();

        game.Play(Engine.GridPosition.BottomLeft);
        game.Play(Engine.GridPosition.BottomCenter);
        game.Play(Engine.GridPosition.Middle);
        game.Play(Engine.GridPosition.TopLeft);
        game.Play(Engine.GridPosition.TopRight);

        Assert.IsAssignableFrom<TicTacToe.Engine.GameState.Finished>(game.State);
        
        var finished = (TicTacToe.Engine.GameState.Finished)game.State;
        Assert.Equal(new[]{Engine.GridPosition.BottomLeft, Engine.GridPosition.BottomCenter, Engine.GridPosition.Middle, Engine.GridPosition.TopLeft, Engine.GridPosition.TopRight},finished.History);
        Assert.IsAssignableFrom<TicTacToe.Engine.GameOutcome.Won>(finished.Outcome);
        var outcome = (TicTacToe.Engine.GameOutcome.Won)finished.Outcome;
        Assert.Equal(TicTacToe.Engine.Players.X, outcome.By);
    }

    [Fact]
    public void Check_Play_Invalid_Move()
    {
        var game = new TicTacToe.Engine.TicTacToe();

        game.Play(Engine.GridPosition.BottomLeft);
        var validation = game.Play(Engine.GridPosition.BottomLeft);

        Assert.NotNull(validation);
        Assert.Equal(Engine.TicTacToe.MoveError.PositionAlreadyUsed, validation);
    }
}
