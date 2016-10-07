using UnityEngine;
using System.Collections;
using PickleTools.ImpactEvents;

public class ImpactTriggerChangeState : ImpactTrigger {

	[SerializeField]
	bool activeOnEnable = true;
	[SerializeField]
	bool activeOnDisable = false;

	void OnEnable(){
		if (activeOnEnable) {
			SetTrigger(true);
		}
	}

	void OnDisable(){
		if (activeOnDisable) {
			SetTrigger(true);
		}
	}

	void LateUpdate(){
		SetTrigger(false);
	}
}
