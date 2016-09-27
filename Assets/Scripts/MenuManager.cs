using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour {

	const string NUMBER_OF_GAMES = "numberOfGames";
	const string NUMBER_OF_PLAYERS = "numberOfPlayers";
	const string FRAMES_PER_GAME = "framesPerGame";

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
		// TODO: get this another way
		SetFramesPerGame(10);
	
		SceneManager.LoadScene("Game");
	}
}
