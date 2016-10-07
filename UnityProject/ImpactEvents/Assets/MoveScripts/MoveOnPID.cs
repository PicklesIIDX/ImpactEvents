using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using PickleTools;

public class MoveOnPID : MoveOnController {

	PIDController vPID;
	PIDController hPID;
	GameObject target;

	float speedRange = 0.0f;
	[SerializeField]
	bool limitFollowSpeed = false;

	[SerializeField]
	Vector3 offset;

	[SerializeField][Range(-1.0f, 4.0f)]
	float movementSpeed = 0.0f;
	[SerializeField]//[Range(-0.001f, -0.05f)]
	float relentlessness = 0.0f;
	[SerializeField]//[Range(-0.001f, -0.0001f)]
	float smoothness = 0.0f;

	[SerializeField]
	[Range(0, 100)]
	int predictiveCounts = 0;
	Vector3 lastPosition = Vector3.zero;
	[SerializeField]
	float avoidSimilar = 0.0f;
	List<GameObject> similarObjects = new List<GameObject>();

	[SerializeField]
	bool setTowardsClosestTarget = false;
	[SerializeField]
	bool followClosestTarget = false;
	[SerializeField]
	bool updateTarget = false;

	[SerializeField]
	Collider sightTrigger;
	List<GameObject> sightList = new List<GameObject>();
	[SerializeField]
	LayerMask sightMask = 0;

	UpdateTimer updatePIDTimer;
	[SerializeField]
	float updateTime = 0.02f;
	bool canUpdatePID = false;
	Vector3 currentTargetPosition = Vector3.zero;

	[SerializeField]
	bool velocityBasedInput = false;

	void Awake(){
		hPID = new PIDController(movementSpeed, relentlessness, smoothness);
		vPID = new PIDController(movementSpeed, relentlessness, smoothness);
		updatePIDTimer = new UpdateTimer(updateTime);
		updatePIDTimer.TimerComplete += HandleUpdateTimerComplete;
	}

	void OnDestroy(){
		updatePIDTimer.TimerComplete -= HandleUpdateTimerComplete;
	}

	public void Initialize(GameObject newTarget, float newXSpeed, float newYSpeed) {
		target = newTarget;

		xSpeed = newXSpeed;
		ySpeed = newYSpeed;

		speedRange = Mathf.Max(newXSpeed, newYSpeed);

		// move based on the bullet's configuration
		if(setTowardsClosestTarget || followClosestTarget || updateTarget) {
			GameObject closestTarget = GetClosestObject();
			if(closestTarget != null) {
				float angle = Mathf.Atan2(closestTarget.transform.position.y - transform.position.y,
				                          closestTarget.transform.position.x - transform.position.x);
				target = closestTarget.gameObject;
				xSpeed = Mathf.Cos(angle) * speedRange;
				ySpeed = Mathf.Sin(angle) * speedRange;
			}
			if(!followClosestTarget && !updateTarget){
				target = null;
			}
		}

		if(target != null){
			lastPosition = target.transform.position;

			hPID.P = movementSpeed;
			hPID.I = relentlessness;
			hPID.D = smoothness;
			hPID.Reset();
			vPID.P = movementSpeed;
			vPID.I = relentlessness;
			vPID.D = smoothness;
			vPID.Reset();
			if(avoidSimilar > 0.0f) {
				similarObjects.Clear();
				BulletController[] bullets = GameObject.FindObjectsOfType<BulletController>();
				for(int b = 0; b < bullets.Length; b++) {
					if(!bullets[b].gameObject.activeInHierarchy || bullets[b].gameObject == gameObject) {
						return;
					}
					similarObjects.Add(bullets[b].gameObject);
				}
			}
			currentTargetPosition = target.transform.position + offset;
		}
	}

	void Update() {
		if(target != null) {
			if(velocityBasedInput){
				if(canUpdatePID){
					currentTargetPosition = target.transform.position + offset;
					canUpdatePID = false;
				}
				// move in a direction, influcenced by the target
				GetNextSpeed(Time.deltaTime, transform.position, ref xSpeed, ref ySpeed, currentTargetPosition);
				transform.position += new Vector3(xSpeed, ySpeed);
			} else {
				// move directly towards the target
				if(canUpdatePID) {
					currentTargetPosition = target.transform.position + offset;
					canUpdatePID = false;
				}
				transform.position = GetNextPosition(Time.deltaTime, transform.position, currentTargetPosition);
			}

		} else {
			transform.position = GetNextPosition(Time.deltaTime, transform.position);	
		}
		if(hPID.P != movementSpeed ||
		   hPID.I != relentlessness ||
		   hPID.D != smoothness){
			hPID.Reset();
		}
		hPID.P = movementSpeed;
		hPID.I = relentlessness;
		hPID.D = smoothness;
		if(vPID.P != movementSpeed ||
		   vPID.I != relentlessness ||
		   vPID.D != smoothness) {
			vPID.Reset();
		}
		vPID.P = movementSpeed;
		vPID.I = relentlessness;
		vPID.D = smoothness;

		updatePIDTimer.Update(Time.deltaTime);
	}

	void HandleUpdateTimerComplete(UpdateTimer updateTimer){
		canUpdatePID = true;
		updateTimer.Reset(updateTime);
	}

	private Vector3 GetNextPosition(float deltaTime, Vector3 currentPosition, Vector3 targetPosition) {
		Vector3 nextPosition = currentPosition;
		Vector3 aimingPosition = targetPosition;
		Vector3 velocity = targetPosition - lastPosition;
		aimingPosition += velocity * predictiveCounts;
		lastPosition = targetPosition;

		float pidX = hPID.UpdateAt(deltaTime, currentPosition.x, aimingPosition.x);
		float pidY = vPID.UpdateAt(deltaTime, currentPosition.y, aimingPosition.y);

		for(int s = 0; s < similarObjects.Count; s ++){
			float distance = Vector2.Distance(similarObjects[s].transform.position, nextPosition);
			if(distance > avoidSimilar){
				continue;
			}
			pidX -= hPID.UpdateAt(deltaTime, nextPosition.x, similarObjects[s].transform.position.x);
			pidY -= vPID.UpdateAt(deltaTime, nextPosition.y, similarObjects[s].transform.position.y);
		}

		if(limitFollowSpeed){
			float range = speedRange * deltaTime;
			pidX = Mathf.Min(Mathf.Max(pidX, -range), range);
			pidY = Mathf.Min(Mathf.Max(pidY, -range), range);
		}
		nextPosition.x -= pidX;
		nextPosition.y -= pidY;

		return nextPosition;
	}

	private void GetNextSpeed(float deltaTime, Vector3 currentPosition, ref float hSpeed, ref float vSpeed, Vector3 targetPosition){
		hSpeed = -hPID.UpdateAt(deltaTime, currentPosition.x, targetPosition.x);
		vSpeed = -vPID.UpdateAt(deltaTime, currentPosition.y, targetPosition.y);

		for(int s = 0; s < similarObjects.Count; s++) {
			float distance = Vector2.Distance(similarObjects[s].transform.position, currentPosition);
			if(distance > avoidSimilar) {
				continue;
			}
			hSpeed += hPID.UpdateAt(deltaTime, currentPosition.x, similarObjects[s].transform.position.x);
			vSpeed += vPID.UpdateAt(deltaTime, currentPosition.y, similarObjects[s].transform.position.y);
		}

		if(limitFollowSpeed) {
			float range = speedRange * deltaTime;
			hSpeed = Mathf.Min(Mathf.Max(hSpeed, -range), range);
			vSpeed = Mathf.Min(Mathf.Max(vSpeed, -range), range);
		}
	}

	GameObject GetClosestObject(){
		float minDistance = 9999;
		int index = -1;
		for (int i = 0; i < sightList.Count; i ++){
			float newDistance = Vector2.Distance(transform.position, sightList[i].transform.position);
			if(newDistance < minDistance){
				minDistance = newDistance;
				index = i;
			}
		}

		if(index > -1){
			return sightList[index];
		}

		return null;
	}

	void OnTriggerEnter(Collider other){
		if((sightMask & (1 << other.gameObject.layer)) > 0 &&
		   !sightList.Contains(other.gameObject)){
			sightList.Add(other.gameObject);
		}
	}

	void OnTriggerExit(Collider other){
		sightList.Remove(other.gameObject);
	}
}
