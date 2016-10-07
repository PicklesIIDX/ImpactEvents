using UnityEngine;
using System.Collections.Generic;
using PickleTools.ImpactEvents;

public class ImpactEffectChangeSpeed : ImpactEffect {

	[SerializeField]
	float startSetXSpeed = 0.0f;
	[SerializeField]
	float startSetYSpeed = 0.0f;
	[SerializeField]
	float endSetXSpeed = 0.0f;
	[SerializeField]
	float endSetYSpeed = 0.0f;

	[SerializeField]
	MoveOnController moveOnController;
	[SerializeField]
	float tweenTime = 0.0f;
	float currentTweenTime = 0.0f;
	[SerializeField]
	AnimationCurve tweenCurve;
	bool tweening = false;

	[SerializeField]
	bool startWithCurrentSpeed = false;

	public override void TriggerEffect(GameObject listener, Dictionary<string, object> triggerData) {
		if(tweenTime <= 0.0f){
			moveOnController.Initialize(endSetXSpeed, endSetYSpeed);
			TriggerComplete();
		} else {
			tweening = true;
			if(startWithCurrentSpeed){
				startSetXSpeed = moveOnController.XSpeed;
				startSetYSpeed = moveOnController.YSpeed;
			}
		}
	}

	void OnDisable(){
		currentTweenTime = 0.0f;
	}

	public void Update(){
		if(tweening){
			currentTweenTime += Time.deltaTime;
			if(currentTweenTime > tweenTime){
				currentTweenTime = tweenTime;
			}
			float percentage = tweenCurve.Evaluate(currentTweenTime / tweenTime);
			float newXSpeed = startSetXSpeed + (endSetXSpeed - startSetXSpeed) * percentage;
			float newYSpeed = startSetYSpeed + (endSetYSpeed - startSetYSpeed) * percentage;
			moveOnController.Initialize(newXSpeed, newYSpeed);

			if(currentTweenTime == tweenTime){
				tweening = false;
				moveOnController.Initialize(endSetXSpeed, endSetYSpeed);
				TriggerComplete();
			}
		}
	}
}
