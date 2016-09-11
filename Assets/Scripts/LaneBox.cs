using UnityEngine;
using System.Collections;

public class LaneBox : MonoBehaviour {

	private BallMaster ballMaster;

	void Start() {
		ballMaster = GameObject.FindObjectOfType<BallMaster>();
	}

	void OnTriggerExit(Collider collider) {
		// Make sure it's the ball that's leaving
		if (collider.GetComponent<BallMaster>()) {
			// We want to update the ball's status
			ballMaster.SetStatus(BallMaster.Status.OutOfPlay);
		}
	}

}
