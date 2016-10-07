using UnityEngine;
using System.Collections;

public class MoveOnVelocity : MoveOnController {

	void Update () {
		transform.position = GetNextPosition(Time.deltaTime, transform.position);
	}

	new public Vector3 GetNextPosition(float deltaTime, Vector3 currentPosition) {
		currentPosition.x += xSpeed * deltaTime;
		currentPosition.y += ySpeed * deltaTime;
		return currentPosition;
	}
}
