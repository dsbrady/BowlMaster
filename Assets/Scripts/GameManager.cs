using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/* TODO:
	* Add animator that displays the ball result (gutter, strike, spare, split, turkey, two in a row)
	* Add display for when the game ends
	* Create a start menu, where they enter how many players and what their names are, how many games to play, and how many frames per game (minimum 3)
	* Update the scrolling panel to automatically scroll to the current frame (at the end of the scroll if it's frame 3 or higher)
	* Update the scrolling panel to have the playerCard show up at all times
	* Adjust physics of rolling
	* add "power meter" for launching the ball instead of dragging it?
*/

public class GameManager : MonoBehaviour {

	private BallMaster ballMaster;
	private bool ballCanBeRolled = false;
	private Text ballText;
// TODO: remove currentStatus
//	private Hashtable currentStatus;
	//TODO: get number of players/games some other way
	private int framesPerGame = 3;
	private Text frameText;
	private int numberOfPlayers = 2;
	private int numberOfGames = 3;
	private PinCounter pinCounter;
	private PinSetter pinSetter;
	private List<string> playerNames = new List<string> {"Scott", "Derek"};
	private int previousPinsKnockedDown;
	private ScoreCard scoreCard;
	private ScorePanel scorePanel;
	private Text scoreText;
//
//	static GameManager instance = null;
//	
//	void Awake () {
//		// putting this in Awake to ensure there are not duplicate game managers
//		if (instance != null) {
//			Destroy(gameObject);
//		}
//		else {
//			instance = this;
//			GameObject.DontDestroyOnLoad(gameObject);
//		}
//	}
//
	// Use this for initialization
	void Start () {
		ballMaster = GameObject.FindObjectOfType<BallMaster>();
		pinCounter = GameObject.FindObjectOfType<PinCounter>();
		pinSetter = GameObject.FindObjectOfType<PinSetter>();
		scoreCard = new ScoreCard();
		scorePanel = GameObject.FindObjectOfType<ScorePanel>();
		StartSeries();
	}

	public void FinishTurn() {
		ScoreCard.Action action;

		int pinsKnockedDown = 10 - pinCounter.CountStandingPins() - previousPinsKnockedDown;

		action = scoreCard.Bowl(pinsKnockedDown);
//		this.currentStatus = scoreCard.GetCurrentStatus();

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
					Debug.Log("What to do when ending a series?");
				break;
		}
		scorePanel.UpdateScores(scoreCard);
	}

	public bool GetBallCanBeRolled() {
		return ballCanBeRolled;
	}

	public void SetNumberOfGames(int numberOfGames) {
		this.numberOfGames = numberOfGames;
	}

	public void SetNumberOfPlayers(int numberOfPlayers) {
		this.numberOfPlayers = numberOfPlayers;
	}

	public void StartSeries() {
		// Initialize the scoreCard object, which contains all of the information about the series (games, players, scores, etc.)
		this.scoreCard.Initialize(numberOfPlayers, numberOfGames, framesPerGame);
		this.scorePanel.Initialize(playerNames, framesPerGame, numberOfGames);

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
//		this.currentStatus = scoreCard.GetCurrentStatus();
		scorePanel.StartNewGame(scoreCard);
		StartFrame();
	}

}
