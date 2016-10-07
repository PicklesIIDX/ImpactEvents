using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using PickleTools.ImpactEvents;

public class ImpactEffectChangeState : ImpactEffect {

	[SerializeField]
	bool enableObject = false;
	[SerializeField]
	bool disableObject = true;


	public override void TriggerEffect(GameObject listener, Dictionary<string, object> triggerData) {
		//Debug.Log(ToString());
		if(enableObject){
			gameObject.SetActive(true);
		} else if (disableObject){
			gameObject.SetActive(false);
		}
		TriggerComplete();
	}

}
