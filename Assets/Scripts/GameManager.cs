using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/* TODO:
	* Update start menu to take player names (maybe after hitting start game) and how many frames per game (3-10)
	* Update the scrolling panel to automatically scroll to the current frame (at the end of the scroll if it's frame 3 or higher)
	* Update the scrolling panel to have the playerCard show up at all times
	* Adjust physics of rolling
	* add "hook meter" (-1 to 1)
	* add labels for meters
	* Add difficulty setting (affects animation speed of power meter)
*/

public class GameManager : MonoBehaviour {

	const string NUMBER_OF_GAMES = "numberOfGames";
	const string NUMBER_OF_PLAYERS = "numberOfPlayers";
	const string FRAMES_PER_GAME = "framesPerGame";
	
	private BallMaster ballMaster;
	private bool ballCanBeRolled = false;
	private Text ballText;
	private int framesPerGame = 10;
	private Text frameText;
	private int numberOfPlayers = 1;
	private int numberOfGames = 1;
	private PinCounter pinCounter;
	private PinSetter pinSetter;
	private List<string> playerNames = new List<string> {"Scott", "Derek", "Brent", "Amy"};
	private int previousPinsKnockedDown;
	private ResultPanel resultPanel;
	private ScoreCard scoreCard;
	private ScorePanel scorePanel;
	private Text scoreText;

	// Use this for initialization
	void Start () {
		ballMaster = GameObject.FindObjectOfType<BallMaster>();
		pinCounter = GameObject.FindObjectOfType<PinCounter>();
		pinSetter = GameObject.FindObjectOfType<PinSetter>();
		resultPanel = GameObject.FindObjectOfType<ResultPanel>();
		scoreCard = new ScoreCard();
		scoreCard.SetResultPanel(resultPanel);
		scorePanel = GameObject.FindObjectOfType<ScorePanel>();
		StartSeries();
	}

	public void FinishTurn() {
		ScoreCard.Action action;

		int pinsKnockedDown = 10 - pinCounter.CountStandingPins() - previousPinsKnockedDown;

		action = scoreCard.Bowl(pinsKnockedDown);
		switch (action) {
			case ScoreCard.Action.Tidy:
					previousPinsKnockedDown = pinsKnockedDown;
					pinSetter.Tidy();
					ballMaster.Reset();
				break;
			case ScoreCard.Action.Reset:
					previousPinsKnockedDown = 0;
					pinSetter.Reset();
					ballMaster.Reset();
				break;
			case ScoreCard.Action.EndTurn:
					StartFrame();
					ballMaster.Reset();
				break;
			case ScoreCard.Action.EndGame:
					StartGame();
					ballMaster.Reset();
				break;
			case ScoreCard.Action.EndSeries:
					// Disable the game starting ?
					ballMaster.Reset();
			
					// Get the winner's name, and winning score -- if it's a one player game, don't report that they won, just their total
					resultPanel.EndSeries(scoreCard);
				break;
		}
		scorePanel.UpdateScores(scoreCard);
	}

	public bool GetBallCanBeRolled() {
		return ballCanBeRolled;
	}

	public void ReturnToMainMenu() {
		SceneManager.LoadScene(0);
	}

	public void SetNumberOfGames(int numberOfGames) {
		this.numberOfGames = numberOfGames;
	}

	public void SetNumberOfPlayers(int numberOfPlayers) {
		this.numberOfPlayers = numberOfPlayers;
	}

	public void StartSeries() {
		// Pull the values from the player prefs
		SetNumberOfPlayers(PlayerPrefs.GetInt(NUMBER_OF_PLAYERS));
		SetNumberOfGames(PlayerPrefs.GetInt(NUMBER_OF_GAMES));

		// Initialize the scoreCard object, which contains all of the information about the series (games, players, scores, etc.)
		this.scoreCard.Initialize(numberOfPlayers, numberOfGames, framesPerGame, playerNames);
		this.scorePanel.Initialize(numberOfPlayers, numberOfGames, framesPerGame, playerNames);
		
		StartGame();
	}

	public void UpdateBallRollableStatus(bool canBeRolled) {
		ballCanBeRolled = canBeRolled;
	}

	private void StartFrame() {
		previousPinsKnockedDown = 0;
		pinSetter.Reset();
		ballMaster.Reset();
		scorePanel.UpdateScores(scoreCard);
	}

	private void StartGame() {
		scorePanel.StartNewGame(scoreCard);
		StartFrame();
	}

}
