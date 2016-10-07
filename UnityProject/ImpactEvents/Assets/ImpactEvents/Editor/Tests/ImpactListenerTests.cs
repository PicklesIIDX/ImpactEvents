using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using PickleTools.ImpactEvents;

public class ImpactListenerTests {

	ImpactTrigger trigger;
	ImpactEffect effect;
	ImpactListener listener;
	ImpactEffectHandler effectHandler;
	bool effectTriggered = false;

	[SetUp]
	public void SetUp() {
		GameObject triggerObject = new GameObject("_test_trigger");
		trigger = triggerObject.AddComponent<ImpactTrigger>();
		trigger.Initialize(2);


		GameObject effectObject = new GameObject("_test_effect");
		effect = effectObject.AddComponent<ImpactEffect>();
		effect.Initialize(true);
		effectTriggered = false;
		effectHandler = delegate (ImpactEffect impactEffect) {
			effectTriggered = true;
		};
		effect.OnTriggerComplete += effectHandler;

		GameObject listenerObject = new GameObject("_test_listener");
		listener = listenerObject.AddComponent<ImpactListener>();
		listener.Initialize(new ImpactTrigger[1] { trigger }, new ImpactEffect[1] { effect });

	}

	[TearDown]
	public void TearDown(){
		GameObject.DestroyImmediate(trigger.gameObject);
		effect.OnTriggerComplete -= effectHandler;
		GameObject.DestroyImmediate(effect.gameObject);
		GameObject.DestroyImmediate(listener.gameObject);
	}

	[Test]
	public void EffectsFireOnTriggerActive()
	{
		trigger.SetTrigger(true);
		Assert.IsTrue(effectTriggered);
	}
	[Test]
	public void EffectsDoNotFireOnTriggerInactive() {
		trigger.SetTrigger(false);
		Assert.IsTrue(!effectTriggered);
	}
	[Test]
	public void DataClearsOnTriggerStart() {
		trigger.Initialize(3);
		trigger.SetTrigger(true);
		Assert.IsTrue(listener.ImpactData.Count == 0);
		trigger.SetTrigger(true, 1);
		Assert.IsTrue(listener.ImpactData.Count == 1);
		trigger.SetTrigger(true);
		Assert.IsTrue(listener.ImpactData.Count == 0);

	}
}
