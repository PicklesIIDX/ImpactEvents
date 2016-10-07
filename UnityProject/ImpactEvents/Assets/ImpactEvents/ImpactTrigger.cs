using UnityEngine;

namespace PickleTools.ImpactEvents {

	public delegate void ImpactTriggerHandler(ImpactTrigger trigger);

	/// <summary>
	/// A base class that is to be extended to watch for specific conditions in your application.
	/// </summary>
	public class ImpactTrigger : MonoBehaviour {

		/// <summary>
		/// Occurs when this trigger is set to active and it has not exceeded its activation count.
		/// </summary>
		public event ImpactTriggerHandler OnTriggerActive;

		[SerializeField][Tooltip("number of times this trigger can be activated by the ImpactListener; if 0 there is " +
		                         "no limit")]
		private int timesCanBeTriggered = 1;
		private int triggerCount = 0;

		[SerializeField]
		private bool activated = false;
		public bool Activated {
			get { 
				if(!CanBeTriggered){
					return false;
				}
				return activated; 
			}
		}

		private object triggerData;
		public object TriggerData { 
			get { return triggerData; }
		}

		/// <summary>
		/// Use this to set the trigger limit in code instead of the Unity Editor.
		/// </summary>
		/// <param name="numberOfTimesCanBeTriggered">Number of times this trigger can be activated.</param>
		public void Initialize(int numberOfTimesCanBeTriggered = 1){
			timesCanBeTriggered = numberOfTimesCanBeTriggered;
		}

		/// <summary>
		/// Override this to reset any local values your trigger stores.
		/// </summary>
		public virtual void Reset() {
			activated = false;
			triggerCount = 0;
		}

		/// <summary>
		/// Typicaly this function calls Reset when disabled. Override this if you do not want your trigger to reset 
		/// when the object is disabled.
		/// </summary>
		public virtual void OnDisabled(){
			Reset();
		}

		/// <summary>
		/// Returns true if the trigger has not exceeded its activation count.
		/// </summary>
		public bool CanBeTriggered {
			get {
				if (timesCanBeTriggered > 0 && triggerCount >= timesCanBeTriggered) {
					return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Increments the amount of times this trigger has been activated. This should only be called by the
		/// ImpactListener which will call it when all triggers are simultaneously active.
		/// </summary>
		public void ActivateTrigger(){
			triggerCount++;
		}

		/// <summary>
		/// Sets this trigger to active or inactive.
		/// </summary>
		/// <param name="active">If the conditions of your trigger are true.</param>
		/// <param name="data">Generic data that will be shared with ImpactEffects through the ImpactListener.</param>
		public void SetTrigger(bool active, object data = null){
			activated = active;
			if(activated && CanBeTriggered && OnTriggerActive != null){
				triggerData = data;
				OnTriggerActive(this);
			}
		}
	}
}