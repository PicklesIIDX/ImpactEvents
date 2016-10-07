using UnityEngine;
using System.Collections.Generic;
using PickleTools.ImpactEvents;

public class ImpactEffectSetColliders : ImpactEffect {

	Collider[] colliders = new Collider[0];
	[SerializeField]
	bool enableColliders = false;

	// Use this for initialization
	void Start () {
		colliders = GetComponentsInChildren<Collider>();
		Collider baseCollider = GetComponent<Collider>();
		if(baseCollider != null){
			System.Array.Resize(ref colliders, colliders.Length + 1);
			colliders[colliders.Length - 1] = baseCollider;
		}
	}

	void OnDisable(){
		for(int c = 0; c < colliders.Length; c++) {
			colliders[c].enabled = true;
		}
	}


	public override void TriggerEffect(GameObject listener, Dictionary<string, object> triggerData) {
		for(int c = 0; c < colliders.Length; c ++){
			colliders[c].enabled = enableColliders;
		}
		TriggerComplete();
	}
}
