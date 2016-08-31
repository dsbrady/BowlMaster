using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour {

	private Ball ball;
	private Text ballText;
	private int currentFrame;
	private int currentGame;
	private int currentPlayer;
	private int currentTurnBall;
	private Text frameText;
	private int numberOfPlayers;
	private PinSetter pinSetter;
	private Animator pinSetterAnimator;
	private ScoreKeeper scoreKeeper;
	private Text scoreText;

	// Use this for initialization
	void Start () {
		ball = GameObject.FindObjectOfType<Ball>();
		ballText = GameObject.Find("Ball Text").GetComponent<Text>();
		frameText = GameObject.Find("Frame Text").GetComponent<Text>();
		pinSetter = GameObject.FindObjectOfType<PinSetter>();
		pinSetterAnimator = pinSetter.GetComponent<Animator>();
		scoreKeeper = GameObject.FindObjectOfType<ScoreKeeper>();
		scoreText = GameObject.Find("Score Text").GetComponent<Text>();
		//TODO: get number of players some other way
		numberOfPlayers = 1;
		StartSeries();
	}

	// TODO: don't allow launching the ball while tidying/resetting

	// Update is called once per frame
	void Update () {
	
	}

	public void FinishTurn() {
		// TODO: calculate score
		// TODO: special case for the 10th frame

		// If it's currently the first turn, check for a strike; if it's not, just go to the next turn
		if (currentTurnBall == 1) {
			// TODO: check for a strike

			currentTurnBall++;
			pinSetterAnimator.SetTrigger("tidy");
			ball.Reset();
			UpdateDisplay();
		}
		else {
			currentTurnBall--;
			currentFrame++;
			currentPlayer++;
			if (currentPlayer >= numberOfPlayers) {
				currentPlayer = 0;
			}
			StartFrame();
		}
	}

	private void StartFrame() {
		pinSetterAnimator.SetTrigger("reset");
		ball.Reset();
		UpdateDisplay();
	}

	private void StartGame() {
		scoreKeeper.StartGame(currentGame);

		currentPlayer = 0;
		currentTurnBall = 1;
		currentFrame = 1;
		StartFrame();
	}

	private void StartSeries() {
		scoreKeeper.StartSeries(numberOfPlayers);
		currentGame = 1;
		StartGame();
	}

	private void UpdateDisplay() {
		frameText.text = currentFrame.ToString();
		ballText.text = currentTurnBall.ToString();
		scoreText.text = "2";
	}
}
