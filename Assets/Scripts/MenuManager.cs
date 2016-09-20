using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour {

	const string NUMBER_OF_GAMES = "numberOfGames";
	const string NUMBER_OF_PLAYERS = "numberOfPlayers";
	const string FRAMES_PER_GAME = "framesPerGame";

//	void OnGUI() {
//		GUI.Button (new Rect (10,140,180,20), "This is a button", "toggle");
//		Debug.Log (GameObject.Find ("Buttons").transform.Find("2 Players"));
//	}
//
	public void SetFramesPerGame(int framesPerGame) {
		PlayerPrefs.SetInt(FRAMES_PER_GAME, framesPerGame);
	}

	public void SetNumberOfGames(int numberOfGames) {
		PlayerPrefs.SetInt(NUMBER_OF_GAMES, numberOfGames);
	}

	public void SetNumberOfPlayers(int numberOfPlayers) {
		PlayerPrefs.SetInt(NUMBER_OF_PLAYERS, numberOfPlayers);
	}

	public void StartSeries() {
	// Not getting here?
		// Just set at 10 for now
		SetFramesPerGame(10);
	
		Debug.Log(SceneManager.GetSceneByName("Game"));
		SceneManager.LoadScene("Game");
	}
}
