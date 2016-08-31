using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Ball))]

public class DragLaunch : MonoBehaviour {

	private Ball ball;
	private Vector3 startPosition, endPosition;
	private float startTime, endTime;

	// Use this for initialization
	void Start () {
		ball = GetComponent<Ball>();
	}
	
	public void DragStart() {
		// Don't do this if the ball is already rolling
		if (!ball.inPlay) {
			// Capture time and position of drag start/mouse click
			startTime = Time.time;
			startPosition = Input.mousePosition;
		}
	}

	public void DragEnd() {
		// Don't do this if the ball is already rolling
		if (!ball.inPlay) {
			Vector3 positionDifference;
			float timeDuration;
			Vector3 launchVelocity;

			// Capture time and position of drag start/mouse click
			endTime = Time.time;
			endPosition = Input.mousePosition;

			// Calculate the launch velocity (the difference in the y mouse position correlates to the z position in the game
			timeDuration = endTime - startTime;
			positionDifference = new Vector3(endPosition.x - startPosition.x, 0f, endPosition.y - startPosition.y);
			launchVelocity = positionDifference / timeDuration;

// TODO: remove this launchVelocity setting
launchVelocity = new Vector3(0.3f, 0f,1700f);
			// Launch the ball
			ball.Roll(launchVelocity);
		}
	}
}
