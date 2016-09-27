using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public BallMaster ballMaster;

	private Vector3 initialPosition;
	private Vector3 offset;


	void Awake() {
		initialPosition = transform.position;
	}

	// Use this for initialization
	void Start () {
		transform.position = initialPosition;
		offset = transform.position - ballMaster.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		// We want to stop the camera once the ball reaches the headpin (temporarily hard-coded to 1729)
		if (transform.position.z < 1729f && (ballMaster.GetStatus() == BallMaster.Status.InPlay)) {
			transform.position = ballMaster.transform.position + offset;
		}
	}

	public void Reset() {
		transform.position = ballMaster.transform.position + offset;
	}
}
