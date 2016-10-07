using UnityEngine;
using System.Collections.Generic;

namespace PickleTools.ImpactEvents {

	public delegate void ImpactEffectHandler(ImpactEffect effect);

	/// <summary>
	/// A base class to be extended to perform specific responses in your application.
	/// </summary>
	public class ImpactEffect : MonoBehaviour {

		/// <summary>
		/// Occurs when on trigger finishes its effect.
		/// </summary>
		public event ImpactEffectHandler OnTriggerComplete;
		[SerializeField][Tooltip("set to true if this should callback after it completes")]
		bool announceTriggerComplete = false;

		/// <summary>
		/// Use this if you are creating ImpactEffects in code instead of the Unity Editor to enable callbacks.
		/// </summary>
		/// <param name="announceTriggerOnComplete">Announce trigger on complete.</param>
		public void Initialize(bool announceTriggerOnComplete = false){
			announceTriggerComplete = announceTriggerOnComplete;
		}

		/// <summary>
		/// A method to be overriden which will start your effect when the ImpactListener's triggers are all 
		/// simultaneously active
		/// </summary>
		/// <param name="listener">The ImpactListener GameObject that called this effect.</param>
		/// <param name="triggerData">A collection of data from the ImpactTriggers. The keys are strings of the 
		/// Triggers's class name.</param>
		public virtual void TriggerEffect(GameObject listener, Dictionary<string, object> triggerData) {
			Debug.LogWarning("[ImpactEffect.cs]: " + name + " has triggered the impact effect!");
			TriggerComplete();
		}

		protected void TriggerComplete() {
			if (announceTriggerComplete && OnTriggerComplete != null) {
				OnTriggerComplete(this);
			}
		}
	}

}