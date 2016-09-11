using UnityEngine;
using System.Collections;

public class PinSetter : MonoBehaviour {

	public GameObject pinSet;

	private GameManager gameManager;
	private Pin[] pins;
	private PinCounter pinCounter;
	private GameObject pinSetClone;
	private Animator pinSetterAnimator;
	private float raisePinHeight = 40f;

	// Use this for initialization
	void Awake () {
		pinSetterAnimator = GetComponent<Animator>();
	}

	void Start () {
		gameManager = GameObject.FindObjectOfType<GameManager>();
		pins = GameObject.FindObjectsOfType<Pin>();
		pinCounter = GameObject.FindObjectOfType<PinCounter>();
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
		pinCounter.SetPins(pins);
		RaiseOrLowerPins(pinTranslation);
	}

	public void Reset() {
		pinSetterAnimator.SetTrigger("reset");
	}

	public void Tidy() {
		pinSetterAnimator.SetTrigger("tidy");
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
}
