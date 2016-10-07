using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pool {

	List<GameObject> poolList = new List<GameObject>();
	GameObject prefab;
	int poolSize = 0;
	string name = "none";

	public Pool(string ownerName, GameObject newPrefab, int newSize){
		name = ownerName;
		prefab = newPrefab;
		poolSize = newSize;
		for(int o = 0; o < poolSize; o ++){
			Create();
			poolList[poolList.Count - 1].SetActive(false);
		}
	}

	public GameObject GetNext() {
		for(int o = 0; o < poolList.Count; o++) {
			if(!poolList[o].activeInHierarchy) {
				poolList[o].SetActive(true);
				return poolList[o];
			}
		}
		Debug.LogWarning("[Pool.cs]: " + name + "'s " + prefab.name + " limit reached! (" + poolList.Count + "/" + poolSize +")");
		return Create();
	}

	GameObject Create() {
		poolList.Add(GameObject.Instantiate(prefab));
		poolList[poolList.Count - 1].name = prefab.name + "_owned_by_" + name;
		return poolList[poolList.Count - 1];
	}

	public void ApplyActionToAllObjects(System.Action<GameObject> action){
		for(int o = 0; o < poolList.Count; o ++){
			action(poolList[o]);
		}
	}
}
