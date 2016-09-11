using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PinSetter : MonoBehaviour {

	public GameObject pinSet;
	public Text standingPinCount;

	private bool ballOutOfPlay = false;
	private GameManager gameManager;
	private int lastStandingCount = -1;
	private float lastChangeTime;
	private Pin[] pins;
	private GameObject pinSetClone;
	private float raisePinHeight = 40f;

	// Use this for initialization
	void Start () {
		gameManager = GameObject.FindObjectOfType<GameManager>();
		pins = GameObject.FindObjectsOfType<Pin>();
		lastChangeTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (ballOutOfPlay) {
			standingPinCount.color = Color.red;
			UpdateStandingCount();
		}
	}

	public int CountStandingPins() {
		int standingPinCount = 0;

		foreach(Pin pin in pins) {
			if (pin) {
				if (!pin.hasFallen && pin.IsStanding()) {
					standingPinCount++;
				}
				else {
					pin.hasFallen = true;
				}
			}
		}

		return standingPinCount;
	}

	public void LowerPins() {
		Vector3 pinTranslation = new Vector3(0f, -raisePinHeight, 0f);
		RaiseOrLowerPins(pinTranslation);
		gameManager.UpdateBallRollableStatus(true);
	}

	public void RaisePins() {
		Vector3 pinTranslation = new Vector3(0f, raisePinHeight, 0f);

		UpdateBallRollableStatus(0);
		RaiseOrLowerPins(pinTranslation);
	}

	public void RefillPins() {
		if (pinSetClone) {
			Destroy(pinSetClone);
		}
		Vector3 initialPosition = new Vector3(0f, raisePinHeight, 1829f);
		pinSetClone = Instantiate(pinSet, initialPosition, Quaternion.identity) as GameObject;

		// Now we're going to "raise" the pins by 0, so we can get them settled without going through some weird code kerfluffle
		Vector3 pinTranslation = new Vector3(0f, 0f, 0f);
		pins = GameObject.FindObjectsOfType<Pin>();
		RaiseOrLowerPins(pinTranslation);
	}

	public void SetBallOutOfPlay(bool isOutOfPlay) {
		this.ballOutOfPlay = isOutOfPlay;
	}

	private void PinsHaveSettled() {
		gameManager.FinishTurn();
		lastStandingCount = -1;
		ballOutOfPlay = false;
		standingPinCount.color = Color.green;
	}

	private void RaiseOrLowerPins(Vector3 pinTranslation) {
		foreach(Pin pin in pins) {
			if (pin && !pin.hasFallen && pin.IsStanding()) {
				pin.transform.Translate(pinTranslation, Space.World);
				pin.Settle((pinTranslation.y < 0f));
			}
		}
	}

	private void UpdateBallRollableStatus(int canBeRolled) {
		// Using an integer, because apparently animation events can't pass in boolean arguments
		gameManager.UpdateBallRollableStatus(canBeRolled == 1);
	}

	private void UpdateStandingCount() {
		int currentStandingCount = CountStandingPins();
		float pinSettlingThreshold = 3f;

		if (currentStandingCount != lastStandingCount) {
			// If the standing count has changed, update the lastStandingCount and the lastChangeTime
			lastStandingCount = currentStandingCount;
			lastChangeTime = Time.time;
			standingPinCount.text = currentStandingCount.ToString();
		}
		else if ((Time.time - lastChangeTime) > pinSettlingThreshold) {
			// Otherwise, if it's been 3 seconds since it last changed, move on to PinsHaveSettled()
			PinsHaveSettled();
		}

		return;
	}
}
