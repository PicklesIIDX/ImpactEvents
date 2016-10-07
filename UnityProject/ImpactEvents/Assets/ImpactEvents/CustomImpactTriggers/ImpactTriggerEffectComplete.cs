using UnityEngine;
using System.Collections;
using PickleTools.ImpactEvents;

public class ImpactTriggerEffectComplete : ImpactTrigger {

	[SerializeField]
	ImpactEffect effect;

	// Use this for initialization
	void Start () {
		effect.OnTriggerComplete += HandleTriggerComplete;
	}

	void OnDestroy(){
		effect.OnTriggerComplete -= HandleTriggerComplete;
	}

	void HandleTriggerComplete(ImpactEffect effectComplete){
		SetTrigger(true, effectComplete);
	}

	void LateUpdate () {
		SetTrigger(false);
	}
}
