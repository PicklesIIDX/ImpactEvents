using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class BulletController : MonoBehaviour {
	
	bool initialized = false;

	MoveOnVelocity moveOnVelocity;
	MoveOnPID moveOnPID;

	MoveOnSurface moveOnSurface;

	void Awake(){
		moveOnVelocity = GetComponent<MoveOnVelocity>();
		moveOnSurface = GetComponent<MoveOnSurface>();
		moveOnPID = GetComponent<MoveOnPID>();
	}

	public void Initialize(float xSpeed, float ySpeed){
		UpdateBulletDirection(xSpeed, ySpeed);
		initialized = true;
	}

	void UpdateBulletDirection(float xSpeed, float ySpeed){
		if(initialized){
			return;
		}

		if(moveOnVelocity != null) {
			moveOnVelocity.enabled = true;
			moveOnVelocity.Initialize(xSpeed, ySpeed);
		}
		if(moveOnPID != null) {
			moveOnPID.enabled = true;
			moveOnPID.Initialize(null, xSpeed, ySpeed);
		}
		if(moveOnSurface != null) {
			moveOnSurface.enabled = true;
			moveOnSurface.Initialize(xSpeed, ySpeed);
		}
	}

	void OnDisable(){
		initialized = false;
	}

}
