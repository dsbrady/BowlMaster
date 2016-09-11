using UnityEngine;
using System.Collections;

public class LaneBox : MonoBehaviour {

	void OnTriggerExit(Collider collider) {
		// Make sure it's the ball that's leaving
		if (collider.GetComponent<BallMaster>()) {
			// We want to set the PinSetter's ballLeftBox to true
			PinSetter pinSetter = GameObject.FindObjectOfType<PinSetter>();
			pinSetter.SetBallOutOfPlay(true);
		}
	}

}
