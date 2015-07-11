using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D coll){
		if(coll.gameObject == GameManager.getPlayer().gameObject){
			GameManager.addCollectible();
			Destroy (gameObject);
		}
	}
}
