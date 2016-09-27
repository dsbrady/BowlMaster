using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreCard {
	public enum Action {EndGame, EndSeries, EndTurn, Reset, Tidy, None};
	public enum BallResult {Gutter, Spare, Split, Strike, Turkey, TwoInRow, None};

	private Action currentAction = Action.None;
	private BallResult ballResult = BallResult.None;

	private int currentGameNumber, currentPlayerNumber, currentFrameNumber, currentBallNumber, numberOfGames, numberOfPlayers, framesPerGame;
	private Player[] players;
	private ResultPanel resultPanel;
	
	public Action Bowl(int pinsKnockedDown) {
		// Make sure the number of pins is valid
		if (!IsValidBowl(pinsKnockedDown)) {
			throw new UnityException("invalid number of pins knocked down: " + pinsKnockedDown);
		}

		this.currentAction = Action.None;

		// Get the current player's frame
		Frame playerFrame = GetCurrentPlayerFrame()["frame"] as Frame;

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
				Ball lastBall = GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).GetBall(1);

				// If the last ball was a strike or a spare, and we knocked down 10 pins, set this as a strike
				if ((lastBall.GetIsSpare() || lastBall.GetIsStrike()) && pinsKnockedDown == 10) {
					GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).GetBall(this.currentBallNumber).SetIsStrike(true);
				}
				// Else if we knocked down 10 pins, set this as a spare
				else if (pinsKnockedDown == 10) {
					GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).GetBall(this.currentBallNumber).SetIsSpare(true);
				}

				this.currentAction = Action.EndTurn;
			}
			// Second ball, look to see what we want to do
			else if (this.currentBallNumber == 2) {
				// First ball wasn't a strike, and this ball isn't a strike or a spare --> end the turn
				Ball lastBall = GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).GetBall(1);
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

		// Determine the ball result (strike, spare, turkey, etc.)
		this.ballResult = DetermineBallResult();
		ShowBallResult();

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

	public BallResult GetBallResult() {
		return this.ballResult;
	}

	public Hashtable GetCurrentPlayerFrame() {
		Hashtable playerFrame = new Hashtable();
		Player currentPlayer = GetPlayer(this.currentPlayerNumber);
		Frame currentFrame = currentPlayer.GetGame(currentGameNumber).GetFrame(currentFrameNumber);

		playerFrame["playerName"] = currentPlayer.GetName();
		playerFrame["playerNumber"] = this.currentPlayerNumber;
		playerFrame["gameNumber"] = this.currentGameNumber;
		playerFrame["frameNumber"] = this.currentFrameNumber;
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

	public List<Hashtable> GetFinalResults() {
		Player player;
		bool playerInserted;
		Player[] players = GetPlayers();
		List<Hashtable> results = new List<Hashtable>();

		// Loop over the players, then loop over the results; if player's score is higher than current result player, insert it at that index; then break out of the results loop
		for (int playerNumber = 0; playerNumber < players.Length; ++playerNumber) {
			player = players[playerNumber];
			// If this is the first playerNumber, just insert the player
			if (playerNumber == 0) {
				results.Add(new Hashtable());
				results[0]["name"] = player.GetName();
				results[0]["score"] = player.GetScore();
			}
			else {
				playerInserted = false;
				for (int resultNumber = 0; resultNumber < results.Count; ++resultNumber) {
					if (player.GetScore() > (int)results[resultNumber]["score"]) {
						results.Insert(resultNumber, new Hashtable());
						results[resultNumber]["name"] = player.GetName();
						results[resultNumber]["score"] = player.GetScore();
						playerInserted = true;
						break;
					}
				}
				// If the player was inserted, continue to the next interation
				if (playerInserted) {
					continue;
				}
				// If we get this point, the player hasn't been inserted yet, so it's the lowest score so far; just add it
				results.Add(new Hashtable());
				results[results.Count - 1]["name"] = player.GetName();
				results[results.Count - 1]["score"] = player.GetScore();
			}

		}

		return results;
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

	public void Initialize(int numberOfPlayers, int numberOfGames, int framesPerGame, List<string> playerNames) {
		// Make sure the player names length is at least as large as the number of players
		if (playerNames.Count < numberOfPlayers) {
			throw new UnityException("There aren't enough names for the number of players!");
		}

		this.numberOfPlayers = numberOfPlayers;
		this.numberOfGames = numberOfGames;
		this.framesPerGame = framesPerGame;

		players = new Player[numberOfPlayers];

		for (int i = 0; i < numberOfPlayers; ++i) {
			players[i] =  new Player(numberOfGames, framesPerGame);
			players[i].SetName(playerNames[i]);
		}

		currentGameNumber = 1;
		currentPlayerNumber = 1;
		currentFrameNumber = 1;
		currentBallNumber = 1;
	}

	public void SetResultPanel(ResultPanel resultPanel) {
		this.resultPanel = resultPanel;
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
		Frame thisFrame = GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber);
		Frame lastFrame = null;
		Frame nextToLastFrame = null;
		Ball thisBall = GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).GetBall(this.currentBallNumber);
		Ball lastBall = null;
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
			lastBall = lastFrame.GetLastBall();
			if (lastBall.GetIsSpare()) {
				lastFrame.SetScore(10 + pinsKnockedDown);
			}
			// If the last two frames were strikes (and it's the third frame or higher), calculate this frame - 2
			else if (this.currentFrameNumber >= 3 && lastBall.GetIsStrike() && nextToLastFrame.GetLastBall().GetIsStrike()) {
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
			Ball nextToLastBall = thisFrame.GetBall(1);
			lastBall = thisFrame.GetBall(2);

			thisFrame.SetScore(nextToLastBall.GetPinsKnockedDown() + lastBall.GetPinsKnockedDown() + pinsKnockedDown);
		}
	}

	private void CalculateGameScore() {
		// Go through each frame of this player's current game and set the score
		Frame[] frames = GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrames();
		int runningScore = 0;

		foreach (Frame frame in frames) {
			if (frame != null && frame.GetScore() != -1) {
				runningScore += frame.GetScore();
			}
		}

		GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).SetScore(runningScore);
	}

	private void CalculateSeriesScore() {
		// Go through each game of this player's series and set the score
		Game[] games = GetPlayer(this.currentPlayerNumber).GetGames();
		int runningScore = 0;

		foreach (Game game in games) {
			if (game != null && game.GetScore() != -1) {
				runningScore += game.GetScore();
			}
		}

		GetPlayer(this.currentPlayerNumber).SetScore(runningScore);

	}

	private BallResult DetermineBallResult() {
		BallResult result = BallResult.None;
		Ball ball = GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).GetBall(this.currentBallNumber);
		Ball previousBall, previousPreviousBall ;

		// First, just see if the current ball is a spare
		if (ball.GetIsSpare()) {
			result = BallResult.Spare;
		}
		// Now, see if it's a strike
		else if (ball.GetIsStrike()) {
			// Handle the last frame balls 2 or 3 first
			if (this.currentFrameNumber == this.framesPerGame && this.currentBallNumber > 1) {
				previousBall = GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).GetBall(this.currentBallNumber - 1);
				if (previousBall.GetIsStrike()) {
					// We need to check for a turkey, but keeping in mind that if this is ball 2, we have to look back at the previous frame, but if it's ball 3, we can just look at the first ball of this frame
					if (this.currentBallNumber == 2) {
						previousPreviousBall = GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber - 1).GetBall(1);
					}
					else {
						previousPreviousBall = GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).GetBall(1);
					}

					if (previousPreviousBall.GetIsStrike()) {
						result = BallResult.Turkey;
					}
					else {
						result = BallResult.TwoInRow;
					}
				}
			}
			else if (this.currentFrameNumber >= 2) {
				// Get the first ball of the previous frame
				previousBall = GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber - 1).GetBall(1);
				if (previousBall.GetIsStrike()) {
					// We need to check the frame before the previous frame if we're in the third frame or higher
					if (this.currentFrameNumber >= 3) {
						previousPreviousBall = GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber - 2).GetBall(1);
						if (previousPreviousBall.GetIsStrike()) {
							result = BallResult.Turkey;
						}
						else {
							result = BallResult.TwoInRow;
						}
					}
					else {
						result = BallResult.TwoInRow;
					}
				}
				else {
					result = BallResult.Strike;
				}
			}
			else {
				result = BallResult.Strike;
			}
		}
		// For a gutter, we probably also need a flag of some sort that it entered the gutter area, but for now, we'll assume 0 pins is a gutter
		else if (ball.GetPinsKnockedDown() == 0) {
			result = BallResult.Gutter;
		
		}
		else {
			// TODO: check for split -- will require making sure it's the first ball and seeing what pins are still standing
		}

		return result;
	}

	private Action EndBowl() {
		switch (this.currentAction) {
			case Action.Tidy:
				// If it's a tidy, all we do is increment the ball number and add a ball
				this.currentBallNumber++;
				GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).SetNextBall();

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
				GetPlayer(this.currentPlayerNumber).GetGame(this.currentGameNumber).GetFrame(this.currentFrameNumber).SetNextBall();
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
	
	private void ShowBallResult() {
		bool panelShown = false;
		switch (this.ballResult) {
			case BallResult.Gutter:
				this.resultPanel.ShowGutter();
				panelShown = true;
				break;
			case BallResult.Spare:
				this.resultPanel.ShowSpare();
				panelShown = true;
				break;
			case BallResult.Strike:
				this.resultPanel.ShowStrike();
				panelShown = true;
				break;
		}

		// After 2 seconds, turn off the panel after a deleay
		if (panelShown) {
			this.resultPanel.TurnOffPanel();
		}
	}
}
