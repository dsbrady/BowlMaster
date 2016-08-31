using UnityEngine;
using System.Collections;

public class ScoreKeeper : MonoBehaviour {

	private int[] gameScores;
	private int numberOfPlayers;
	private int[] seriesScores;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GetScores() {

	}

	public void StartGame(int gameNumber) {
		gameScores = new int[numberOfPlayers];
		for (int i = 0; i < numberOfPlayers; ++i) {
			gameScores[i] = 0;
		}
	}

	public void StartSeries(int players) {
		numberOfPlayers = players;

		seriesScores = new int[numberOfPlayers];

		for (int i = 0; i < numberOfPlayers; ++i) {
			seriesScores[i] = 0;
		}
	}

}
