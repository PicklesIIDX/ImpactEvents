using UnityEngine;
using System.Collections.Generic;
using PickleTools.ImpactEvents;

public class ImpactTriggerCollideWithLayers : ImpactTrigger {

	LayerMask layers = 0;
	bool IsInLayer(int layer) {
		return (layers & (1 << layer)) > 0;
	}

	[SerializeField]
	bool trigger = false;
	[SerializeField]
	bool enter = true;
	[SerializeField]
	bool stay = true;
	[SerializeField]
	bool exit = true;

	public List<GameObject> IgnoreList = new List<GameObject>();

	void OnCollisionEnter(Collision collision){
		if(!trigger && enter && IsInLayer(collision.gameObject.layer) && !IgnoreList.Contains(collision.gameObject)){
			SetTrigger(true, collision.gameObject);
		}
	}

	void OnTriggerEnter(Collider other){
		if (trigger && enter && IsInLayer(other.gameObject.layer) && !IgnoreList.Contains(other.gameObject)) {
			SetTrigger(true, other.gameObject);
		}
	}

	void OnCollisionStay(Collision collision) {
		if (!trigger && stay && IsInLayer(collision.gameObject.layer) && !IgnoreList.Contains(collision.gameObject)) {
			SetTrigger(true, collision.gameObject);
		}
	}

	void OnTriggerStay(Collider other) {
		if (trigger && stay && IsInLayer(other.gameObject.layer) && !IgnoreList.Contains(other.gameObject)) {
			SetTrigger(true, other.gameObject);
		}
	}

	void OnCollisionExit(Collision collision) {
		if (!trigger && exit && IsInLayer(collision.gameObject.layer) && !IgnoreList.Contains(collision.gameObject)) {
			SetTrigger(true, collision.gameObject);
		}
	}

	void OnTriggerExit(Collider other) {
		if (trigger && exit && IsInLayer(other.gameObject.layer) && !IgnoreList.Contains(other.gameObject)) {
			SetTrigger(true, other.gameObject);
		}
	}

	void LateUpdate () {
		SetTrigger(false);
	}
}
