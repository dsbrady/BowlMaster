using UnityEngine;
using System.Collections;

public class Pin : MonoBehaviour {

	public bool hasFallen = false;
	public float standingThreshold = 3f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	public bool IsStanding() {
		return (transform.forward.y > 0.6f);
	}

	public void Settle(bool enableGravity) {
		Rigidbody rigidBody = gameObject.GetComponent<Rigidbody>();
		rigidBody.useGravity = enableGravity;
		rigidBody.velocity = Vector3.zero;
		rigidBody.angularVelocity = Vector3.zero;
		transform.rotation = Quaternion.Euler(270f, 0f, 0f);
	}
}
