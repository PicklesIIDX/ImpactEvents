using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using PickleTools.ImpactEvents;

public class ImpactEffectSetVelocity : ImpactEffect {

	[SerializeField]
	float xSpeed = 0.0f;
	[SerializeField]
	float ySpeed = 0.0f;
	MoveOnVelocity moveOnVelocity;


	void Awake(){
		moveOnVelocity = GetComponent<MoveOnVelocity>();
		Assert.IsNotNull(moveOnVelocity);
	}
	
	public override void TriggerEffect(GameObject listener, Dictionary<string, object> triggerData) {
		moveOnVelocity.Initialize(xSpeed, ySpeed);
		TriggerComplete();
	}
}
