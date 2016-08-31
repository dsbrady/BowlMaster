using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Ball ball;
	private Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = transform.position - ball.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		// We want to stop the camera once the ball reaches the headpin (temporarily hard-coded to 1729)
		if (transform.position.z < 1729f && ball.inPlay) {
			transform.position = ball.transform.position + offset;
		}
	}

	public void Reset() {
		transform.position = ball.transform.position + offset;
	}
}
