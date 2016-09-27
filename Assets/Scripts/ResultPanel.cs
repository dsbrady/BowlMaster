using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ResultPanel : MonoBehaviour {

	private GameObject contentBox;
	private RectTransform contentBoxRectTransform;
	private Text contentText;
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
			panelContent += "! The winner is " + results[0]["name"] + " with a final score of " + results[0]["score"];
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
		
		Debug.Log(panelContent);
	}
	
	public void ShowGutter() {
		// Clear any content from the panel
		ResetContent();
		
		contentText.text = "GUTTER BALL!!";
		
		// Enable the panel
		EnablePanel();
	}

	public void ShowSpare() {
		// Clear any content from the panel
		ResetContent();
		
		// TODO: show this one letter at a time!
		contentText.text = "SPARE!!";
		
		// Enable the panel
		EnablePanel();
	}
	
	public void ShowStrike() {
		// Clear any content from the panel
		ResetContent();
		
		// TODO: show this one letter at a time!
		contentText.text = "STRIKE!!";

		// Enable the panel
		EnablePanel();
	}

	private void DisablePanel() {
		contentText.enabled = false;
		gameObject.GetComponent<Image>().enabled = false;
	}	
	
	private void EnablePanel() {
		contentText.enabled = true;
		gameObject.GetComponent<Image>().enabled = true;
	}	

	private void ResetContent() {
		contentText.text = "";
	}

}
