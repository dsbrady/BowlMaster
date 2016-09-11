using UnityEngine;

public class Game {
	private int score = -1;
	private Frame[] frames;

	public Game(int framesPerGame) {
		frames = new Frame[framesPerGame];

		// Create the first frame
		frames[0] = new Frame(0, framesPerGame);
	}

	public Frame[] GetFrames() {
		return frames;
	}

	public Frame GetFrame(int frameNumber) {
		if (frameNumber > frames.Length) {
			throw new UnityException("Invalid frame number");
		}

		return frames[frameNumber - 1];
	}

	public int GetScore() {
		return score;
	}

	public void SetNextFrame(int frameNumber, int framesPerGame) {
		frames[frameNumber - 1] = new Frame(frameNumber, framesPerGame);
	}

	public void SetScore(int gameScore) {
		score = gameScore;
	}

	public string ToString(int gameNumber) {
		string returnString = "\n\t\tGame " + (gameNumber + 1) + ": ";;
		Frame frame;

		returnString += "\n\t\t\tScore: " + this.score;
		for (int frameCount = 0; frameCount < this.frames.Length; ++frameCount) {
			frame = this.frames[frameCount];
			if (frame is Frame) {
				returnString += frame.ToString(frameCount);
			}
		}

		return returnString;
	}
}
