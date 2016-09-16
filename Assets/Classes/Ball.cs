public class Ball
{
	private bool isSpare = false;
	private bool isStrike = false;
	// TODO: refactor this to default to -1
	private int pinsKnockedDown = -1;

	public bool GetIsSpare() {
		return isSpare;
	}

	public bool GetIsStrike() {
		return isStrike;
	}

	public int GetPinsKnockedDown() {
		return pinsKnockedDown;
	}

	public void SetIsSpare(bool spareStatus) {
		isSpare = spareStatus;
	}

	public void SetIsStrike(bool strikeStatus) {
		isStrike = strikeStatus;
	}

	public void SetPinsKnockedDown(int pinCount) {
		pinsKnockedDown = pinCount;
	}
}

