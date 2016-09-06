using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour {

	private Ball ball;
	private bool ballCanBeRolled = false;
	private Text ballText;
	private int currentFrame;
	private int currentGame;
	private int currentPlayer;
	private int currentBall;
	// TODO: make this 10 when ready
	private int framesPerGame = 3;
	private Text frameText;
	private int numberOfPlayers;
	private int numberOfGames;
	private PinSetter pinSetter;
	private Animator pinSetterAnimator;
	private int previousPinsKnockedDown;
	private ScoreCard scoreCard;
	private Text scoreText;

	// Use this for initialization
	void Start () {
		ball = GameObject.FindObjectOfType<Ball>();
		ballText = GameObject.Find("Ball Text").GetComponent<Text>();
		frameText = GameObject.Find("Frame Text").GetComponent<Text>();
		pinSetter = GameObject.FindObjectOfType<PinSetter>();
		pinSetterAnimator = pinSetter.GetComponent<Animator>();
		scoreCard = GameObject.FindObjectOfType<ScoreCard>();
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
		// TODO: calculate score
		// TODO: special case for the 10th frame
		int pinsKnockedDown = 10 - pinSetter.CountStandingPins() - previousPinsKnockedDown;
		previousPinsKnockedDown = 10 - pinSetter.CountStandingPins();
/* TODO: remove
		scoreCard.UpdateScore(currentPlayer, currentGame, currentFrame, currentBall, pinsKnockedDown);
*/

		// If it's currently the first turn, check for a strike; if it's not, just go to the next turn
		if (currentBall == 1) {
			// TODO: check for a strike

			currentBall++;
			pinSetterAnimator.SetTrigger("tidy");
			ball.Reset();
			UpdateDisplay();
		}
		else {
			currentBall--;
			currentFrame++;
			currentPlayer++;
			if (currentPlayer > numberOfPlayers) {
				currentPlayer = 1;
			}
			StartFrame();
		}
	}

	public bool GetBallCanBeRolled() {
		return ballCanBeRolled;
	}

	public void UpdateBallRollableStatus(bool canBeRolled) {
		ballCanBeRolled = canBeRolled;
	}

	private void StartFrame() {
		previousPinsKnockedDown = 0;
		pinSetterAnimator.SetTrigger("reset");
		ball.Reset();
		UpdateDisplay();
	}

	private void StartGame() {
		currentPlayer = 1;
		currentBall = 1;
		currentFrame = 1;
		StartFrame();
	}

	private void StartSeries() {
		// Initialize the scoreCard object, which contains all of the information about the series (games, players, scores, etc.)
		scoreCard.Initialize(numberOfPlayers, numberOfGames, framesPerGame);
		currentGame = 1;
		StartGame();
	}

	private void UpdateDisplay() {
		frameText.text = currentFrame.ToString();
		ballText.text = currentBall.ToString();
		scoreText.text = "2";
	}
}
