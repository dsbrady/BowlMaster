using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PinCounter : MonoBehaviour {

	public Text standingPinCount;

	private BallMaster ballMaster;
	private GameManager gameManager;
	private float lastChangeTime;
	private int lastStandingCount = -1;
	private Pin[] pins;

	// Use this for initialization
	void Start () {
		ballMaster = GameObject.FindObjectOfType<BallMaster>();
		gameManager = GameObject.FindObjectOfType<GameManager>();
		lastChangeTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (ballMaster.GetStatus() == BallMaster.Status.OutOfPlay) {
			UpdateStandingCount();
		}
	}

	void OnTriggerExit(Collider collider) {
		// Make sure it's the ball that's leaving
		if (collider.GetComponent<BallMaster>()) {
			// We want to update the ball's status
			ballMaster.SetStatus(BallMaster.Status.OutOfPlay);
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

	public void SetPins(Pin[] newPins) {
		pins = newPins;
	}

	private void PinsHaveSettled() {
		gameManager.FinishTurn();
		lastStandingCount = -1;
	}

	private void UpdateStandingCount() {
		int currentStandingCount = CountStandingPins();
		float pinSettlingThreshold = 3f;

		if (currentStandingCount != lastStandingCount) {
			// If the standing count has changed, update the lastStandingCount and the lastChangeTime
			lastStandingCount = currentStandingCount;
			lastChangeTime = Time.time;
		}
		else if ((Time.time - lastChangeTime) > pinSettlingThreshold) {
			// Otherwise, if it's been 3 seconds since it last changed, move on to PinsHaveSettled()
			PinsHaveSettled();
		}

		return;
	}
}
