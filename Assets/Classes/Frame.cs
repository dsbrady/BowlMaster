using System.Collections.Generic;

public class Frame {
	private List<Ball> balls = new List<Ball>();
	private Ball[] ballsOld;
	private int score = -1; // flag of -1 so that if the frame has a spare or a strike, the score doesn't show yet

	public Frame(int frameNumber, int framesPerGame) {
		// Create the first ball
		balls.Add(new Ball());
	}

	public Ball GetBall(int ballNumber) {
		return balls[ballNumber - 1];
	}

	public List<Ball> GetBalls() {
		return balls;
	}

	public Ball GetLastBall() {
		return balls[balls.Count - 1];
	}

	public int GetScore() {
		return score;
	}

	public void SetNextBall() {
		balls.Add(new Ball());
	}

	public void SetScore(int frameScore) {
		score = frameScore;
	}

	public string ToString(int frameNumber) {
		string returnString = "\n\t\t\tFrame " + (frameNumber + 1) + ":";

		returnString += "\n\t\t\t\tScore: " + this.score;

		for (int ballCount = 0; ballCount < this.balls.Count; ++ ballCount) {
			if (balls[ballCount] is Ball) {
				returnString += "\n\t\t\t\tBall: " + (ballCount + 1) + ": " + this.balls[ballCount].GetPinsKnockedDown();
				if (balls[ballCount].GetIsStrike()) {
					returnString += " STRIKE!!";
				}
				else if (balls[ballCount].GetIsSpare()) {
					returnString += " Spare!!";
				}

			}
		}

		return returnString;
	}

}
