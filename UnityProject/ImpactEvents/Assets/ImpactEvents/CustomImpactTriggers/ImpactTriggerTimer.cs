using UnityEngine;
using System.Collections;
using PickleTools.ImpactEvents;
using PickleTools;

public class ImpactTriggerTimer : ImpactTrigger {

	[SerializeField]
	float timerDuration = 1.0f;
	[SerializeField]
	bool repeat = true;

	UpdateTimer updateTimer;

	void Awake(){
		updateTimer = new UpdateTimer(0.0f);
		updateTimer.TimerComplete += HandleTimerComplete;
	}

	void OnDestroy(){
		updateTimer.TimerComplete -= HandleTimerComplete;
	}

	void OnEnable() {
		updateTimer.Reset(timerDuration);
	}

	void HandleTimerComplete(UpdateTimer timer){
		SetTrigger(true);
		if(repeat){
			updateTimer.Reset(timerDuration);
		}
	}
		
	void LateUpdate () {
		if(repeat){
			SetTrigger(false);
		}
	}
}
