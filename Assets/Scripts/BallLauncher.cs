using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(BallMaster))]

public class BallLauncher : MonoBehaviour {

	private BallMaster ballMaster;
	private GameManager gameManager;
	private float horizontalSpeed = 0f;
	private Vector3 launchVelocity;
	private float maxSpeed = 1500f;
	private PowerMeter powerMeter;
	private Slider powerMeterSlider;

	// Use this for initialization
	void Start () {
		ballMaster = GetComponent<BallMaster>();
		gameManager = GameObject.FindObjectOfType<GameManager>();
		powerMeter = GameObject.FindObjectOfType<PowerMeter>();
		powerMeterSlider = GameObject.Find("PowerMeter").GetComponent<Slider>();
	}

	public void ClickMouse() {
		// Only do this if the ball can be rolled
		if (ballMaster.GetStatus() == BallMaster.Status.Idle && gameManager.GetBallCanBeRolled()) {
			// If the power meter isn't running, start it; otherwise, pause the meter and launch the ball
			if (!powerMeter.IsRunning()) {
				powerMeter.StartMeter();
			}
			else {
				powerMeter.Pause();
				launchVelocity = new Vector3(horizontalSpeed, 0f, (maxSpeed * powerMeterSlider.value) );
				ballMaster.Roll(launchVelocity);
			}
		}
	}
}
