public class Ball
{
	private bool isSpare = false;
	private bool isStrike = false;
	private int pinsKnockedDown = 0;

	// TODO: can we refactor these using enums?
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

