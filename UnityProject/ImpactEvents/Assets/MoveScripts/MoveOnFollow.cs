using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveOnFollow : MoveOnController {

	GameObject target;

	float speedRange = 0.0f;
	[SerializeField]
	bool limitFollowSpeed = true;

	[SerializeField]
	float maxDistanceFromTarget = 0.0f;
	Vector3 nextTargetLocation = Vector3.zero;
	bool hasTarget = false;
	Queue<Vector3> followPositions = new Queue<Vector3>();

	public void Initialize(GameObject newTarget, float newXSpeed, float newYSpeed) {
		target = newTarget;

		xSpeed = newXSpeed;
		ySpeed = newYSpeed;
		speedRange = Mathf.Max(Mathf.Abs(newXSpeed), Mathf.Abs(newYSpeed));

		followPositions.Clear();

		nextTargetLocation = transform.position;
	}

	void Update() {
		if(target != null && target.activeInHierarchy) {
			transform.position = GetNextPosition(Time.deltaTime, transform.position, target.transform.position);
		} else {
			if(target != null && !target.activeInHierarchy) {
				target = null;

			}
			transform.position = GetNextPosition(Time.deltaTime, transform.position);
		}
	}

	Vector3 GetNextPosition(float deltaTime, Vector3 currentPosition, Vector3 targetPosition){
		Vector3 nextPosition = currentPosition;

		// if we are far enough away from our target, we should record that position
		// so we can move towards it
		float distanceToTarget = Vector3.Distance(currentPosition, targetPosition);
		if(distanceToTarget > maxDistanceFromTarget) {
			followPositions.Enqueue(targetPosition);
		}

		// if we don't have a target location, but we do have positions queued, 
		// then we set the next position as our target so we can start moving towards it
		if(!hasTarget && followPositions.Count > 0){
			nextTargetLocation = followPositions.Dequeue();
			hasTarget = true;
		}

		if(hasTarget){
			float distanceX = nextTargetLocation.x - currentPosition.x;
			float distanceY = nextTargetLocation.y - currentPosition.y;
			if(limitFollowSpeed) {
				float angle = Mathf.Atan2(currentPosition.y - nextTargetLocation.y, currentPosition.x - nextTargetLocation.x);
				// change the direction, but do not lose speed
				xSpeed = Mathf.Cos(angle) * speedRange * Mathf.Sign(distanceX);
				ySpeed = Mathf.Sin(angle) * speedRange * Mathf.Sign(distanceY);

				float range = speedRange * deltaTime;
				distanceX = Mathf.Min(Mathf.Max(distanceX, -range), range);
				distanceY = Mathf.Min(Mathf.Max(distanceY, -range), range);
			}
			nextPosition.x = currentPosition.x + distanceX;
			nextPosition.y = currentPosition.y + distanceY;
			// if our next position will get us to our target location
			// we no longer need to move
			float delta = Vector3.Distance(nextPosition, currentPosition);
			if(delta <= 1.0f) {
				hasTarget = false;
				nextTargetLocation = nextPosition;
			}
		}

		return nextPosition;
	}
}
