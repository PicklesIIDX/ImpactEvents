using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using PickleTools.ImpactEvents;

public class ImpactTriggerTests {

	[Test]
	public void EditorTest()
	{
		GameObject triggerObject = new GameObject("_test_trigger");
		ImpactTrigger trigger = triggerObject.AddComponent<ImpactTrigger>();
		trigger.Initialize(1);

		trigger.SetTrigger(true);
		Assert.IsTrue(trigger.Activated);
		trigger.ActivateTrigger();
		Assert.IsTrue(!trigger.Activated);

		trigger.Initialize(0);
		for (int i = 0; i < 99; i ++){
			trigger.SetTrigger(true);
			trigger.ActivateTrigger();
			Assert.IsTrue(trigger.Activated);
			trigger.SetTrigger(false);
		}
	}
}
