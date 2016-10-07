# ImpactEvents
An extendable component based trigger/event system for Unity. Ideal for responding to small scale conditions in gameplay.

# Installation
Download the .unitypackage from the Releases tab at https://github.com/PicklesIIDX/ImpactEvents and import into your project.

# Overview
This system was original built to solve a problem where I wanted bullets fired from weapons to change dynamically based on certain conditions over its lifetime. After building the system I realized that it was a fairly generic trigger/event system that could easily be used in many other scenarios. 

This system takes advantage of Unity's component system to allow you to build up complex responses to multiple triggers. Each of the triggers and responses are extended off of base classes from this system. This system focuses on making triggers and events easily extendable so you can easily build quick conditions and responses to your application. Additionally, it focuses on keeping each individual trigger and event separate so that it's easy to define very specific scenarios for your application to respond to. Finally, by using components, it encourages developers familiar with Unity the ability to create complex responses to specific conditions without having to touch any scripting after a programmer has implemented the necessary components.

# Use in the Editor

 1. Add an ImpactListener component to a GameObject.
 2. Add any number of ImpactTriggers as components to the original GameObject or other GameObjects.
 3. Link those ImpactTriggers to the ImpactListener of the first object.
 4. Add any number of ImpactEffects as components to the original GameObject or other GameObjects.
 5. Link those ImpactEffects to the ImpactListener of the first object.
 6. When the conditions of all the linked ImpactTriggers are simultaneously true, all of the ImpactEffects's triggers will be fired.

# ImpactListener

This component is the link between your triggers and effects. In the editor, you'll use the inspector to assign ImpactTriggers to the Impact Triggers list and ImpactEffects to the Impact Effects list. Remember that all of the ImpactTriggers must simultaneously be active for the ImpactEffects to be activated. And when ImpactEffects are activated, they are all done so on the same frame, in the order they appear in the array.

Tips:
 * Use a different ImpactListener for each combination of triggers and effects you want. For example, one impact listener could be used for playing an animation when taking a small amount of damage, and another impact listener could be used for playing a different animation when taking specific types of damage.

# ImpactTrigger

This component is what you will extend to implement your application's custom triggers. When extending this the important method to use is the SetTrigger() method. This is what you will call when the conditions of your trigger are true. For example, if your trigger is watching for collisions on a specific layer you may have something like this:

```csharp
using PickleTools.ImpactEvents;

public class ImpactTriggerMyTrigger : MonoBehaviour, ImpactTrigger {

	public LayerMask layersToCollide;

	// Use Unity physics to detect a collision
	public void OnCollisionEnter(Collision collision){
		// if the colliding object is one of our layers to collide with
		if((layersToCollide & (1 << collision.gameObject.layer)) > 0){
			// set this trigger to true and send the colliding game object as data
			SetTrigger(true, collision.gameObject);
		}
	}

	public void LateUpdate(){
		// we can disable the trigger, because the collision should
		// only trigger this for one frame
		SetTrigger(false);
	}
}
```

When calling SetTrigger() you pass in the new state of this trigger. If true, it will be active which will call the OnTriggerActive() event and tell the ImpactListener to check all other triggers to see if they are all true. If false, the state of the trigger will no longer be active. This is not updated automatically, so make sure you code describes when to disable this trigger.

The second optional parameter of SetTrigger() is a generic object which will hold any data. This is useful if you want your ImpactEffects to use information received in the ImpactTrigger. For example, after colliding with an object we may want to deal damage to it so we need to pass that object to get the relevant information from it.

After you have created your own custom ImpactTriggers, add them as components to GameObjects to link them up to your ImpactListener in the editor.

Tips
 * ImpactTriggers should be individual conditions. Appropriate triggers would be "can I see an ally," or "is my health critical?" Things that are specific situations that you want to respond to. There are few triggers that would be inappropriate, but try to break down your triggers into single questions. If a trigger is only activated when it collides and 5 seconds have elapsed, then that should likely be two separate triggers.
 * ImpactTriggers don't have to be on the same GameObject as the ImpactListener. In fact, if your object deals with many complex sequences, it can be useful to have ImpactTriggers on child GameObjects for the sake of organization.
 * If you want a trigger to be true as long as it has been true at least once, then do not set the trigger false.
 * ImpactTriggers have a virtual Reset() method which will reset the ImpactTrigger to no longer be active and reset the number of times it has been triggered. This is called OnDisable(). You can override this method (or OnDisable()) if you do not want this behaviour.
 * ImpactTriggers will increment the number of times they have been triggered only when all triggers of an ImpactListener are active. If you want to increment the counter otherwise, you can call the TriggerActivated() method.

# ImpactEffect

This component is what you will extend to implement specific events in your application. There are two methods you'll be using. First is TriggerEffect(), which you will have to override. This method is called by the ImpactListener when all of its ImpactTriggers are simultaneously active. This is where you application code goes for whatever you want to happen. This method will receive two parameters, the GameObject of the ImpactListener, and a dictionary of strings and objects which is filled with the second parameter of the ImpactListener SetTrigger() method. 

Let's look at an example where we want to deal damage to an object we collided with. This will continue the previous ImpactTrigger example that sent the colliding game object as data.

```csharp
	using PickleTools.ImpactEvents;

	public class ImpactEffectMyEffect : MonoBehaviour, ImpactEffect{

		public int damageToDeal = 5;

		public override void TriggerEffect(GameObject self, Dictionary<string, object> triggerData){
			// get the object we collided with from the triggerData.
			// it was stored with the ImpactTriggerMyTrigger, so we can use that name as the key
			if (triggerData.ContainsKey[typeof(ImpactTriggerMyTrigger).ToString()]) {
				GameObject collidedObject = triggerData[typeof(ImpactTriggerMyTrigger).ToString()] as GameObject;
				if(collidedObject != null){
					collidedObject.GetComponent<MyHealthClass>.TakeDamage(damageToDeal);
				}
			}
			TriggerComplete();
		}
	}
```

Here we overrode the TriggerEffect(), and used the triggerData to find any data that the ImpactTriggerMyTrigger could have sent over. Because we have authored both of these classes, we know what we are looking for, which is a GameObject. Once we get the GameObject, we will perform the effect of this trigger, which is to deal some damage through a component we expect that object to have.

Finally let's take a look at the second method, TriggerComplete(). Call this when you have finished performing the TriggerEffect(). This will fire the event OnEffectComplete(), which can be used as a handy callback.

After you have extended this class with triggers relevant to your application, you will add these components on to GameObjects in the editor and link them to the ImpactListener. They will call the TriggerEffect() method in the order they are organized in the Trigger Effects array on the ImpactListener when all of the ImpactTriggers are active.

Tips:
 * ImpactEffects should be very specific individual game events. The idea is to have many of these effects that can be reused all over the application. If you have any effect that is performing multiple tasks, unless that is a specific sequence that happens frequently in your application, you'll get more creativity on the content side if your effects only perform small specific functions.
 * Like ImpactTriggers, if you have a complex object with multiple ImpactListeners, it can be helpful to group TriggerEffects on child objects in the editor for organization.
 * By default, ImpactTriggers will not call OnEffectComplete() when TriggerComplete() is called. That must be enabled in the editor through the announceTriggerComplete toggle. 


# What should I use this for?

 I find this system useful for the following scenarios:

 * A relatively closed system that is related, but not tightly coupled.
 * Systems that require lots of content with variation
 * As a tool for non technical team members to create content

# What should I not use this for?

 I think this system would start becoming cumbersome in the following scenarios:

 * A way to respond to everything your application can do. This system is good at reacting, but not managing state.
 * Responding to scenarios that require triggers to be activated in different modes of the application. Although one could have Triggers keep their state active, it would become very easy to loose track of what original caused the activation.
 * Anything that has very simple triggers or very general responses. This system shows its power when multiple unrelated triggers must all be true in order to trigger a specific response. Otherwise, other trigger/event systems may be easier to work with.
 * Systems with very little content. It takes awhile to set up triggers and events in the editor. If you don't plan on creating a lot of content over the life of the application, it may be easier to code something more specific instead of using Unity's editor to link everything.