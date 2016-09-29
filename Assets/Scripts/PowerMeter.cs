using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerMeter : MonoBehaviour {

	private Animator animator;
	private Slider powerMeterSlider;

	// Use this for initialization
	void Start () {
		animator = gameObject.GetComponent<Animator>();
		powerMeterSlider = GameObject.Find("PowerMeter").GetComponent<Slider>();
	}

	public bool IsRunning() {
		return animator.GetBool("PowerMeterEnabled");
	}

	public void Pause() {
		animator.speed = 0f;
	}

	public void Reset() {
		// Make sure the animator has been initialized
		if (animator) {
			animator.SetBool("PowerMeterEnabled", false);
			animator.speed = 1f;
		}
		if (powerMeterSlider) {
			powerMeterSlider.value = 0f;
		}
	}

	public void StartMeter() {
		animator.SetBool("PowerMeterEnabled", true);
	}
}
