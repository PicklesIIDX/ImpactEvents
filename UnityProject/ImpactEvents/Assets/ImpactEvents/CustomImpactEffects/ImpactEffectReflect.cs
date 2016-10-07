using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using PickleTools.ImpactEvents;

public class ImpactEffectReflect : ImpactEffect {

	MoveOnController moveOnController;

	Collider[] colliders;
	Rect boundsRect;

	void Awake(){
		moveOnController = GetComponent<MoveOnController>();
		Assert.IsNotNull(moveOnController);

		// get size of bullet
		colliders = GetComponentsInChildren<Collider>();
		for(int c = 0; c < colliders.Length; c++) {
			float xMax = transform.position.x - colliders[c].bounds.center.x + colliders[c].bounds.extents.x;
			float xMin = transform.position.x - colliders[c].bounds.center.x - colliders[c].bounds.extents.x;
			float yMax = transform.position.y - colliders[c].bounds.center.y + colliders[c].bounds.extents.y;
			float yMin = transform.position.y - colliders[c].bounds.center.y - colliders[c].bounds.extents.y;
			if(boundsRect.xMin > xMin) {
				boundsRect.xMin = xMin;
			}
			if(boundsRect.xMax < xMax) {
				boundsRect.xMax = xMax;
			}
			if(boundsRect.yMin > yMin) {
				boundsRect.yMin = yMin;
			}
			if(boundsRect.yMax < yMax) {
				boundsRect.yMax = yMax;
			}
		}
	}

	void OnEnable(){
		checkCount = 0;
	}

	float checkCount = 0;

	public override void TriggerEffect(GameObject listener, Dictionary<string, object> triggerData) {
		if(checkCount > 8){
			return;
		}
		transform.position -= new Vector3(moveOnController.XSpeed * Time.deltaTime, 
		                                  moveOnController.YSpeed * Time.deltaTime);
		RaycastHit hitInfo;
		RaycastHit closestHitInfo = new RaycastHit();
		float distance = 999;
		// spew lines out in orthoginal directions
		// whichever is closest is the impact normal
		// reflect based on that normal
		// up
		if(Physics.Linecast(transform.position, transform.position + new Vector3(0, boundsRect.yMax + 10, 0), out hitInfo)){
			if(hitInfo.distance < distance){
				distance = hitInfo.distance;
				closestHitInfo = hitInfo;
			}
		}
		// down
		if(Physics.Linecast(transform.position, transform.position + new Vector3(0, boundsRect.yMin - 10, 0), out hitInfo)) {
			if(hitInfo.distance < distance) {
				distance = hitInfo.distance;
				closestHitInfo = hitInfo;
			}
		}
		// left
		if(Physics.Linecast(transform.position, transform.position + new Vector3(boundsRect.xMin - 10, 0, 0), out hitInfo)) {
			if(hitInfo.distance < distance) {
				distance = hitInfo.distance;
				closestHitInfo = hitInfo;
			}
		}
		// right
		if(Physics.Linecast(transform.position, transform.position + new Vector3(boundsRect.xMax + 10, 0, 0), out hitInfo)) {
			if(hitInfo.distance < distance) {
				distance = hitInfo.distance;
				closestHitInfo = hitInfo;
			}
		}

		if(distance < 999) {
			Assert.IsTrue(distance < 999);
			// reflect based on normal
			Vector3 newSpeed = Vector3.Reflect(new Vector3(moveOnController.XSpeed, moveOnController.YSpeed),
							closestHitInfo.normal);
			moveOnController.Initialize(newSpeed.x, newSpeed.y);
			transform.position += new Vector3(moveOnController.XSpeed * Time.deltaTime,
			                                  moveOnController.YSpeed * Time.deltaTime);
			TriggerComplete();
		} else {
			checkCount++;
			TriggerEffect(listener, triggerData);
		}

	}
}
