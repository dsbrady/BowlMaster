using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	private BallMaster ballMaster;
	private bool ballCanBeRolled = false;
	private Text ballText;
	// TODO: do we need current Status?
	private Hashtable currentStatus;
	//TODO: get number of players/games some other way
	private int framesPerGame = 10;
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

	// Use this for initialization
	void Start () {
		ballMaster = GameObject.FindObjectOfType<BallMaster>();
		pinCounter = GameObject.FindObjectOfType<PinCounter>();
		pinSetter = GameObject.FindObjectOfType<PinSetter>();
		scoreCard = new ScoreCard();
		scorePanel = GameObject.FindObjectOfType<ScorePanel>();
		StartSeries();
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void FinishTurn() {
		ScoreCard.Action action;

		int pinsKnockedDown = 10 - pinCounter.CountStandingPins() - previousPinsKnockedDown;

		action = scoreCard.Bowl(pinsKnockedDown);
		this.currentStatus = scoreCard.GetCurrentStatus();

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
					throw new UnityException("What to do when ending a series?");
				break;
		}
		scorePanel.Update(scoreCard);
	}

	public bool GetBallCanBeRolled() {
		return ballCanBeRolled;
	}

	public void UpdateBallRollableStatus(bool canBeRolled) {
		ballCanBeRolled = canBeRolled;
	}

	private void StartFrame() {
		previousPinsKnockedDown = 0;
		pinSetter.Reset();
		ballMaster.Reset();
		scorePanel.Update(scoreCard);
	}

	private void StartGame() {
		this.currentStatus = scoreCard.GetCurrentStatus();
		StartFrame();
	}

	private void StartSeries() {
		// Initialize the scoreCard object, which contains all of the information about the series (games, players, scores, etc.)
		scoreCard.Initialize(numberOfPlayers, numberOfGames, framesPerGame);
		scorePanel.Initialize(playerNames, framesPerGame);

		StartGame();
	}
}
