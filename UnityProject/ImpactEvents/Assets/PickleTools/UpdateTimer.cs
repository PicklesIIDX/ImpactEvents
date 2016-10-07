using UnityEngine;
using System.Collections;

namespace PickleTools{

	public delegate void TimerHandler (UpdateTimer sender);

	public class UpdateTimer {

		public event TimerHandler TimerComplete;

		private float duration = 0.0f;
		public float Duration {
			get { return duration; }
		}
		private float currentTime = 0.0f;
		public float CurrentTime {
			get { return duration - currentTime; }
		}

		public UpdateTimer(float newDuration){
			Reset(newDuration);
		}

		public void Reset(float newDuration = -1.0f){
			if(newDuration == -1.0f){
				newDuration = duration;
			}
			duration = newDuration;
			currentTime = duration;
		}

		public bool IsRunning(){
			return currentTime != 0.0f;
		}

		public void Update (float deltaTime) {
			if(currentTime > 0.0f){
				currentTime -= deltaTime;
				if(currentTime <= 0.0f){
					currentTime = 0.0f;
					if(TimerComplete != null){
						TimerComplete(this);
					}
				}
			}
		}

		public override string ToString ()
		{
			return string.Format ("[UpdateTimer: CurrentTime={0}]", CurrentTime);
		}
	}
}