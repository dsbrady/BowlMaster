using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

public class ScoreCardTest {
	private ScoreCard scoreCard;
	private ScoreCard.Action endGame = ScoreCard.Action.EndGame;
	private ScoreCard.Action endSeries = ScoreCard.Action.EndSeries;
	private ScoreCard.Action endTurn = ScoreCard.Action.EndTurn;
	private ScoreCard.Action reset = ScoreCard.Action.Reset;
	private ScoreCard.Action tidy = ScoreCard.Action.Tidy;
	private ScoreCard.BallResult spare = ScoreCard.BallResult.Spare;
	private ScoreCard.BallResult strike = ScoreCard.BallResult.Strike;
	private ScoreCard.BallResult twoInRow = ScoreCard.BallResult.TwoInRow;
	private ScoreCard.BallResult turkey = ScoreCard.BallResult.Turkey;

	private int numberOfPlayers, numberOfGames, framesPerGame;
	private List<string> playerNames;

	[SetUp]
	public void Setup() {
		scoreCard = new ScoreCard();

		// Default to a standard single-player game
		numberOfPlayers = 1;
		numberOfGames = 1;
		framesPerGame = 10;

		playerNames = new List<string> {"Scott", "Derek", "Brent", "Amy"};
	}

	// Make sure game was initialized properly
	[Test]
	[Category("Initial")]
	public void T01_InitializeSeries() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);
		Assert.AreEqual(numberOfPlayers, scoreCard.GetPlayers().Length);
		Assert.AreEqual(numberOfGames, scoreCard.GetPlayer(1).GetGames().Length);
		Assert.AreEqual(framesPerGame, scoreCard.GetPlayer(1).GetGame(1).GetFrames().Length);
		// We add the balls dynamically, so should only be 1 ball right now
		Assert.AreEqual(1, scoreCard.GetPlayer(1).GetGame(1).GetFrame(1).GetBalls().Count);
	}

	// Can we get the current player's current frame
	[Test]
	[Category("Initial")]
	public void T02_GetCurrentFrame() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);
		Assert.IsInstanceOf<Hashtable>(scoreCard.GetCurrentPlayerFrame());
		Hashtable currentPlayerFrame = scoreCard.GetCurrentPlayerFrame();
		Assert.IsNotNull(currentPlayerFrame["playerName"]);
		Assert.IsNotNull(currentPlayerFrame["gameNumber"]);
		Assert.AreEqual(1,currentPlayerFrame["gameNumber"]);
		Assert.IsNotNull(currentPlayerFrame["frameNumber"]);
		Assert.AreEqual(1,currentPlayerFrame["frameNumber"]);
		Assert.IsNotNull(currentPlayerFrame["frame"]);
		Assert.IsInstanceOf<Frame>(currentPlayerFrame["frame"]);
	}

	// Make sure you can take a turn and get an action back
	[Test]
	[Category("Actions")]
	public void T03_Bowl() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);
		Assert.IsInstanceOf<ScoreCard.Action>(scoreCard.Bowl(3));
	}


	// It's the first ball and they don't throw a strike
	[Test]
	[Category("Actions")]
	public void T04_FirstBallNonStrikeTidies() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);
		Assert.AreEqual(tidy, scoreCard.Bowl(3));
	}

	// Strike in the non-last frame
	[Test]
	[Category("Actions")]
	public void T05_StrikeNotLastFrameEndsTurn() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);
		Assert.AreEqual(endTurn, scoreCard.Bowl(10));
	}

	// Second ball in the the non-last frame
	[Test]
	[Category("Actions")]
	public void T06_SecondBallNotLastFrameEndsTurn() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);
		scoreCard.Bowl(3);
		Assert.AreEqual(endTurn, scoreCard.Bowl(2));
	}

	// Ensure you can go to the next player when the turn ends
	[Test]
	[Category("Actions")]
	public void T07_EndTurnGoesToNextPlayer() {
		this.numberOfPlayers = 2;
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		Hashtable status;
		scoreCard.Bowl(1);
		scoreCard.Bowl(2);
		status = scoreCard.GetCurrentStatus();
		Assert.AreEqual(2, status["currentPlayerNumber"]);
	}

	// Can you go to the next frame after all players have bowled that frame?
	[Test]
	[Category("Actions")]
	public void T08_EndFrameGoesToNextFrame() {
		this.numberOfPlayers = 2;
		playerNames.Add("Helix Brady");
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		Hashtable status;
		// Player 1's turn
		scoreCard.Bowl(1);
		scoreCard.Bowl(2);
		// Player 2's turn
		scoreCard.Bowl(3);
		scoreCard.Bowl(4);
		status = scoreCard.GetCurrentStatus();
		Assert.AreEqual(2, status["currentFrameNumber"]);
	}

	// Can you go to the next game after all frames are done?
	[Test]
	[Category("Actions")]
	public void T09_EndGameGoesToNextGame() {
		this.numberOfGames = 2;
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		Hashtable status;
		// Bowl all of the frames in the game
		for (int i = 1; i <= 10; ++i) {
			scoreCard.Bowl(4);
			scoreCard.Bowl(5);
		}
		status = scoreCard.GetCurrentStatus();
		Assert.AreEqual(2, status["currentGameNumber"]);
	}

	// If it's the second ball in the last frame, and it's not a spare, and they haven't gotten a strike or a spare, and it's the last player, then end the game 
	// This is really the same as the last test, but it's good to spell it out
	[Test]
	[Category("Actions")]
	public void T10_TenthFrameNoMarkEndsGame() {
		this.numberOfGames = 2;
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		Hashtable status;
		// Bowl all of the frames in the game
		for (int i = 1; i <= 10; ++i) {
			scoreCard.Bowl(4);
			scoreCard.Bowl(5);
		}
		Assert.AreEqual(endGame,scoreCard.GetCurrentAction());
		status = scoreCard.GetCurrentStatus();
		Assert.AreEqual(2, status["currentGameNumber"]);
	}

	// If it's the second ball in the last frame, and it's not a spare, and they haven't gotten a strike or a spare, and it's NOT the last player, then end the turn
	[Test]
	[Category("Actions")]
	public void T11_TenthFrameFirstPlayerNoMarkEndsTurn() {
		this.numberOfPlayers = 2;
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		Hashtable status;
		// Bowl all of the frames in the game, up through the 9th frame
		for (int i = 1; i <= 18; ++i) {
			scoreCard.Bowl(4);
			scoreCard.Bowl(5);
		}
		// Let's make sure it's the 10th frame
		status = scoreCard.GetCurrentStatus();
		Assert.AreEqual(10, status["currentFrameNumber"]);
		Assert.AreEqual(1, status["currentPlayerNumber"]);

		// Bowl the 10th frame (no mark)
		scoreCard.Bowl(2);
		scoreCard.Bowl(6);
		Assert.AreEqual(endTurn,scoreCard.GetCurrentAction());
	}

	// Strike in the first ball of the last frame when they get to go again (reset)
	[Test]
	[Category("Actions")]
	public void T12_TenthFrameFirstBallStrikeResets() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		Hashtable status;
		// Bowl all of the frames in the game, up through the 9th frame
		for (int i = 1; i <= 9; ++i) {
			scoreCard.Bowl(4);
			scoreCard.Bowl(5);
		}
		// Let's make sure it's the 10th frame
		status = scoreCard.GetCurrentStatus();

		int playerNumber = (int)status["currentPlayerNumber"],
			gameNumber = (int)status["currentGameNumber"],
			frameNumber = (int)status["currentFrameNumber"];

		Assert.AreEqual(10, frameNumber);
		Assert.AreEqual(1, playerNumber);

		// Bowl the first ball of the 10th frame as a strike
		scoreCard.Bowl(10);

		Assert.IsTrue(scoreCard.GetPlayer(playerNumber).GetGame(gameNumber).GetFrame(frameNumber).GetBall(1).GetIsStrike());
		Assert.AreEqual(reset,scoreCard.GetCurrentAction());
	}

	// Two strikes in the first two balls of the last frame -- reset
	[Test]
	[Category("Actions")]
	public void T13_TenthFrameFirstTwoBallsStrikeResets() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		Hashtable status;
		// Bowl all of the frames in the game, up through the 9th frame
		for (int i = 1; i <= 9; ++i) {
			scoreCard.Bowl(4);
			scoreCard.Bowl(5);
		}
		// Let's make sure it's the 10th frame
		status = scoreCard.GetCurrentStatus();

		int playerNumber = (int)status["currentPlayerNumber"],
			gameNumber = (int)status["currentGameNumber"],
			frameNumber = (int)status["currentFrameNumber"];

		Assert.AreEqual(10, frameNumber);
		Assert.AreEqual(1, playerNumber);

		// Bowl the first two balls of the 10th frame as a strike
		scoreCard.Bowl(10);
		scoreCard.Bowl(10);

		Assert.IsTrue(scoreCard.GetPlayer(playerNumber).GetGame(gameNumber).GetFrame(frameNumber).GetBall(2).GetIsStrike());
		Assert.AreEqual(reset,scoreCard.GetCurrentAction());
	}

	// Spare in the last frame when they get to go again (reset)
	[Test]
	[Category("Actions")]
	public void T14_TenthFrameFirstTwoBallsSpareResets() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		Hashtable status;
		// Bowl all of the frames in the game, up through the 9th frame
		for (int i = 1; i <= 9; ++i) {
			scoreCard.Bowl(4);
			scoreCard.Bowl(5);
		}
		// Let's make sure it's the 10th frame
		status = scoreCard.GetCurrentStatus();

		int playerNumber = (int)status["currentPlayerNumber"],
			gameNumber = (int)status["currentGameNumber"],
			frameNumber = (int)status["currentFrameNumber"];

		Assert.AreEqual(10, frameNumber);
		Assert.AreEqual(1, playerNumber);

		// Bowl the first two balls of the 10th frame as a strike
		scoreCard.Bowl(3);
		scoreCard.Bowl(7);

		Assert.IsTrue(scoreCard.GetPlayer(playerNumber).GetGame(gameNumber).GetFrame(frameNumber).GetBall(2).GetIsSpare());
		Assert.AreEqual(reset,scoreCard.GetCurrentAction());
	}

	// If it's the third ball in the last frame and no more players, end the game
	[Test]
	[Category("Actions")]
	public void T15_TenthFrameThidBallLastPlayerEndsGame() {
		this.numberOfGames = 2;
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		Hashtable status;
		// Bowl all of the frames in the game, up through the 9th frame
		for (int i = 1; i <= 9; ++i) {
			scoreCard.Bowl(4);
			scoreCard.Bowl(5);
		}
		// Let's make sure it's the 10th frame
		status = scoreCard.GetCurrentStatus();

		int playerNumber = (int)status["currentPlayerNumber"],
			frameNumber = (int)status["currentFrameNumber"];

		Assert.AreEqual(10, frameNumber);
		Assert.AreEqual(1, playerNumber);

		// Bowl the first two balls of the 10th frame as a spare
		scoreCard.Bowl(3);
		scoreCard.Bowl(7);
		scoreCard.Bowl(4);

		Assert.AreEqual(endGame,scoreCard.GetCurrentAction());
	}

	// If it's the third ball in the last frame and there are more players, end the turn
	[Test]
	[Category("Actions")]
	public void T16_TenthFrameThidBallMorePlayersEndsTurn() {
		this.numberOfPlayers = 2;
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		Hashtable status;
		// Bowl all of the frames in the game, up through the 9th frame
		for (int i = 1; i <= 18; ++i) {
			scoreCard.Bowl(4);
			scoreCard.Bowl(5);
		}
		// Let's make sure it's the 10th frame
		status = scoreCard.GetCurrentStatus();

		int playerNumber = (int)status["currentPlayerNumber"],
			frameNumber = (int)status["currentFrameNumber"];

		Assert.AreEqual(10, frameNumber);
		Assert.AreEqual(1, playerNumber);

		// Bowl the a three-ball last frame for the first player
		scoreCard.Bowl(3);
		scoreCard.Bowl(7);
		scoreCard.Bowl(4);

		Assert.AreEqual(endTurn,scoreCard.GetCurrentAction());
	}

	// Does the series end when the last turn is taken?
	[Test]
	[Category("Actions")]
	public void T17_EndSeries() {
		this.numberOfGames = 2;
		this.numberOfPlayers = 2;
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		Hashtable status;
		// Bowl all of the frames in both games, up through the 9th frame of the second game
		for (int i = 1; i <= 38; ++i) {
			scoreCard.Bowl(4);
			scoreCard.Bowl(5);
		}
		// Let's make sure it's the 10th frame
		status = scoreCard.GetCurrentStatus();

		int playerNumber = (int)status["currentPlayerNumber"],
			gameNumber = (int)status["currentGameNumber"],
			frameNumber = (int)status["currentFrameNumber"];

		Assert.AreEqual(2, gameNumber);
		Assert.AreEqual(10, frameNumber);
		Assert.AreEqual(1, playerNumber);

		// Bowl each player's final frame as 3 balls.
		scoreCard.Bowl(3);
		scoreCard.Bowl(7);
		scoreCard.Bowl(4);

		scoreCard.Bowl(10);
		scoreCard.Bowl(10);
		scoreCard.Bowl(10);

		Assert.AreEqual(endSeries,scoreCard.GetCurrentAction());
	}

	// Is a gutter and 10 marked a spare?
	[Test]
	[Category("Scores")]
	public void T18_GutterTenIsSpare() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		scoreCard.Bowl(0);
		scoreCard.Bowl(10);
		Assert.IsTrue(scoreCard.GetPlayer(1).GetGame(1).GetFrame(1).GetBall(2).GetIsSpare());
	}

	// Are three strikes in a rwo calculated as 30?
	[Test]
	[Category("Scores")]
	public void T19_ThreeStrikesReturns30() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		scoreCard.Bowl(10);
		scoreCard.Bowl(10);
		scoreCard.Bowl(10);

		Assert.IsTrue(scoreCard.GetPlayer(1).GetGame(1).GetFrame(1).GetBall(1).GetIsStrike());
		Assert.IsTrue(scoreCard.GetPlayer(1).GetGame(1).GetFrame(2).GetBall(1).GetIsStrike());
		Assert.IsTrue(scoreCard.GetPlayer(1).GetGame(1).GetFrame(3).GetBall(1).GetIsStrike());
		Assert.AreEqual(30,scoreCard.GetPlayer(1).GetGame(1).GetFrame(1).GetScore());

	}

	// Are two strikes and 5 calculated as 25?
	[Test]
	[Category("Scores")]
	public void T20_TwoStrikesAnd5Returns25() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		scoreCard.Bowl(10);
		scoreCard.Bowl(10);
		scoreCard.Bowl(5);

		Assert.IsTrue(scoreCard.GetPlayer(1).GetGame(1).GetFrame(1).GetBall(1).GetIsStrike());
		Assert.IsTrue(scoreCard.GetPlayer(1).GetGame(1).GetFrame(2).GetBall(1).GetIsStrike());
		Assert.AreEqual(25,scoreCard.GetPlayer(1).GetGame(1).GetFrame(1).GetScore());

	}

	// Is one strike and a spare caclualted as 20?
	[Test]
	[Category("Scores")]
	public void T21_StrikeAndSpareReturns20() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		scoreCard.Bowl(10);
		scoreCard.Bowl(4);
		scoreCard.Bowl(6);

		Assert.IsTrue(scoreCard.GetPlayer(1).GetGame(1).GetFrame(1).GetBall(1).GetIsStrike());
		Assert.IsTrue(scoreCard.GetPlayer(1).GetGame(1).GetFrame(2).GetBall(2).GetIsSpare());
		Assert.AreEqual(20,scoreCard.GetPlayer(1).GetGame(1).GetFrame(1).GetScore());

	}

	// Is one strike and frame of 9 caclualted as 19?
	[Test]
	[Category("Scores")]
	public void T22_StrikeAndNineReturns19() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		scoreCard.Bowl(10);
		scoreCard.Bowl(4);
		scoreCard.Bowl(5);

		Assert.IsTrue(scoreCard.GetPlayer(1).GetGame(1).GetFrame(1).GetBall(1).GetIsStrike());
		Assert.AreEqual(19,scoreCard.GetPlayer(1).GetGame(1).GetFrame(1).GetScore());

	}

	// Is one spare and a strike calculated as 20?
	[Test]
	[Category("Scores")]
	public void T23_SpareAndStrikeReturns20() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		scoreCard.Bowl(4);
		scoreCard.Bowl(6);
		scoreCard.Bowl(10);

		Assert.IsTrue(scoreCard.GetPlayer(1).GetGame(1).GetFrame(1).GetBall(2).GetIsSpare());
		Assert.IsTrue(scoreCard.GetPlayer(1).GetGame(1).GetFrame(2).GetBall(1).GetIsStrike());
		Assert.AreEqual(20,scoreCard.GetPlayer(1).GetGame(1).GetFrame(1).GetScore());

	}

	// Is one spare and 8 calculated as 18?
	[Test]
	[Category("Scores")]
	public void T24_SpareAnd8Returns18() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		scoreCard.Bowl(4);
		scoreCard.Bowl(6);
		scoreCard.Bowl(8);

		Assert.IsTrue(scoreCard.GetPlayer(1).GetGame(1).GetFrame(1).GetBall(2).GetIsSpare());
		Assert.AreEqual(18,scoreCard.GetPlayer(1).GetGame(1).GetFrame(1).GetScore());

	}

	// Is an open frame of 7 calculated as 7?
	[Test]
	[Category("Scores")]
	public void T25_OpenFrameReturnsScore() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		scoreCard.Bowl(4);
		scoreCard.Bowl(3);

		Assert.AreEqual(7,scoreCard.GetPlayer(1).GetGame(1).GetFrame(1).GetScore());

	}

	// If it's the second ball of the last frame, and the last frame was a strike, and the first ball of this frame was 4 and the second ball was a 6, is last frame calculated as 20?
	[Test]
	[Category("Scores")]
	public void T26_SecondBallLastFrameLastBallStrike4And6Returns20() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		// Bowl all of the frames in the game, up through the 8th frame
		for (int i = 1; i <= 8; ++i) {
			scoreCard.Bowl(4);
			scoreCard.Bowl(5);
		}
		// 9th frame is a strike
		scoreCard.Bowl(10);

		// 10th frame is 4 and 6
		scoreCard.Bowl(4);
		scoreCard.Bowl(6);

		Assert.AreEqual(20,scoreCard.GetPlayer(1).GetGame(1).GetFrame(9).GetScore());
	}

	// If it's the second ball of the last frame, and the last frame was a strike, and the first ball of this frame was a strike and the second ball was a 6, is it calculated as 26?
	[Test]
	[Category("Scores")]
	public void T27_SecondBallLastFrameLastBallStrikeStrikeAnd6Returns26() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		// Bowl all of the frames in the game, up through the 8th frame
		for (int i = 1; i <= 8; ++i) {
			scoreCard.Bowl(4);
			scoreCard.Bowl(5);
		}
		// 9th frame is a strike
		scoreCard.Bowl(10);

		// 10th frame is strike and 6
		scoreCard.Bowl(10);
		scoreCard.Bowl(6);

		Assert.AreEqual(26,scoreCard.GetPlayer(1).GetGame(1).GetFrame(9).GetScore());
	}

	// If it's the third ball of the last frame, does it calculate corectly?
	[Test]
	[Category("Scores")]
	public void T28_ThirdBallLastFrameReturnsScore() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		// Bowl all of the frames in the game, up through the 9th frame
		for (int i = 1; i <= 9; ++i) {
			scoreCard.Bowl(4);
			scoreCard.Bowl(5);
		}

		// 10th frame is strike and 6 and 3
		scoreCard.Bowl(10);
		scoreCard.Bowl(6);
		scoreCard.Bowl(3);

		Assert.AreEqual(19,scoreCard.GetPlayer(1).GetGame(1).GetFrame(10).GetScore());
	}

	// If it's ball 1 and a strike, does the score come back as -1?
	[Test]
	[Category("Scores")]
	public void T29_StrikeNotScored() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		scoreCard.Bowl(10);

		Assert.AreEqual(-1,scoreCard.GetPlayer(1).GetGame(1).GetFrame(1).GetScore());
	}

	// If it's ball 2 and a spare, does the score come back as -1?
	[Test]
	[Category("Scores")]
	public void T30_SpareNotScored() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		scoreCard.Bowl(4);
		scoreCard.Bowl(6);

		Assert.AreEqual(-1,scoreCard.GetPlayer(1).GetGame(1).GetFrame(1).GetScore());
	}

	// Does a game of 300 get calculated correctly?
	[Test]
	[Category("Scores")]
	public void T31_300ScoredCorrectly() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		// Bowl a 300
		for (int i = 1; i <= 12; ++i) {
			scoreCard.Bowl(10);
		}

		Assert.AreEqual(300,scoreCard.GetPlayer(1).GetGame(1).GetScore());
	}

	// Does a game get calculated correctly?
	[Category("Scores")]
	[Test]
	public void T32_GameScoredCorrectly() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		// Bowl a game
		for (int i = 1; i <= 20; ++i) {
			scoreCard.Bowl(1);
		}

		Assert.AreEqual(20,scoreCard.GetPlayer(1).GetGame(1).GetScore());
	}

	// Does a two-player game get calculated correctly?
	[Test]
	[Category("Scores")]
	public void T33_TwoPlayerGameScoredCorrectly() {
		this.numberOfPlayers = 2;
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		// Bowl a game
		for (int i = 1; i <= 40; ++i) {
			scoreCard.Bowl(1);
		}

		Assert.AreEqual(20,scoreCard.GetPlayer(1).GetGame(1).GetScore());
		Assert.AreEqual(20,scoreCard.GetPlayer(2).GetGame(1).GetScore());
	}

	// Does a series get calculated correctly?
	[Test]
	[Category("Scores")]
	public void T34_SeriesScoredCorrectly() {
		this.numberOfGames = 3;
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		// Bowl all the games
		for (int i = 1; i <= 30; ++i) {
			scoreCard.Bowl(1);
			scoreCard.Bowl(2);
		}

		Assert.AreEqual(30,scoreCard.GetPlayer(1).GetGame(1).GetScore());
		Assert.AreEqual(30,scoreCard.GetPlayer(1).GetGame(2).GetScore());
		Assert.AreEqual(30,scoreCard.GetPlayer(1).GetGame(3).GetScore());
		Assert.AreEqual(90,scoreCard.GetPlayer(1).GetScore());
	}

	// Does a two-player series get calculated correctly?
	[Test]
	[Category("Scores")]
	public void T35_TwoPlayerSeriesScoredCorrectly() {
		this.numberOfGames = 3;
		this.numberOfPlayers = 2;
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		// Bowl all the games
		for (int i = 1; i <= 60; ++i) {
			scoreCard.Bowl(1);
			scoreCard.Bowl(2);
		}

		Assert.AreEqual(90,scoreCard.GetPlayer(1).GetScore());
		Assert.AreEqual(90,scoreCard.GetPlayer(2).GetScore());
	}

	// Does a strike report as a strike?
	[Test]
	[Category("Scores")]
	public void T36_StrikeReportsStrike() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		// Bowl a strike
		scoreCard.Bowl(10);

		Assert.AreEqual(strike, scoreCard.GetBallResult());
	}

	// Does a spare report as a spare?
	[Test]
	[Category("Scores")]
	public void T37_SpareReportsSpare() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		// Bowl a spare
		scoreCard.Bowl(3);
		scoreCard.Bowl(7);

		Assert.AreEqual(spare, scoreCard.GetBallResult());
	}

	// Do two strikes in a row report correctly?
	[Test]
	[Category("Scores")]
	public void T38_TwoStrikesInARow() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		// Bowl 2 strikes
		scoreCard.Bowl(10);
		scoreCard.Bowl(10);

		Assert.AreEqual(twoInRow, scoreCard.GetBallResult());
	}

	// Do three strikes in a row return turkey?
	[Test]
	[Category("Scores")]
	public void T39_Turkey() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		// Bowl 3 strikes
		scoreCard.Bowl(10);
		scoreCard.Bowl(10);
		scoreCard.Bowl(10);

		Assert.AreEqual(turkey, scoreCard.GetBallResult());
	}

	// First ball of last frame strike
	[Test]
	[Category("Scores")]
	public void T40_FirstBallLastFrameStrike() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		// Bowl first 9 frames as non-strikes
		for (int i = 1; i <= 18; ++i) {
			scoreCard.Bowl(1);
		}

		// Now bowl a strike
		scoreCard.Bowl(10);

		Assert.AreEqual(strike, scoreCard.GetBallResult());
	}

	// First ball of last frame two in a row
	[Test]
	[Category("Scores")]
	public void T41_FirstBallLastFrameTwoInRow() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		// Bowl first 8 frames as non-strikes
		for (int i = 1; i <= 16; ++i) {
			scoreCard.Bowl(1);
		}

		// Now bowl 2 strikes
		scoreCard.Bowl(10);
		scoreCard.Bowl(10);

		Assert.AreEqual(twoInRow, scoreCard.GetBallResult());
	}

	// First ball of last frame turkey 
	[Test]
	[Category("Scores")]
	public void T42_FirstBallLastFrameTurkey() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		// Bowl first 7 frames as non-strikes
		for (int i = 1; i <= 14; ++i) {
			scoreCard.Bowl(1);
		}

		// Now bowl 3 strikes
		scoreCard.Bowl(10);
		scoreCard.Bowl(10);
		scoreCard.Bowl(10);

		Assert.AreEqual(turkey, scoreCard.GetBallResult());
	}

	// Second ball of last frame two in a row
	[Test]
	[Category("Scores")]
	public void T43_SecondBallLastFrameTwoInRow() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		// Bowl first 9 frames as non-strikes
		for (int i = 1; i <= 18; ++i) {
			scoreCard.Bowl(1);
		}

		// Now bowl 2 strikes
		scoreCard.Bowl(10);
		scoreCard.Bowl(10);

		Assert.AreEqual(twoInRow, scoreCard.GetBallResult());
	}

	// Second ball of last frame turkey
	[Test]
	[Category("Scores")]
	public void T44_SecondBallLastFrameTurkey() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		// Bowl first 8 frames as non-strikes
		for (int i = 1; i <= 16; ++i) {
			scoreCard.Bowl(1);
		}

		// Now bowl 3 strikes
		scoreCard.Bowl(10);
		scoreCard.Bowl(10);
		scoreCard.Bowl(10);

		Assert.AreEqual(turkey, scoreCard.GetBallResult());
	}

	// Third ball of last frame turkey
	[Test]
	[Category("Scores")]
	public void T45_ThirdBallLastFrameTurkey() {
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		// Bowl first 9 frames as non-strikes
		for (int i = 1; i <= 18; ++i) {
			scoreCard.Bowl(1);
		}

		// Now bowl 3 strikes
		scoreCard.Bowl(10);
		scoreCard.Bowl(10);
		scoreCard.Bowl(10);

		Assert.AreEqual(turkey, scoreCard.GetBallResult());
	}

	[Test]
	[Category("Scores")]
	public void T46_GetFinalResults() {
		// Test to make sure the final results are in order
		this.numberOfGames = 3;
		this.numberOfPlayers = 3;
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		List<Hashtable> results = new List<Hashtable>();

		// Bowl everything except the last frame of the last game (first 2 games for each player: 20; 18 through frame 9; 58 total so far
		for (int i = 1; i <= 174; ++i) {
			scoreCard.Bowl(1);
		}

		// Now bowl last frame of last game for each player
		// 78 total
		scoreCard.Bowl(8);
		scoreCard.Bowl(2);
		scoreCard.Bowl(10);

		// 88 total
		scoreCard.Bowl(10);
		scoreCard.Bowl(10);
		scoreCard.Bowl(10);

		// 67 total
		scoreCard.Bowl(8);
		scoreCard.Bowl(1);

		// Generate expected results Hashtable
		results.Add(new Hashtable());
		results[0]["name"] = "Derek";
		results[0]["score"] = 88;

		results.Add(new Hashtable());
		results[1]["name"] = "Scott";
		results[1]["score"] = 78;

		results.Add(new Hashtable());
		results[2]["name"] = "Brent";
		results[2]["score"] = 67;

		Assert.AreEqual(results, scoreCard.GetFinalResults());
	}



// TODO:  No ability to check for these yet

	// Does a gutter return a gutter?

	// Does a split return a split?

	// http://slocums.homestead.com/gamescore.html
	[Test]
	[Category ("Verification")]
	public void TG02GoldenCopyA () {
		int[] rolls = { 10, 7,3, 9,0, 10, 0,8, 8,2, 0,6, 10, 10, 10,8,1};

		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		foreach (int roll in rolls) {
			scoreCard.Bowl(roll);
		}

		Assert.AreEqual (167, scoreCard.GetPlayer(1).GetGame(1).GetScore());
	}
	
	//http://guttertoglory.com/wp-content/uploads/2011/11/score-2011_11_28.png
	[Category ("Verification")]
	[Test]
	public void TG03GoldenCopyB1of3 () {
		int[] rolls = { 10, 9,1, 9,1, 9,1, 9,1, 7,0, 9,0, 10, 8,2, 8,2,10};
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		foreach (int roll in rolls) {
			scoreCard.Bowl(roll);
		}

		Assert.AreEqual (168, scoreCard.GetPlayer(1).GetGame(1).GetScore());
	}

	//http://guttertoglory.com/wp-content/uploads/2011/11/score-2011_11_28.png
	[Category ("Verification")]
	[Test]
	public void TG03GoldenCopyB2of3 () {
		int[] rolls = { 8,2, 8,1, 9,1, 7,1, 8,2, 9,1, 9,1, 10, 10, 7,1};
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		foreach (int roll in rolls) {
			scoreCard.Bowl(roll);
		}

		Assert.AreEqual (163, scoreCard.GetPlayer(1).GetGame(1).GetScore());
	}

	//http://guttertoglory.com/wp-content/uploads/2011/11/score-2011_11_28.png
	[Category ("Verification")]
	[Test]
	public void TG03GoldenCopyB3of3 () {
		int[] rolls = { 10, 10, 9,0, 10, 7,3, 10, 8,1, 6,3, 6,2, 9,1,10};
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		foreach (int roll in rolls) {
			scoreCard.Bowl(roll);
		}

		Assert.AreEqual (162, scoreCard.GetPlayer(1).GetGame(1).GetScore());
	}

	// http://brownswick.com/wp-content/uploads/2012/06/OpenBowlingScores-6-12-12.jpg
	[Category ("Verification")]
	[Test]
	public void TG03GoldenCopyC1of3 () {
		int[] rolls = { 7,2, 10, 10, 10, 10, 7,3, 10, 10, 9,1, 10,10,9};
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		foreach (int roll in rolls) {
			scoreCard.Bowl(roll);
		}

		Assert.AreEqual (234, scoreCard.GetPlayer(1).GetGame(1).GetScore());
	}

	// http://brownswick.com/wp-content/uploads/2012/06/OpenBowlingScores-6-12-12.jpg
	[Category ("Verification")]
	[Test]
	public void TG03GoldenCopyC2of3 () {
		int[] rolls = { 10, 10, 10, 10, 9,0, 10, 10, 10, 10, 10,9,1};
		scoreCard.Initialize(this.numberOfPlayers, this.numberOfGames, this.framesPerGame, this.playerNames);

		foreach (int roll in rolls) {
			scoreCard.Bowl(roll);
		}

		Assert.AreEqual (256, scoreCard.GetPlayer(1).GetGame(1).GetScore());
	}
}
