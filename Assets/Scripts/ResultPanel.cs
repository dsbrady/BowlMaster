using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ResultPanel : MonoBehaviour {

	public GameObject mainMenuButton;

	private GameObject contentBox;
	private RectTransform contentBoxRectTransform;
	private Text contentText;
	private bool allowPanelDisabling = true;
	private string panelContent;
	
	void Start() {
		contentBox = gameObject.transform.FindChild("ResultContentBox").gameObject;
		contentBoxRectTransform = contentBox.GetComponent<RectTransform>();
		contentText = contentBox.GetComponent<Text>();
	}
	
	public void TurnOffPanel() {
		Invoke("DisablePanel", 2);
	}
	
	public void EndSeries(ScoreCard scoreCard) {
		GameObject mainMenuClone = Instantiate(mainMenuButton, new Vector3(0f, -200f, 0f), Quaternion.identity) as GameObject;
		mainMenuClone.transform.SetParent(gameObject.transform,false);

		this.allowPanelDisabling = false;

		// Get the ranked results (player names and scores)
		List<Hashtable> results = scoreCard.GetFinalResults();

		// Clear any content from the panel
		ResetContent();
				
		panelContent = "Game Over";
		
		// If there's only one player, just display their name and final score
		if (results.Count == 1) {
			panelContent += ", " + results[0]["name"] + "!\n\nYour final score is: " + results[0]["score"];
		}
		else {
			// See if the first place and second place players tied
			if ((int)results[0]["score"] == (int)results[1]["score"]) {
				panelContent += "! It's a tie game!";
			}
			else {
				panelContent += "! The winner is " + results[0]["name"] + " with a final score of " + results[0]["score"];
			}
			panelContent += "\n";
			foreach (Hashtable playerResult in results) {
				panelContent += "\n" + playerResult["name"] + "\t\t" + playerResult["score"];
			}
		}

		// Update content box
// TODO: why isn't this working?
		contentBoxRectTransform.rect.Set(0f, 0f, 400f, 400f);

		contentText.text = panelContent;
		
		// Enable the panel
		EnablePanel();
	}

	public void ShowBallResult(ScoreCard.BallResult ballResult) {
		string content = "";

		switch (ballResult) {
			case ScoreCard.BallResult.Gutter:
				content = "GUTTER BALL!!";
				break;
			case ScoreCard.BallResult.Spare:
				content = "SPARE!!";
				break;
			case ScoreCard.BallResult.Split:
				content = "SPLIT!";
				break;
			case ScoreCard.BallResult.Strike:
				content = "STRIKE!!";
				break;
			case ScoreCard.BallResult.Turkey:
				content = "TURKEY!!\nGOBBLE, GOBBLE!!!";
				break;
			case ScoreCard.BallResult.TwoInRow:
				content = "TWO STRIKES IN A ROW!!";
				break;
		}

		// Clear any content from the panel
		ResetContent();

		contentText.text = content;
		
		// Enable the panel
		EnablePanel();
	}

	private void DisablePanel() {
		if (this.allowPanelDisabling) {
			contentText.enabled = false;
			gameObject.GetComponent<Image>().enabled = false;
		}
	}	
	
	private void EnablePanel() {
		contentText.enabled = true;
		gameObject.GetComponent<Image>().enabled = true;
	}	

	private void ResetContent() {
		contentText.text = "";
	}

}
