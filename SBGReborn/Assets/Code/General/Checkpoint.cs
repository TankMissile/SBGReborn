using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D coll){
		if(coll.gameObject.tag.Equals ("Player")){
			GameManager.setSpawnPoint(this.transform.position);
		}
	}
}
