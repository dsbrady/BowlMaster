﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour {

	private BallMaster ballMaster;
	private bool ballCanBeRolled = false;
	private Text ballText;
	private Hashtable currentStatus;
// TODO: make this 10 when ready
	private int framesPerGame = 3;
	private Text frameText;
	private int numberOfPlayers;
	private int numberOfGames;
	private PinCounter pinCounter;
	private PinSetter pinSetter;
	private int previousPinsKnockedDown;
	private ScoreCard scoreCard;
	private Text scoreText;

	// Use this for initialization
	void Start () {
		ballMaster = GameObject.FindObjectOfType<BallMaster>();
		ballText = GameObject.Find("Ball Text").GetComponent<Text>();
		frameText = GameObject.Find("Frame Text").GetComponent<Text>();
		pinCounter = GameObject.FindObjectOfType<PinCounter>();
		pinSetter = GameObject.FindObjectOfType<PinSetter>();
		scoreCard = new ScoreCard();
		scoreText = GameObject.Find("Score Text").GetComponent<Text>();
		//TODO: get number of players/games some other way
		numberOfPlayers = 1;
		numberOfGames = 1;
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
		UpdateDisplay();
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
		UpdateDisplay();
	}

	private void StartGame() {
		this.currentStatus = scoreCard.GetCurrentStatus();
		StartFrame();
	}

	private void StartSeries() {
		// Initialize the scoreCard object, which contains all of the information about the series (games, players, scores, etc.)
		scoreCard.Initialize(numberOfPlayers, numberOfGames, framesPerGame);
		StartGame();
	}

	private void UpdateDisplay() {
		frameText.text = this.currentStatus["currentFrameNumber"].ToString();
		ballText.text = this.currentStatus["currentBallNumber"].ToString();
		// TODO: fix this
		scoreText.text = scoreCard.GetPlayer(1).GetGame(1).GetScore().ToString();
	}
}
