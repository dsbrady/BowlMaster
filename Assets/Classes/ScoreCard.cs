using UnityEngine;
using System.Collections;

public class ScoreCard : MonoBehaviour {
	public class Player {
		public class Game {
			public class Frame {
				public class Ball {
					private bool isSpare = false;
					private bool isStrike = false;
					private int pinsKnockedDown = 0;

					// TODO: can we refactor these using enums?
					public bool GetIsSpare() {
						return isSpare;
					}

					public bool GetIsStrike() {
						return isStrike;
					}

					public int GetPinsKnockedDown() {
						return pinsKnockedDown;
					}

					public void SetIsSpare(bool spareStatus) {
						isSpare = spareStatus;
					}

					public void SetIsStrike(bool strikeStatus) {
						isStrike = strikeStatus;
					}

					public void SetPinsKnockedDown(int pinCount) {
						pinsKnockedDown = pinCount;
					}
				}
				private Ball[] balls;
				private int score = -1; // flag of -1 so that if the frame has a spare or a strike, the score doesn't show yet

				public Frame(int frameNumber, int framesPerGame) {
					if (frameNumber == (framesPerGame)) {
						balls = new Ball[3];
					}
					else {
						balls = new Ball[2];
					}

					// Create the first ball
					balls[0] = new Ball();
				}

				public Ball GetBall(int ballNumber) {
					return balls[ballNumber - 1];
				}

				public Ball[] GetBalls() {
					return balls;
				}

				public int GetScore() {
					return score;
				}

				public void SetNextBall(int ballNumber) {
					balls[ballNumber - 1] = new Ball();
				}

				public void SetScore(int frameScore) {
					score = frameScore;
				}

				public string ToString(int frameNumber) {
					string returnString = "\n\t\t\tFrame " + (frameNumber + 1) + ":";

					returnString += "\n\t\t\t\tScore: " + this.score;

					for (int ballCount = 0; ballCount < this.balls.Length; ++ ballCount) {
						if (balls[ballCount] is Ball) {
							returnString += "\n\t\t\t\tBall: " + (ballCount + 1) + ": " + this.balls[ballCount].GetPinsKnockedDown();
							if (balls[ballCount].GetIsStrike()) {
								returnString += " STRIKE!!";
							}
							else if (balls[ballCount].GetIsSpare()) {
								returnString += " Spare!!";
							}

						}
					}

					return returnString;
				}
			}

			private int score = -1;
			private Frame[] frames;

			public Game(int framesPerGame) {
				frames = new Frame[framesPerGame];

				// Create the first frame
				frames[0] = new Frame(0, framesPerGame);
			}

			public Frame[] GetFrames() {
				return frames;
			}

			public Frame GetFrame(int frameNumber) {
				if (frameNumber > frames.Length) {
					throw new UnityException("Invalid frame number");
				}

				return frames[frameNumber - 1];
			}

			public int GetScore() {
				return score;
			}

			public void SetNextFrame(int frameNumber, int framesPerGame) {
				frames[frameNumber - 1] = new Frame(frameNumber, framesPerGame);
			}

			public void SetScore(int gameScore) {
				score = gameScore;
			}

			public string ToString(int gameNumber) {
				string returnString = "\n\t\tGame " + (gameNumber + 1) + ": ";;
				Frame frame;

				returnString += "\n\t\t\tScore: " + this.score;
				for (int frameCount = 0; frameCount < this.frames.Length; ++frameCount) {
					frame = this.frames[frameCount];
					if (frame is Frame) {
						returnString += frame.ToString(frameCount);
					}
				}

				return returnString;
			}
		}

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

	public enum Action {Tidy, Reset, EndTurn, EndGame, EndSeries, None};

	private Action currentAction = Action.None;

	private int currentGameNumber, currentPlayerNumber, currentFrameNumber, currentBallNumber, numberOfGames, numberOfPlayers, framesPerGame;
	private Player[] players;

	public Action Bowl(int pinsKnockedDown) {
		// Make sure the number of pins is valid
		if (!IsValidBowl(pinsKnockedDown)) {
			throw new UnityException("invalid number of pins knocked down: " + pinsKnockedDown);
		}

		this.currentAction = Action.None;

		// Get the current player's frame
		Player.Game.Frame playerFrame = GetCurrentPlayerFrame()["frame"] as Player.Game.Frame;

		// Set the pins knocked down in this turn
		GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).GetBall(this.currentBallNumber).SetPinsKnockedDown(pinsKnockedDown);

		// If it's the first roll and they didn't knock down 10 pins, tidy up
		if (currentBallNumber == 1 && pinsKnockedDown < 10) {
			this.currentAction = Action.Tidy;
		}
		// If it's a strike in the non-last frame, end the turn
		else if (currentFrameNumber < this.framesPerGame && currentBallNumber == 1 && pinsKnockedDown == 10) {
			this.currentAction = Action.EndTurn;
			GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).GetBall(this.currentBallNumber).SetIsStrike(true);
		}

		// If it's the second ball in the non-last frame, end the turn
		else if (currentBallNumber == 2 && currentFrameNumber < this.framesPerGame) {
			this.currentAction = Action.EndTurn;
			if (playerFrame.GetBall(1).GetPinsKnockedDown() + playerFrame.GetBall(2).GetPinsKnockedDown() == 10) {
				GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).GetBall(this.currentBallNumber).SetIsSpare(true);
			}
		}


		/* ********************** Last Frame ************************ */
		else if (this.currentFrameNumber == this.framesPerGame) {
			// Third ball, so just end the turn
			if (this.currentBallNumber == 3) {
				this.currentAction = Action.EndTurn;
			}
			// Second ball, look to see what we want to do
			else if (this.currentBallNumber == 2) {
				// First ball wasn't a strike, and this ball isn't a strike or a spare --> end the turn
				Player.Game.Frame.Ball lastBall = GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).GetBall(1);
				if (!lastBall.GetIsStrike() && (lastBall.GetPinsKnockedDown() + pinsKnockedDown) != 10) {
					this.currentAction = Action.EndTurn;
				}
				// First ball was a strike and so is the second one --> reset
				else if (lastBall.GetIsStrike() && pinsKnockedDown == 10) {
					GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).GetBall(this.currentBallNumber).SetIsStrike(true);
					this.currentAction = Action.Reset;
				}
				// First ball was a strike and the second one is not --> Tidy
				else if (lastBall.GetIsStrike() && pinsKnockedDown < 10) {
					this.currentAction = Action.Tidy;
				}
				// Second ball is a spare
				else if (!lastBall.GetIsStrike() && (lastBall.GetPinsKnockedDown() + pinsKnockedDown) == 10) {
					GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).GetBall(this.currentBallNumber).SetIsSpare(true);
					this.currentAction = Action.Reset;
				}
			}
			// First ball is a strike
			else if (pinsKnockedDown == 10) {
				GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).GetBall(this.currentBallNumber).SetIsStrike(true);
				this.currentAction = Action.Reset;
			}
		}

		// Calculate scores
		CalculateBallScore();
		CalculateGameScore();
		CalculateSeriesScore();

		// End the bowl (one ball thrown)
		if (this.currentAction != Action.None) {
			// Increment the current values by ending the turn
			this.currentAction = EndBowl();

			return this.currentAction;
		}

		throw new UnityException("No valid action found to return!");
	}

	public Action GetCurrentAction() {
		return this.currentAction;
	}

	public Hashtable GetCurrentPlayerFrame() {
		Hashtable playerFrame = new Hashtable();
		Player currentPlayer = GetPlayer(currentPlayerNumber);
		Player.Game.Frame currentFrame = currentPlayer.GetGame(currentGameNumber).GetFrame(currentFrameNumber);

		playerFrame["playerName"] = currentPlayer.GetName();
		playerFrame["playerNumber"] = currentPlayerNumber;
		playerFrame["gameNumber"] = currentGameNumber;
		playerFrame["frameNumber"] = currentFrameNumber;
		playerFrame["frame"] = currentFrame;

		return playerFrame;
	}

	public Hashtable GetCurrentStatus() {
		Hashtable status = new Hashtable();

		status["currentPlayerNumber"] = this.currentPlayerNumber;
		status["currentGameNumber"] = this.currentGameNumber;
		status["currentFrameNumber"] = this.currentFrameNumber;
		status["currentBallNumber"] = this.currentBallNumber;

		return status;
	}

	public Player GetPlayer(int playerNumber) {
		if (playerNumber > players.Length) {
			throw new UnityException("Invalid player number");
		}

		return players[playerNumber - 1];
	}

	public Player[] GetPlayers() {
		return players;
	}

	public void Initialize(int numberOfPlayers, int numberOfGames, int framesPerGame) {
		this.numberOfPlayers = numberOfPlayers;
		this.numberOfGames = numberOfGames;
		this.framesPerGame = framesPerGame;

		players = new Player[numberOfPlayers];

		for (int i = 0; i < numberOfPlayers; ++i) {
			players[i] =  new Player(numberOfGames, framesPerGame);
		}

		currentGameNumber = 1;
		currentPlayerNumber = 1;
		currentFrameNumber = 1;
		currentBallNumber = 1;
	}

	public override string ToString() {
		string returnString = "Score Card:";
		Player player;

		for (int playerCount = 0; playerCount < players.Length; ++playerCount) {
			player = players[playerCount];
			returnString += player.ToString(playerCount);

		}

		return returnString;
	}

	private void CalculateBallScore() {
		Player.Game.Frame thisFrame = GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber);
		Player.Game.Frame lastFrame = null;
		Player.Game.Frame nextToLastFrame = null;
		Player.Game.Frame.Ball thisBall = GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).GetBall(this.currentBallNumber);
		Player.Game.Frame.Ball lastBall = null;
		int pinsKnockedDown = thisBall.GetPinsKnockedDown();

		if (this.currentFrameNumber >= 2) {
			lastFrame = GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber - 1);
		}
		if (this.currentFrameNumber >= 3) {
			nextToLastFrame = GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber - 2);
		}

		// If it's the first ball, look for spares/strikes in previous frames
		if (this.currentBallNumber == 1 && this.currentFrameNumber >= 2) {
			// If the last frame was a spare, calculate this frame - 1
			if (lastFrame.GetBall(2) is Player.Game.Frame.Ball && lastFrame.GetBall(2).GetIsSpare()) {
				lastFrame.SetScore(10 + pinsKnockedDown);
			}
			// If the last two frames were strikes (and it's the third frame or higher), calculate this frame - 2
			else if (this.currentFrameNumber >= 3 && lastFrame.GetBall(1).GetIsStrike() && nextToLastFrame.GetBall(1).GetIsStrike()) {
				nextToLastFrame.SetScore(20 + pinsKnockedDown);
			}
		}

		// Now if it's the second ball,
		if (this.currentBallNumber == 2) {
			lastBall = thisFrame.GetBall(1);
			// If the previous frame was a strike, calculate this frame - 1
			if (this.currentFrameNumber >= 2 && lastFrame.GetBall(1).GetIsStrike()) {
				lastFrame.SetScore(10 + lastBall.GetPinsKnockedDown() + pinsKnockedDown);
			}

			// If this ball wasn't a spare and it's not the last frame, calculate this frame
			if (!thisFrame.GetBall(2).GetIsSpare() && this.currentFrameNumber != this.framesPerGame) {
				thisFrame.SetScore(lastBall.GetPinsKnockedDown() + pinsKnockedDown);
			}
			// If it is the last frame, and the first ball wasn't a strike and this one isn't a spare, calculate this frame
			else if (this.currentFrameNumber == this.framesPerGame && !lastBall.GetIsStrike() && !thisBall.GetIsSpare()) {
				thisFrame.SetScore(lastBall.GetPinsKnockedDown() + pinsKnockedDown);
			}
		}

		// If it's the third ball of the last frame, calculate this frame
		if (this.currentBallNumber == 3) {
			Player.Game.Frame.Ball nextToLastBall = thisFrame.GetBall(1);
			lastBall = thisFrame.GetBall(2);
			thisFrame.SetScore(nextToLastBall.GetPinsKnockedDown() + lastBall.GetPinsKnockedDown() + pinsKnockedDown);
		}
	}

	private void CalculateGameScore() {
		// Go through each frame of this player's current game and set the score
		Player.Game.Frame[] frames = GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrames();
		int runningScore = 0;

		foreach (Player.Game.Frame frame in frames) {
			if (frame != null && frame.GetScore() != -1) {
				runningScore += frame.GetScore();
			}
		}

		GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).SetScore(runningScore);
	}

	private void CalculateSeriesScore() {
		// Go through each game of this player's series and set the score
		Player.Game[] games = GetPlayer(this.currentPlayerNumber).GetGames();
		int runningScore = 0;

		foreach (Player.Game game in games) {
			if (game != null && game.GetScore() != -1) {
				runningScore += game.GetScore();
			}
		}

		GetPlayer(this.currentPlayerNumber).SetScore(runningScore);

	}

	private Action EndBowl() {
		switch (this.currentAction) {
			case Action.Tidy:
				// If it's a tidy, all we do is increment the ball number and add a ball
				this.currentBallNumber++;
				GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).SetNextBall(this.currentBallNumber);

				break;
			case Action.EndTurn:
				this.currentBallNumber = 1;
				// First, see if there are more players to take turns this frame
				if (this.currentPlayerNumber < this.numberOfPlayers) {
					// If it's not, just go to the next player
					this.currentPlayerNumber++;
				}
				else {
					this.currentPlayerNumber = 1;
					// The frame is over, now we see if there's another frame
					if (this.currentFrameNumber < this.framesPerGame) {
						// It's not, so go to the next frame, creating them for each player
						this.currentFrameNumber++;
						for (int i = 1; i <= this.numberOfPlayers; ++i) {
							GetPlayer(i).GetGame(this.currentGameNumber).SetNextFrame(this.currentFrameNumber, this.framesPerGame);
						}
					}
					else {
						this.currentFrameNumber = 1;
						// There aren't any more frames, so check to see if there are any more games to play
						if (this.currentGameNumber < this.numberOfGames) {
							// There are, so go to the next game
							this.currentGameNumber++;
							for (int i = 1; i <= this.numberOfPlayers; ++i) {
								GetPlayer(i).SetNextGame(this.currentGameNumber, this.framesPerGame);
							}
							this.currentAction = Action.EndGame;
						}
						else {
							// The series is over, so we want to set the action to be the series is over
							this.currentAction = Action.EndSeries;
						}
					}
				}
				break;
			case Action.Reset:
				// If it's a reset, all we do is increment the ball number and add a ball
				this.currentBallNumber++;
				GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).SetNextBall(this.currentBallNumber);
				break;
			default:
				throw new UnityException("No valid action in EndTurn(): " + this.currentAction.ToString());
		}

		return this.currentAction;
	}

	private bool IsValidBowl(int pinsKnockedDown) {
		// This is invalid no matter what ball it is
		if (pinsKnockedDown < 0 || pinsKnockedDown > 10) {
			return false;
		}

		// TODO: make sure that it's not the first ball, the number of pins is valid -- watching out for the 10th frame

		return true;
	}
}
