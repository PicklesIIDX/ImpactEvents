using UnityEngine;
using PickleTools.Resource;
using UnityEngine.Assertions;


public class WeaponController:MonoBehaviour {

	[SerializeField][Tooltip("A name for the weapon that should be player facing")]
	private string displayName = "NONE";
	public string DisplayName {
		get { return displayName; }
	}

	// trigger
	[Header("Resources")]
	[SerializeField][Tooltip("how long is seconds the button must be held before we can fire")]
	Resource charge;
	public Resource Charge {
		get { return charge; }
	}
	[SerializeField][Tooltip("a looping list of numbers that changes the charge team on each bullet fired")]
	float[] chargeCycle = new float[0];
	int chargeCycleIndex = -1;
	[SerializeField][Tooltip("how long in seconds before the weapon can be fired again")]
	Resource reload;
	public Resource Reload {
		get { return reload; }
	}
	[SerializeField][Tooltip("a looping list of numbers that changes the reload time on each bullet fired")]
	float[] reloadCycle = new float[0];
	int reloadCycleIndex = -1;
	[SerializeField][Tooltip("how many shots can be made before the weapon can no longer be fired")]
	Resource ammo;
	[SerializeField][Tooltip("a value that can be used to change how the weapon works")]
	Resource powerLevel;


	[Header("Spawn Position")]
	[SerializeField][Tooltip("if false, the bullets spawn at the weapon's position; if true, the bullets spawn at the " +
	                         "world origin")]
	bool spawnAbsolute = false;
	[SerializeField][Tooltip("bullet offset from the spawn point")]
	Vector3 spawnOffset = Vector3.zero;
	[SerializeField][Tooltip("random range to further offset the x spawn position")]
	float spawnXMin = 0.0f;
	[SerializeField][Tooltip("random range to further offset the x spawn position")]
	float spawnXMax = 0.0f;
	[SerializeField][Tooltip("random range to further offset the y spawn position")]
	float spawnYMin = 0.0f;
	[SerializeField][Tooltip("random range to further offset the y spawn position")]
	float spawnYMax = 0.0f;


	[Header("Spawn Direction")]
	[SerializeField][Tooltip("if true, 0 degrees faces in the positive x direction; if false, it faces in negative x")]
	bool rightIs0Degrees = true;
	[SerializeField][Tooltip("minimum angle in degrees")]
	float directionMin = 0.0f;
	[SerializeField][Tooltip("maximum angle in degrees")]
	float directionMax = 0.0f;
	[SerializeField][Tooltip("if true, the curve below will be used to determine angle instead of the min and max")]
	bool useDirectionCurveForAngle = false;
	[SerializeField][Tooltip("indicates the angle bullets will spawn (y axis) depending on time (x axis)")]
	AnimationCurve directionCurve = AnimationCurve.Linear(0, 0, 1.0f, 1.0f);
	[SerializeField][Tooltip("overrides the x axis of the direction curve to use shots fired instead of time")]
	bool directionBasedOnShotsFired = false;

	[Header("Speed")]
	[SerializeField][Tooltip("how fast in pixels per second the bullets should travel")]
	float speed = 100.0f;
	public float Speed {
		get { return speed; }
	}
	[SerializeField][Tooltip("if true, the bullet will be a child of the weapon and move with the weapon after its fired")]
	bool attachToWeapon = false;

	[Header("Bullets")]
	[SerializeField][Tooltip("how many bullets to fire simultaneously")]
	int bulletsPerShot = 1;
	[SerializeField][Tooltip("all the possible bullet prefabs that this weapon can fire")]
	GameObject[] bullets;
	Pool[] bulletPools = new Pool[0];
	[SerializeField][Tooltip("how many bullets you expect to be onscreen at any one time")]
	int poolSize = 1;
	[SerializeField][Tooltip("the sequence of how bullets from the above list are chosen")]
	AnimationCurve fireCurve = AnimationCurve.Linear(0, 0, 1, 1);
	[SerializeField][Tooltip("if true, bullets will be selected completely randomly")]
	bool fireRandom = false;
	[SerializeField][Tooltip("if true, the curve will be used to randomly pick bullets, allowing you to set chance")]
	bool fireRandomBasedOnCurve = false;

	[Header("Fire Type")]
	[SerializeField]
	bool fireOnPress = false;
	[SerializeField]
	bool fireOnHold = true;
	[SerializeField]
	bool fireOnRelease = false;

	bool canUseInput = true;
	public bool CanUseInput{
		get { return canUseInput; }
		set { canUseInput = value; }
	}

	void Awake() {
		Assert.IsTrue(bullets.Length > 0);
		// initialize our bullets to prevent instantiation at runtime
		bulletPools = new Pool[bullets.Length];
		for(int b = 0; b < bullets.Length; b++) {
			bulletPools[b] = new Pool(name, bullets[b], poolSize);
		}

	}

	void OnEnable() {
		if(reloadCycle.Length > 0){
			reloadCycleIndex = 0;
		}
		if (chargeCycle.Length > 0) {
			reloadCycleIndex = 0;
		}
	}


	public void HandlePlayerInput(bool justPressed = false, bool isPressing = false, bool justReleased = false) {
		if(!gameObject.activeInHierarchy || !canUseInput) {
			return;
		}
		if(justPressed) {
			OnPress();
		} else if(isPressing) {
			OnHold();
		} else if(justReleased) {
			OnRelease();
		} else {
			OnNothing();
		}
	}

	void OnPress() {
		if(reload.IsMax && fireOnPress) {
			Fire();
		}
	}

	void OnHold() {
		if(reload.IsMax) {
			ChargeAction(Time.deltaTime);
		}
		if(reload.IsMax && fireOnHold) {
			Fire();
		}
		ReloadAction(Time.deltaTime);
	}

	void OnRelease() {
		if(charge.IsMax && fireOnRelease) {
			Fire();
		}
		charge.Amount = charge.Min;
	}

	void OnNothing() {
		ReloadAction(Time.deltaTime);
	}

	void ReloadAction(float deltaTime){
		if(!reload.IsMax &&
		   (ammo.Max.Equals(0) || !ammo.IsMin)) {
			reload.Add(deltaTime);
		}
	}

	void ChargeAction(float deltaTime){
		charge.Add(deltaTime);
	}

	void AddAmmoAction(float ammoToAdd){
		ammo.Add(ammoToAdd);
	}

	public void SetFireDirection(bool right = true){
		rightIs0Degrees = right;
	}

	int fireCount = 0;

	/// <summary>
	/// A set of parameters that allows the user to customize how this weapon is fired
	/// </summary>
	[System.Serializable]
	public class FireOptions{
		public bool overridePosition = false;
		public float offsetX = 0.0f;
		public float offsetY = 0.0f;
		public GameObject[] targets = new GameObject[0];
		public bool overrideDirection = false;
		public float xSpeed = 0.0f;
		public float ySpeed = 0.0f;
	}

	FireOptions defaultOptions = new FireOptions();

	public void Fire(FireOptions options = null) {
		if(options == null){
			options = defaultOptions;
		}
		for(int b = 0; b < bulletsPerShot; b++) {
			if(ammo.Max > 0 && ammo.IsMin){
				break;
			}
			// reset firing situation
			reload.Amount = reload.Min;
			ammo.Amount -= 1.0f;
			if(reloadCycleIndex >= 0) {
				reload.Max = reloadCycle[reloadCycleIndex];
				reloadCycleIndex++;
				if(reloadCycleIndex >= reloadCycle.Length) {
					reloadCycleIndex = 0;
				}
			}
			if (chargeCycleIndex >= 0) {
				reload.Max = chargeCycle[chargeCycleIndex];
				chargeCycleIndex++;
				if (chargeCycleIndex >= chargeCycle.Length) {
					chargeCycleIndex = 0;
				}
			}

			// get the next bullet to fire
			int bulletIndex = 0;
			if(fireRandom) {
				// select a completely random bullet
				bulletIndex = Random.Range(0, bullets.Length);
			} else if (fireRandomBasedOnCurve) {
				// select a bullet based on the fire curve and time; this will cycle through our bullets list
				bulletIndex = (int)(fireCurve.Evaluate(Random.Range(0.0f, 1.0f)) * bullets.Length % bullets.Length);
			} else if(bullets.Length > 0) {
				// sellect a bullet based on the fire cuve and how many bullets we have fired; this cycles through our 
				// bullets list
				bulletIndex = (int)(fireCurve.Evaluate((float)fireCount / ((float)bullets.Length)) * bullets.Length % bullets.Length);
			}
			// get the bullet from our selected pool
			GameObject instance = bulletPools[bulletIndex].GetNext();
			Assert.IsNotNull(instance);
			// if we are ignoring any specific objects, pass that onto the trigger that checks collisions
			ImpactTriggerCollideWithLayers collideWithLayers = instance.GetComponent<ImpactTriggerCollideWithLayers>();
			if(collideWithLayers != null){
				collideWithLayers.IgnoreList.AddRange(options.targets);
			}
			if(!spawnAbsolute) {
				instance.transform.position = transform.position;
			} else {
				instance.transform.position = Vector3.zero;
			}
			instance.transform.position += spawnOffset;
			// apply random offset and apply function caller offset
			instance.transform.position += new Vector3(
				Random.Range(spawnXMin, spawnXMax),
				Random.Range(spawnYMin, spawnYMax),
				0
			);
			if(options.overridePosition) {
				instance.transform.position = Vector3.zero;
			}
			instance.transform.position += new Vector3(
				options.offsetX,
				options.offsetY,
				0
			);

			float xSpeed = 0.0f;
			float ySpeed = 0.0f;
			float angle = 0;
			if(rightIs0Degrees) {
				angle = 0;
			} else {
				angle = 180;
			}

			if(useDirectionCurveForAngle) {
				if(directionBasedOnShotsFired) {
					// this check will ensure that bullets based on shots fired will have their bullets evenly distributed
					// with a bullet at the min and max ranges
					if(bulletsPerShot % 2 == 0) {
						angle += directionCurve.Evaluate((float)(fireCount % bulletsPerShot) / (float)(bulletsPerShot));
					} else {
						angle += directionCurve.Evaluate((float)(fireCount % bulletsPerShot) / (float)(bulletsPerShot-1));
					}
				} else {
					angle += directionCurve.Evaluate(Time.timeSinceLevelLoad);
				}
			} else {
				angle += Random.Range(directionMin, directionMax);
			}
			float radAngle = angle * Mathf.Deg2Rad;

			xSpeed = Mathf.Cos(radAngle) * speed;
			ySpeed = Mathf.Sin(radAngle) * speed;

			BulletController bulletController = instance.GetComponent<BulletController>();
			Assert.IsNotNull(bulletController);
			if(options.overrideDirection) {
				bulletController.Initialize(options.xSpeed, options.ySpeed);
			} else {
				bulletController.Initialize(xSpeed, ySpeed);
			}
			if(attachToWeapon) {
				bulletController.transform.SetParent(transform);
			}
			fireCount++;
		}
	}

	/// 
	/// This section is useful for determining if this weapon can hit a target if fired
	/// primarily useful for AI
	/// 

	public enum TrajectoryType {
		NONE,
		LINE,
		CONE,
		HOMING
	}

	[SerializeField]
	TrajectoryType trajectory;
	public TrajectoryType Trajectory {
		get { return trajectory; }

	}

	public Vector3 GetTrajectoryMin() {
		switch (trajectory){
			case TrajectoryType.LINE:
				if(rightIs0Degrees) {
					return new Vector3(1, 0, 0);
				} else {
					return new Vector3(-1, 0, 0);
				}
			case TrajectoryType.CONE:
				float minAngle = 0.0f;
				if(directionBasedOnShotsFired){
					minAngle = directionCurve.Evaluate(0.0f);
				} else {
					minAngle = directionMin;
				}
				minAngle *= Mathf.Deg2Rad;
				Vector3 trajectoryVector = new Vector3(Mathf.Cos(minAngle), Mathf.Sin(minAngle)).normalized;
				if(!rightIs0Degrees){
					trajectoryVector = -trajectoryVector;
				}
				return trajectoryVector;
			case TrajectoryType.HOMING:
				return Vector3.zero;
			case TrajectoryType.NONE:
				return Vector3.zero;
		}
		return Vector3.zero;
	}

	public Vector3 GetTrajectoryMax() {
		switch(trajectory) {
			case TrajectoryType.LINE:
				if(rightIs0Degrees) {
					return new Vector3(1, 0, 0);
				} else {
					return new Vector3(-1, 0, 0);
				}
			case TrajectoryType.CONE:
				float maxAngle = 0.0f;
				if(directionBasedOnShotsFired) {
					maxAngle = directionCurve.Evaluate(1.0f);
				} else {
					maxAngle = directionMin;
				}
				maxAngle *= Mathf.Deg2Rad;
				Vector3 trajectoryVector = new Vector3(Mathf.Cos(maxAngle), Mathf.Sin(maxAngle)).normalized;

				if(!rightIs0Degrees) {
					trajectoryVector = -trajectoryVector;
				}
				return trajectoryVector;
			case TrajectoryType.HOMING:
				return Vector3.one;
			case TrajectoryType.NONE:
				return Vector3.zero;
		}
		return Vector3.zero;
	}
}
