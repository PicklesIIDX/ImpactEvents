using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using PickleTools.ImpactEvents;

public class ImpactEffectSpawnFollower : ImpactEffect {

	[SerializeField]
	GameObject follower;
	[SerializeField]
	int followers = 1;
	[SerializeField]
	bool disableFollowersOnDisable = true;
	[SerializeField]
	bool followRoot = false;

	Pool followerPool;
	List<GameObject> activeFollowers = new List<GameObject>();

	float xSpeed = 0;
	float ySpeed = 0;

	void Awake(){
		followerPool = new Pool(name, follower, followers);
	}

	public override void TriggerEffect(GameObject listener, Dictionary<string, object> triggerData) {
		// get speed
		MoveOnVelocity moveOnVelocity = GetComponent<MoveOnVelocity>();
		if(moveOnVelocity != null) {
			xSpeed = moveOnVelocity.XSpeed;
			ySpeed = moveOnVelocity.YSpeed;
		}
		MoveOnPID moveOnPID = GetComponent<MoveOnPID>();
		if(moveOnPID != null) {
			xSpeed = moveOnPID.XSpeed;
			ySpeed = moveOnPID.YSpeed;
		}
		MoveOnFollow moveOnFollow = GetComponent<MoveOnFollow>();
		if(moveOnFollow != null) {
			xSpeed = moveOnFollow.XSpeed;
			ySpeed = moveOnFollow.YSpeed;
		}
		MoveOnSurface moveOnSurface = GetComponent<MoveOnSurface>();
		if(moveOnSurface != null) {
			xSpeed = moveOnSurface.XSpeed;
			ySpeed = moveOnSurface.YSpeed;
		}

		GameObject lastFollower = listener;
		for(int f = 0; f < followers; f ++){
			GameObject newFollower = followerPool.GetNext();
			newFollower.layer = gameObject.layer;
			newFollower.transform.position = transform.position;
			moveOnFollow = newFollower.GetComponent<MoveOnFollow>();
			if(moveOnFollow != null){
				moveOnFollow.Initialize(lastFollower, xSpeed, ySpeed);
			}
			lastFollower = newFollower;
			activeFollowers.Add(newFollower);
			if(followRoot){
				lastFollower = listener;
			}
		}
		TriggerComplete();
	}

	void OnDisable(){
		if(disableFollowersOnDisable) {
			for(int f = 0; f < activeFollowers.Count; f++) {
				if(activeFollowers[f] != null) {
					activeFollowers[f].SetActive(false);
				}
			}
		}
		activeFollowers.Clear();
	}
}
