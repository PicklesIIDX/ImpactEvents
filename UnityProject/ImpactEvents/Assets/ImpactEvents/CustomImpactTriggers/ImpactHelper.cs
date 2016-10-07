using UnityEngine;
using System.Collections;

public delegate void CollideHandler(GameObject other);

public class ImpactHelper : MonoBehaviour {
	
	[SerializeField]
	bool collisions = true;
	[SerializeField]
	bool triggers = true;

	[SerializeField]
	bool enter = true;
	public event CollideHandler OnCollideEnter;
	[SerializeField]
	bool stay = false;
	public event CollideHandler OnCollideStay;
	[SerializeField]
	bool exit = false;
	public event CollideHandler OnCollideExit;

	void OnCollisionEnter(Collision other){
		if(collisions && enter && OnCollideEnter != null) {
			OnCollideEnter(other.collider.gameObject);
		}
	}

	void OnCollisionStay(Collision other) {
		if(collisions && stay && OnCollideStay != null) {
			OnCollideStay(other.collider.gameObject);
		}
	}

	void OnCollisionExit(Collision other) {
		if(collisions && exit && OnCollideExit != null) {
			OnCollideExit(other.collider.gameObject);
		}
	}

	void OnTriggerEnter(Collider other){
		if(triggers && enter && OnCollideEnter != null) {
			OnCollideEnter(other.gameObject);
		}
	}

	void OnTriggerStay(Collider other) {
		if(triggers && stay && OnCollideStay != null) {
			OnCollideStay(other.gameObject);
		}
	}

	void OnTriggerExit(Collider other) {
		if(triggers && exit && OnCollideExit != null) {
			OnCollideExit(other.gameObject);
		}
	}
}
