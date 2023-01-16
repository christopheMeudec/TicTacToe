using TicTacToe.Engine;

var game = new TicTacToe.Engine.TicTacToe();

while(game.State is GameState.InProgress i)   {
            game.Play(GridPosition.Middle);

}