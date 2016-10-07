using UnityEngine;
using System.Collections.Generic;

namespace PickleTools.ImpactEvents {
	/// <summary>
	/// A class used to link triggers and effects. It will wait until all triggers are simultaneously active and 
	/// then fire all effects in sequence
	/// </summary>
	public class ImpactListener : MonoBehaviour {

		[SerializeField]
		ImpactTrigger[] impactTriggers = new ImpactTrigger[0];

		[SerializeField]
		ImpactEffect[] impactEffects = new ImpactEffect[0];

		bool initialized = false;

		public void Awake(){
			if(!initialized){
				initialized = true;
				for (int t = 0; t < impactTriggers.Length; t ++){
					impactTriggers[t].OnTriggerActive += HandleTriggerActive;
				}
			}
		}

		void OnDestroy(){
			for (int t = 0; t < impactTriggers.Length; t++) {
				impactTriggers[t].OnTriggerActive -= HandleTriggerActive;
			}
		}

		/// <summary>
		/// This allows us to use this class without the Unity Editor if your prefer by initializing our
		/// triggers and effects through this method.
		/// </summary>
		/// <param name="newTriggers">The triggers that all must be active to fire effects.</param>
		/// <param name="newEffects">The effects that will be fired when all triggers are active.</param>
		public void Initialize(ImpactTrigger[] newTriggers, ImpactEffect[] newEffects){
			impactTriggers = newTriggers;
			impactEffects = newEffects;
			Awake();
		}

		Dictionary<string, object> impactData = new Dictionary<string, object>();
		/// <summary>
		/// The data passed by triggers to be used by effects.
		/// </summary>
		/// <value>The impact data.</value>
		public Dictionary<string, object> ImpactData {
			get { return impactData; }
		}

		/// <summary>
		/// When a trigger is activated, this function will be called which will check all other impact triggers to
		/// see if they are active. If they are, we clear out old impact data, record the new impact data,
		/// increment the activation count of all triggers, and then perform all of the trigger effects.
		/// </summary>
		/// <param name="trigger">Trigger that initialized this call.</param>
		void HandleTriggerActive(ImpactTrigger trigger){
			for (int t = 0; t < impactTriggers.Length; t ++){
				if(!impactTriggers[t].Activated){
					return;
				}
			}

			impactData.Clear();

			for (int t = 0; t < impactTriggers.Length; t ++){
				if (impactTriggers[t].TriggerData != null) {
					impactData.Add(impactTriggers[t].GetType().ToString(), impactTriggers[t].TriggerData);
				}
				impactTriggers[t].ActivateTrigger();
			}

			for (int e = 0; e < impactEffects.Length; e ++){
				impactEffects[e].TriggerEffect(gameObject, impactData);
			}
		}
	}
}