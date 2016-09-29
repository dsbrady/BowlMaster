using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public BallMaster ballMaster;

	private Vector3 initialPosition;
	private float yOffset, zOffset;

	void Awake() {
		initialPosition = transform.position;
	}

	// Use this for initialization
	void Start () {
		transform.position = initialPosition;
		yOffset = transform.position.y - ballMaster.transform.position.y;
		zOffset = transform.position.z - ballMaster.transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
		// We want to stop the camera once the ball reaches the headpin (temporarily hard-coded to 1729)
		if (transform.position.z < 1729f && (ballMaster.GetStatus() == BallMaster.Status.InPlay || ballMaster.GetStatus() == BallMaster.Status.Idle)) {
			transform.position = new Vector3(ballMaster.transform.position.x, ballMaster.transform.position.y + yOffset, ballMaster.transform.position.z + zOffset);
		}
	}

	public void Reset() {
		transform.position = new Vector3(ballMaster.transform.position.x, ballMaster.transform.position.y + yOffset, ballMaster.transform.position.z + zOffset);
	}
}
