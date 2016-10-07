using UnityEngine;
using System.Collections.Generic;
using PickleTools.ImpactEvents;
using PickleTools.Resource;

public class ImpactEffectDamage : ImpactEffect {

	[SerializeField]
	int damage = 1;

	private readonly string IMPACT_TRIGGER_COLLIDE_WITH_LAYER = typeof(ImpactTriggerCollideWithLayers).ToString();

	public override void TriggerEffect(GameObject listener, Dictionary<string, object> triggerData) {
		GameObject target = null;
		Resource health = null;
		if (triggerData.ContainsKey(IMPACT_TRIGGER_COLLIDE_WITH_LAYER)) {
			target = triggerData[IMPACT_TRIGGER_COLLIDE_WITH_LAYER] as GameObject;
			if (target != null){
				
			}
		}
		if(health != null){
			health.Add(-damage);
		}
		TriggerComplete();
	}
}
