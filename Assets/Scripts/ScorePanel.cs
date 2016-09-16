using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScorePanel : MonoBehaviour {

	public GameObject frame, lastFrame, playerCard;

	private int currentGame = 1;
	private float initialXPositionFrame = -330f,
		initialXPositionPlayerCard = -271f,
		initialYPositionFrame = 135f,
		initialYPositionPlayerCard = 200f;
	private GameObject frameClone, lastFrameClone, playerCardClone;
	private int framesPerGame;
	private GameObject scoreCardGameObject;

	// Use this for initialization
	void Start () {
		scoreCardGameObject = gameObject.transform.Find("ScoreCard").gameObject;
		if (!scoreCardGameObject) {
			throw new UnityException("Could not find scoreCard game object!!");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(List<string> playerNames, int framesPerGame) {
		float currentXPositionFrame = initialXPositionFrame, 
			currentXPositionPlayerCard = initialXPositionPlayerCard,
			currentYPositionFrame = initialYPositionFrame,
			currentYPositionPlayerCard = initialYPositionPlayerCard;
		GameObject frameNumberText, playerNameText;
		RectTransform frameRectTransform, playerCardRectTransform;
		Vector3 objectPosition;
		int playerNumber = 0;

		this.framesPerGame = framesPerGame;

		foreach (string playerName in playerNames) {
			// Create the player card
			playerNumber++;
			objectPosition = new Vector3(currentXPositionPlayerCard, currentYPositionPlayerCard, 0f);
			playerCardClone = Instantiate(playerCard, objectPosition, Quaternion.identity) as GameObject;
			playerCardClone.name = "Player " + playerNumber.ToString();
			playerCardClone.transform.SetParent(scoreCardGameObject.transform, false);
			playerCardRectTransform = (RectTransform)playerCardClone.transform;
			currentYPositionPlayerCard -= playerCardRectTransform.rect.height - 1f;
			playerNameText = playerCardClone.transform.Find("Name").gameObject;
			if (!playerNameText) {
				throw new UnityException("Can not find player name object!");
			}
			playerNameText.GetComponent<Text>().text = playerName;
			// Now start generating the frames for the first game
			for (int frameNumber = 1; frameNumber <= framesPerGame; ++ frameNumber) {
				objectPosition = new Vector3(currentXPositionFrame, currentYPositionFrame, 0f);
				if (frameNumber < framesPerGame) {
					frameClone = Instantiate(frame, objectPosition, Quaternion.identity) as GameObject;
				}
				else {
					frameRectTransform = (RectTransform)frame.transform;
					objectPosition.x += frameRectTransform.rect.width/4;
					frameClone = Instantiate(lastFrame, objectPosition, Quaternion.identity) as GameObject;
				}
				frameClone.name = "Player " + playerNumber.ToString() + " Frame " + frameNumber.ToString();
				frameClone.transform.SetParent(scoreCardGameObject.transform, false);
				frameNumberText = frameClone.transform.Find("Frame Label").gameObject;
				frameNumberText.GetComponent<Text>().text = frameNumber.ToString();

				frameRectTransform = (RectTransform)frameClone.transform;
				currentXPositionFrame += frameRectTransform.rect.width;
			}
			currentYPositionPlayerCard -= 100f;
			currentXPositionFrame = initialXPositionFrame;
			currentYPositionFrame -= 134f;
		}
	}

	public void StartNewGame() {

	}

	public void Update(ScoreCard scoreCard) {
		Ball ball;
		List<Ball> balls;
		int frameScore, gameScore, seriesScore;
		Game game;
		Frame frame;
		GameObject ballPanel, ballText, framePanel, frameScoreText, playerCard, playerScore;
		Frame[] frames;
		int playerNumber = 0;
		Player[] players = scoreCard.GetPlayers();
		int runningScore = 0;

		// We need to loop over each player, then over the game, then the frame, and then the balls
		foreach (Player player in players) {
			runningScore = 0;
			playerNumber++;
			game = player.GetGame(currentGame);
			playerScore = scoreCardGameObject.transform.Find("Player " + playerNumber.ToString()).gameObject.transform.Find("Total Score").gameObject;
			playerScore.GetComponent<Text>().text = Mathf.Clamp(game.GetScore(),0,300).ToString();

			frames = game.GetFrames();
			for (int frameNumber = 1; frameNumber <= frames.Length; ++frameNumber) {
				frame = game.GetFrame(frameNumber);
				if (frame != null) {
					framePanel = scoreCardGameObject.transform.Find("Player " + playerNumber.ToString() + " Frame " + frameNumber).gameObject;
					if (frame.GetScore() >= 0) {
						runningScore += frame.GetScore();
						frameScoreText = framePanel.transform.Find("Score").gameObject.transform.Find("Score Text").gameObject;
						if (runningScore >= 100) {
							frameScoreText.GetComponent<Text>().fontSize = 18;
						}
						frameScoreText.GetComponent<Text>().text = runningScore.ToString();
					}

					balls = frame.GetBalls();
					for (int ballNumber = 1; ballNumber <= balls.Count; ++ballNumber) {
						ball = frame.GetBall(ballNumber);
						if (ball != null) {
							ballPanel = framePanel.transform.Find("Ball " + ballNumber.ToString()).gameObject;
							ballText = ballPanel.transform.GetChild(0).gameObject;
							if (ball.GetPinsKnockedDown() >= 0) {
								// If it's a spare or a strike, mark accordingly
								if (ball.GetIsStrike()) {
									ballText.GetComponent<Text>().text = "X";
								}
								else if (ball.GetIsSpare()) {
									ballText.GetComponent<Text>().text = "/";
								}
								else {
									ballText.GetComponent<Text>().text = ball.GetPinsKnockedDown().ToString();
								}
							}

						}
						
					}
				}
			}
		}

	}
}
