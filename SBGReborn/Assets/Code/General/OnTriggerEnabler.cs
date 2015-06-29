using UnityEngine;
using System.Collections;

public class OnTriggerEnabler : MonoBehaviour {

	Transform[] children;
	
	// Use this for initialization
	void Start () {
		children = GetComponentsInChildren<Transform>();
		foreach(Transform t in children){
			if(t != this.transform)
				t.gameObject.SetActive(false);
		}
	}
	
	void OnTriggerEnter2D(Collider2D coll){
		
		if(coll.tag != "Player") return;
		
		foreach(Transform t in children){
			if(t != this.transform)
				t.gameObject.SetActive(true);
		}
	}
	
	void OnTriggerExit2D(Collider2D coll){
	
		if(coll.tag != "Player" && coll.tag != "FriendlyAttack") return;
		
		foreach(Transform t in children){
			if(t != this.transform)
				t.gameObject.SetActive(false);
		}
	}
}
