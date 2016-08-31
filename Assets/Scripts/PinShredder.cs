using UnityEngine;
using System.Collections;

public class PinShredder : MonoBehaviour {

	void OnTriggerExit(Collider collider) {
		// We want to destroy any pins that leave the box
		if (collider.GetComponent<Pin>()) {
			Destroy(collider.gameObject);
		}
	}

}
