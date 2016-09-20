using UnityEngine;
using System.Collections;

public class BallMaster : MonoBehaviour {

	public enum Status {Idle, InPlay, OutOfPlay };

	public Vector3 launchVelocity;

	private CameraController cameraController;
	private Vector3 initialPosition;
	private Quaternion initialRotation;
	private Rigidbody rigidBody;
	private AudioSource rollingAudioSource;
	private Status status = Status.Idle;
	private Vector3 velocity;

	void Awake() {
		cameraController = GameObject.FindObjectOfType<CameraController>();
		initialPosition = transform.position;
		initialRotation = transform.rotation;
		rigidBody = gameObject.GetComponent<Rigidbody>();
		rigidBody.useGravity = false;
		rollingAudioSource = GetComponent<AudioSource>();
	}

	public Status GetStatus() {
		return status;
	}

	public void NudgeStart(float amount) {
		// Only allow this if the ball isn't rolling
		if (status == Status.Idle) {
			transform.Translate(new Vector3(0,0,amount));
		}
	}

	public void Reset() {
		status = Status.Idle;
		rigidBody.useGravity = false;
		transform.position = initialPosition;
		transform.rotation = initialRotation;
		rigidBody.velocity = Vector3.zero;
		rigidBody.angularVelocity = Vector3.zero;
		cameraController.Reset();
	}

	public void Roll(Vector3 initialVelocity) {
		// Just set an initial speed for now, will look at adding in hook later
		status = Status.InPlay;
		rigidBody.useGravity = true;
		velocity = initialVelocity;
		rollingAudioSource.Play();
		rigidBody.velocity = velocity;
	}

	public void SetStatus(Status newStatus) {
		status = newStatus;
	}
}
