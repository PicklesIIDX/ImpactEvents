using UnityEngine;
using System.Collections.Generic;
using PickleTools;
using UnityEngine.Assertions;
using PickleTools.ImpactEvents;

public class ImpactEffectWeapon:ImpactEffect {

	[SerializeField]
	GameObject weaponPrefab;
	WeaponController weaponInstance;
	[SerializeField]
	int fireCount = 1;
	int timesFired = 0;
	[SerializeField]
	float[] delaySequence = new float[0];
	int delayIndex = 0;
	UpdateTimer updateTimer;
	Vector3 impactPoint = Vector3.zero;
	[SerializeField]
	bool ignorePreviousTargets = false;
	List<GameObject> ignoreList = new List<GameObject>();
	[SerializeField]
	WeaponController.FireOptions fireOptions;
	MoveOnController moveOnController;

	[SerializeField]
	bool fireOnInput = false;

	[SerializeField]
	bool fireInOppositeDirectionMoved = false;
	Vector3 previousPosition = Vector3.zero;
	Vector3 velocity = new Vector3(1, 0, 0);

	private readonly string IMPACT_TRIGGER_COLLIDE_WITH_LAYER = typeof(ImpactTriggerCollideWithLayers).ToString();

	void Awake(){
		updateTimer = new UpdateTimer(0.0f);
		updateTimer.TimerComplete += HandleTimerComplete;
		moveOnController = GetComponent<MoveOnController>();
	}

	void Start(){
		weaponInstance = Instantiate(weaponPrefab.GetComponent<WeaponController>());
		weaponInstance.CanUseInput = fireOnInput;
		weaponInstance.transform.SetParent(transform);
		weaponInstance.transform.localPosition = Vector3.zero;
		previousPosition = transform.position;
	}

	void OnDestroy(){
		updateTimer.TimerComplete -= HandleTimerComplete;
	}

	void OnDisable(){
		updateTimer.Reset(0.0f);
		ignoreList.Clear();
	}

	void Update(){
		updateTimer.Update(Time.deltaTime);
		if(transform.position != previousPosition) {
			velocity = (transform.position - previousPosition).normalized;
			previousPosition = transform.position;
		}
	}

	public override void TriggerEffect(GameObject listener, Dictionary<string, object> triggerData) {
		GameObject target = null;
		if (triggerData.ContainsKey(IMPACT_TRIGGER_COLLIDE_WITH_LAYER)){
			target = triggerData[IMPACT_TRIGGER_COLLIDE_WITH_LAYER] as GameObject;
		}
	    if(fireOptions.overridePosition && target != null) {
			impactPoint = target.transform.position;
		} else {
			impactPoint = Vector3.zero;
		}
		if(ignorePreviousTargets){
			ImpactTriggerCollideWithLayers collideWithLayers = target.GetComponent<ImpactTriggerCollideWithLayers>();
			ignoreList.Add(target);
			collideWithLayers.IgnoreList.AddRange(ignoreList);
			fireOptions.targets = ignoreList.ToArray();
			if(fireOptions.overridePosition){
				fireOptions.offsetX = target.transform.position.x;
				fireOptions.offsetY = target.transform.position.y;
			}
		}
		timesFired = 0;
		delayIndex = 0;
		// fire on this frame
		HandleTimerComplete(updateTimer);
	}

	void HandlePlayerInputMessage(bool justPressed = false, bool isPressing = false, bool justReleased = false){
		if(justPressed && !updateTimer.IsRunning()){
			timesFired = 0;
			delayIndex = 0;
			HandleTimerComplete(updateTimer);
		}
	}

	void HandleTimerComplete(UpdateTimer timer){
		if(timesFired < fireCount){ 
			// check assumptions
			if(!fireOptions.overridePosition) { Assert.IsTrue(impactPoint.Equals(Vector3.zero)); }
			// override the shot direction with the speed of this bullet
			if(fireOptions.overrideDirection) {
				if(fireInOppositeDirectionMoved) {
					fireOptions.xSpeed = -velocity.x * weaponInstance.Speed;
					fireOptions.ySpeed = -velocity.y * weaponInstance.Speed;
				} else {
					fireOptions.xSpeed = moveOnController.XSpeed;
					fireOptions.ySpeed = moveOnController.YSpeed;
				}
			}
			// fire the weapon
			weaponInstance.Fire(fireOptions);
			timesFired++;
			// delay before we fire again
			float delay = GetDelay();
			if(delay > 0.0f) {
				updateTimer.Reset(delay);
			} else {
				HandleTimerComplete(timer);
			}
		} 
		if(timesFired >= fireCount){
			TriggerComplete();
		}
	}

	float GetDelay(){
		float delay = 0.0f;
		if(delaySequence.Length > 0) {
			delay = delaySequence[delayIndex];
			delayIndex++;
			if(delayIndex >= delaySequence.Length) {
				delayIndex = 0;
			}
		}
		return delay;
	}
}
