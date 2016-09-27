using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class ScorePanel : MonoBehaviour {

	public GameObject frame, lastFrame, playerCard;

	private float initialXPositionFrame = -490f,
		initialXPositionPlayerCard = -355f,
		initialYPositionFrame = 240f,
		initialYPositionPlayerCard = 340f;
	private GameObject frameClone, lastFrameClone, playerCardClone;
	private int framesPerGame;
	private int numberOfGames;
	private GameObject scoreCardGameObject;

	// Use this for initialization
	void Awake () {
		scoreCardGameObject = gameObject.transform.Find("ScoreCard").gameObject;
		if (!scoreCardGameObject) {
			throw new UnityException("Could not find scoreCard game object!!");
		}
	}
	
	public void Initialize(int numberOfPlayers, int numberOfGames, int framesPerGame, List<string> playerNames) {
		float currentXPositionFrame = initialXPositionFrame, 
			currentXPositionPlayerCard = initialXPositionPlayerCard,
			currentYPositionFrame = initialYPositionFrame,
			currentYPositionPlayerCard = initialYPositionPlayerCard;
		GameObject frameNumberText, playerNameText;
		RectTransform frameRectTransform, playerCardRectTransform;
		Vector3 objectPosition;
		string playerName;

		this.framesPerGame = framesPerGame;
		this.numberOfGames = numberOfGames;

		for (int playerNumber = 1; playerNumber <= numberOfPlayers; ++playerNumber) {
			// Create the player card
			playerName = playerNames[playerNumber - 1];
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
					objectPosition.x += frameRectTransform.rect.width/3;
					frameClone = Instantiate(lastFrame, objectPosition, Quaternion.identity) as GameObject;
				}
				frameClone.name = "Player " + playerNumber.ToString() + " Frame " + frameNumber.ToString();
				frameClone.transform.SetParent(scoreCardGameObject.transform, false);
				frameNumberText = frameClone.transform.Find("Frame Label").gameObject;
				frameNumberText.GetComponent<Text>().text = frameNumber.ToString();

				frameRectTransform = (RectTransform)frameClone.transform;
				currentXPositionFrame += frameRectTransform.rect.width;
			}
			currentYPositionPlayerCard -= 166f;
			currentXPositionFrame = initialXPositionFrame;
			currentYPositionFrame -= 215f;
		}
	}

	public void StartNewGame(ScoreCard scoreCard) {
		GameObject ballText, framePanel, frameScoreText;
		GameObject gameNumberObject = transform.Find("ScoreCard").gameObject.transform.Find("Game Number").gameObject;
		Text gameNumberText = gameNumberObject.GetComponent<Text>();
		Hashtable gameStatus = scoreCard.GetCurrentStatus();
		Player[] players = scoreCard.GetPlayers();

		gameNumberText.text = "Game " + gameStatus["currentGameNumber"];

		// Clear out the score card
		for (int playerNumber = 1; playerNumber <= players.Length; ++playerNumber) {
			for (int frameNumber = 1; frameNumber <= this.framesPerGame; ++frameNumber) {
				framePanel = scoreCardGameObject.transform.Find("Player " + playerNumber.ToString() + " Frame " + frameNumber).gameObject;
				frameScoreText = framePanel.transform.Find("Score").gameObject.transform.Find("Score Text").gameObject;
				frameScoreText.GetComponent<Text>().fontSize = 24;
				frameScoreText.GetComponent<Text>().text = "";
				foreach (Transform child in framePanel.transform) {
					if (child.tag == "BallPanel") {
						ballText = child.GetChild(0).gameObject;
						ballText.GetComponent<Text>().text = "";
					}
		}

			}
		}
		UpdateScores(scoreCard);
	}

	public void UpdateScores(ScoreCard scoreCard) {
		Ball ball;
		List<Ball> balls;
		int currentGame;
		Game game;
		Frame frame;
		GameObject ballPanel, ballText, framePanel, frameScoreText, playerScore;
		Frame[] frames;
		int playerNumber = 0;
		Player[] players = scoreCard.GetPlayers();
		int runningScore = 0;
		Hashtable gameStatus = scoreCard.GetCurrentStatus();
		currentGame = (int)gameStatus["currentGameNumber"];

		// We need to loop over each player, then over the game, then the frame, and then the balls
		foreach (Player player in players) {
			runningScore = 0;
			playerNumber++;
			game = player.GetGame(currentGame);
			playerScore = scoreCardGameObject.transform.Find("Player " + playerNumber.ToString()).gameObject.transform.Find("Total Score").gameObject;
			playerScore.GetComponent<Text>().text = Mathf.Clamp(player.GetScore(),0,300 * this.numberOfGames).ToString();

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
