public class Frame {
	private Ball[] balls;
	private int score = -1; // flag of -1 so that if the frame has a spare or a strike, the score doesn't show yet

	public Frame(int frameNumber, int framesPerGame) {
		if (frameNumber == (framesPerGame)) {
			balls = new Ball[3];
		}
		else {
			balls = new Ball[2];
		}

		// Create the first ball
		balls[0] = new Ball();
	}

	public Ball GetBall(int ballNumber) {
		return balls[ballNumber - 1];
	}

	public Ball[] GetBalls() {
		return balls;
	}

	public int GetScore() {
		return score;
	}

	public void SetNextBall(int ballNumber) {
		balls[ballNumber - 1] = new Ball();
	}

	public void SetScore(int frameScore) {
		score = frameScore;
	}

	public string ToString(int frameNumber) {
		string returnString = "\n\t\t\tFrame " + (frameNumber + 1) + ":";

		returnString += "\n\t\t\t\tScore: " + this.score;

		for (int ballCount = 0; ballCount < this.balls.Length; ++ ballCount) {
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
