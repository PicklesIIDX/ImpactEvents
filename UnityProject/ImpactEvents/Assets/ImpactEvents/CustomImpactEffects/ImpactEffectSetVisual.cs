using UnityEngine;
using System.Collections.Generic;
using PickleTools.ImpactEvents;

public class ImpactEffectSetVisual : ImpactEffect {

	[SerializeField]
	GameObject[] sprites = new GameObject[0];
	[SerializeField]
	bool enableSprites = false;

	void Awake(){
	}

	void OnDisable(){
		for(int s = 0; s < sprites.Length; s++) {
			sprites[s].gameObject.SetActive(true);
		}
	}

	public override void TriggerEffect(GameObject listener, Dictionary<string, object> triggerData) {
		for(int s = 0; s < sprites.Length; s ++){
			sprites[s].gameObject.SetActive(enableSprites);
		}
		TriggerComplete();
	}
}
