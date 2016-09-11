using UnityEngine;

public class Player {
	private Game[] games;
	private string name;
	private int score = -1;

	public Player(int numberOfGames, int framesPerGame) {
		games = new Game[numberOfGames];

		// TODO: get the player's name
		name = "Bob Smith";

		// Create the first game
		games[0] = new Game(framesPerGame);
	}

	public Game GetGame(int gameNumber) {
		if (gameNumber > games.Length) {
			throw new UnityException("Invalid game number");
		}

		return games[gameNumber - 1];
	}

	public Game[] GetGames() {
		return games;
	}

	public string GetName() {
		return name;
	}

	public int GetScore() {
		return score;
	}

	public void SetNextGame(int gameNumber, int framesPerGame) {
		games[gameNumber - 1] = new Game(framesPerGame);
	}

	public void SetScore(int seriesScore) {
		score = seriesScore;
	}

	public string ToString (int playerNumber)
	{
		string returnString = "\n\tPlayer " + (playerNumber + 1) + ":";
		Game game;

		returnString += "\n\t\tSeries Score: " + this.score;
		for (int gameCount = 0; gameCount < this.games.Length; ++gameCount) {
			game = this.games[gameCount];
			if (game is Game) {
				returnString += game.ToString(gameCount);
			}
		}

		return returnString;
	}

}
