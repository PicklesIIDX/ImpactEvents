using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class MoveOnSurface : MoveOnController {

	[System.Serializable]
	private class TerrainMovementRayProperties{
		public float halfWidth = 1.0f;
		public float distanceToFloor = 1.0f;
		public float originOffsetY = 1.0f;
		public float attachedRayLength = 1.0f;
	}

	[SerializeField]
	float speed = 0.0f;

	[SerializeField]
	LayerMask defaultTerrainLayerMask;
	[SerializeField]
	TerrainMovementRayProperties verticalRayProperties;

	RaycastHit leftHitInfo;
	RaycastHit rightHitInfo;
	Vector3 transformUp = Vector3.up;
	Vector3 transformRight = Vector3.right;

	Vector3 leftRayDirectionCurveIn = Vector3.zero;
	Vector3 leftRayDirectionStraight = Vector3.zero;
	Ray leftRay;
	Vector3 rightRayDirectionCurveIn = Vector3.zero;
	Vector3 rightRayDirectionStraight = Vector3.zero;
	Ray rightRay;

	// TODO: Add one to the middle
	RaycastHit centerHitInfo;
	Vector3 centerDirectionStraight = Vector3.zero;
	Ray centerRay;

	bool hitLeft = false;
	bool hitRight = false;
	bool hitCenter = false;

	void Awake(){

	}

	public override void Initialize(float newXSpeed, float newYSpeed) {
		xSpeed = newXSpeed;
		ySpeed = newYSpeed;
		speed = new Vector3(xSpeed, ySpeed, 0).magnitude;
		float angle = Mathf.Atan2(xSpeed, ySpeed) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0, 0, angle);
	}

	/// <summary>
	/// From our current position, perform a series of linecasts to see if we collide with any ground
	/// </summary>
	/// <returns>The test.</returns>
	void LinecastTest(){
		hitLeft = hitRight = hitCenter = false;

		transformUp = transform.up;
		transformRight = transform.right;

		leftRayDirectionStraight = -transform.up;
		leftRay = new Ray(transform.position - verticalRayProperties.originOffsetY * transformUp +
						  verticalRayProperties.halfWidth * transformRight, leftRayDirectionStraight);
		hitLeft = Physics.Linecast(leftRay.origin, leftRay.origin + leftRay.direction * verticalRayProperties.attachedRayLength,
										out leftHitInfo, defaultTerrainLayerMask);

		rightRayDirectionStraight = -transform.up;
		rightRay = new Ray(transform.position - verticalRayProperties.originOffsetY * transformUp -
							   verticalRayProperties.halfWidth * transformRight, rightRayDirectionStraight);
		hitRight = Physics.Linecast(rightRay.origin, rightRay.origin + rightRay.direction * verticalRayProperties.attachedRayLength,
										 out rightHitInfo, defaultTerrainLayerMask);

		centerDirectionStraight = -transform.up;
		centerRay = new Ray(transform.position - verticalRayProperties.originOffsetY * transformUp,
							centerDirectionStraight);
		hitCenter = Physics.Linecast(centerRay.origin, centerRay.origin + centerRay.direction * verticalRayProperties.attachedRayLength,
										  out centerHitInfo, defaultTerrainLayerMask);

		//Debug.LogWarning("LEFT: " + hitLeft + " CENTER :" + hitCenter + " RIGHT: " + hitRight);
		// if we are moving, we want to check if we bump into any walls
		// if we do, then we override the hit info with what we get from the wall, which we will transition to
		if(speed > 0) {
			RaycastHit overrideLeftHitInfo;
			Ray overrideLeftRay = new Ray(transform.position, transformRight);
			if(Physics.Linecast(overrideLeftRay.origin,
							 overrideLeftRay.origin + overrideLeftRay.direction * verticalRayProperties.halfWidth,
										out overrideLeftHitInfo, defaultTerrainLayerMask)) {
				//Debug.LogWarning("OVERRIDE LEFT HORIZONTAL");
				hitLeft = true;
				leftHitInfo = overrideLeftHitInfo;
			}
		}
		if(speed < 0) {
			RaycastHit overrideRightHitInfo;
			Ray overrideRightRay = new Ray(transform.position, -transformRight);
			if(Physics.Linecast(overrideRightRay.origin,
								 overrideRightRay.origin + overrideRightRay.direction * verticalRayProperties.halfWidth,
										out overrideRightHitInfo,
								 defaultTerrainLayerMask)) {
				//Debug.LogWarning("OVERRIDE RIGHT HORIZONTAL");
				hitRight = true;
				rightHitInfo = overrideRightHitInfo;
			}
		}
		// if we haven't hit anything, let's check if we are at a ledge,
		// we check this by projecting a line that starts at our edges and moves inward underneath us
		if(!hitLeft) {
			RaycastHit overrideLeftHitInfo;
			leftRayDirectionCurveIn =
				((transform.position - verticalRayProperties.originOffsetY * transformUp -
				 verticalRayProperties.halfWidth * transformRight) -
				 (centerRay.origin - centerRay.direction * verticalRayProperties.distanceToFloor));
			Ray overrideLeftRay = new Ray(transform.position - verticalRayProperties.distanceToFloor * transformUp +
							  verticalRayProperties.halfWidth * transformRight, leftRayDirectionCurveIn);
			if(Physics.Linecast(overrideLeftRay.origin,
								 overrideLeftRay.origin + overrideLeftRay.direction * verticalRayProperties.attachedRayLength,
									  out overrideLeftHitInfo,
								defaultTerrainLayerMask)) {
				//Debug.LogWarning("OVERRIDE LEFT CURVE");
				hitLeft = true;
				leftHitInfo = overrideLeftHitInfo;
			}
		}
		// curve in if we don't hit
		if(!hitRight) {
			RaycastHit overrideRightHitInfo;
			rightRayDirectionCurveIn =
				((transform.position - verticalRayProperties.originOffsetY * transformUp +
				 verticalRayProperties.halfWidth * transformRight) -
				 (centerRay.origin - centerRay.direction * verticalRayProperties.distanceToFloor));
			Ray overrideRightRay = new Ray(transform.position - verticalRayProperties.distanceToFloor * transformUp -
							   verticalRayProperties.halfWidth * transformRight, rightRayDirectionCurveIn);
			if(Physics.Linecast(overrideRightRay.origin,
								 overrideRightRay.origin + overrideRightRay.direction * verticalRayProperties.attachedRayLength,
								 out overrideRightHitInfo, defaultTerrainLayerMask)) {
				//Debug.LogWarning("OVERRIDE RIGHT CURVE");
				hitRight = true;
				rightHitInfo = overrideRightHitInfo;
			}
		}
	}

	void Update() {

		Vector3 currentPosition = transform.position;

		for(int i = 0; i < 10; i ++){
			transform.position = currentPosition;
			transform.position += transform.right * speed * Time.deltaTime * ((10.0f-i)/10.0f);
			LinecastTest();
			if((hitLeft && hitRight) || hitCenter){
				break;
			}
		}

		if(hitLeft || hitRight || hitCenter) {

			Vector3 averageNormal = Vector3.zero;
			Vector3 averagePoint = Vector3.zero;
			if(hitLeft && hitRight) {
				averageNormal = (leftHitInfo.normal + rightHitInfo.normal) * 0.5f;
				//Debug.LogWarning("Using average normal: " + averageNormal);
			} else if (hitCenter) {
				averageNormal = centerHitInfo.normal;
				//Debug.LogWarning("Using center normal: " + averageNormal);
			} else if (hitLeft){
				averageNormal = leftHitInfo.normal;
			} else if (hitRight){
				averageNormal = rightHitInfo.normal;
			}
			if(hitLeft && hitRight) {
				averagePoint = (leftHitInfo.point + rightHitInfo.point) * 0.5f;
				//Debug.LogWarning("Using average point: " + averagePoint);
			} else if (hitCenter){
				averagePoint = centerHitInfo.point;
				//Debug.LogWarning("Using center point: " + averagePoint);
			} else if (hitLeft){
				averagePoint = leftHitInfo.point;
			} else if (hitRight){
				averagePoint = rightHitInfo.point;
			}

			Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, averageNormal);
			Quaternion finalRotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
			                                                    speed);
			transform.rotation = Quaternion.Euler(0, 0, finalRotation.eulerAngles.z);



			// if we curve, we should still use the straight average
			if(hitLeft && hitRight) {
				transform.position = (averagePoint + transform.up * verticalRayProperties.distanceToFloor);
			} else if(hitCenter) {
				transform.position = (centerHitInfo.point + transform.up * verticalRayProperties.distanceToFloor);
			} else if(hitLeft){
				transform.position = (leftHitInfo.point + transform.up * verticalRayProperties.distanceToFloor);
			} else if (hitRight){
				transform.position = (rightHitInfo.point + transform.up * verticalRayProperties.distanceToFloor);
			}

		} else {
			// update down angle to the direction we are moving in
			Quaternion targetRotation = Quaternion.FromToRotation(Vector3.down, -transform.up);
			Quaternion finalRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, speed);
			transform.rotation = Quaternion.Euler(0, 0, finalRotation.eulerAngles.z);

			// move along velocity
			currentPosition.x += -transform.up.x * speed * Time.deltaTime;
			currentPosition.y += -transform.up.y * speed * Time.deltaTime;
			transform.position = currentPosition;
		}

	}

	void LateUpdate(){

		// DEBUG
		// left = yellow
		Debug.DrawRay(leftRay.origin, leftRay.direction * verticalRayProperties.attachedRayLength, Color.yellow);
		// horizontal
		Debug.DrawRay(transform.position, 
		              transformRight * verticalRayProperties.halfWidth, Color.yellow);
		// left normal = green
		Debug.DrawRay(leftHitInfo.point, leftHitInfo.normal * 10, Color.green);


		// right = red
		Debug.DrawRay(rightRay.origin, rightRay.direction * verticalRayProperties.attachedRayLength, Color.red);
		// horizontal
		Debug.DrawRay(transform.position, 
		              -transformRight * verticalRayProperties.halfWidth, Color.red);
		
		// right normal = magenta
		Debug.DrawRay(rightHitInfo.point, rightHitInfo.normal * 10, Color.magenta);


		Vector3 averageNormal = (leftHitInfo.normal + rightHitInfo.normal) * 0.5f;
		Vector3 averagePoint = (leftHitInfo.point + rightHitInfo.point) * 0.5f;
		// average = cyan
		Debug.DrawRay(averagePoint, averageNormal, Color.cyan);
		// facing = white
		Debug.DrawRay(transform.position - verticalRayProperties.originOffsetY * transformUp, 
		              -transform.up * verticalRayProperties.attachedRayLength,
		              Color.white);

		// draw center lines
		Debug.DrawRay(transform.position, centerDirectionStraight * verticalRayProperties.attachedRayLength, 
		              new Color(.5f, .5f, .8f));
		Debug.DrawRay(centerHitInfo.point, centerHitInfo.normal * 10, new Color(.7f, .7f, .9f));

		// draw left curve in
		Debug.DrawRay(transform.position - verticalRayProperties.distanceToFloor * transformUp +
							  verticalRayProperties.halfWidth * transformRight,
		              leftRayDirectionCurveIn,
		              Color.gray);
		// draw right curve in
		Debug.DrawRay(transform.position - verticalRayProperties.distanceToFloor * transformUp -
							  verticalRayProperties.halfWidth * transformRight,
		              rightRayDirectionCurveIn,
		              Color.gray);
	}

}
