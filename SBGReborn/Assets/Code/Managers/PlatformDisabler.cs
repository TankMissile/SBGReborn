using UnityEngine;
using System.Collections;

public class PlatformDisabler : MonoBehaviour {
	public Collider2D box;

	float timeToDisable = -1;

	void Update () {
		if(Input.GetButtonDown("Crouch")){
			timeToDisable = .3f;
		}
		else if(Input.GetButtonUp("Crouch")){
			box.enabled = true;
			timeToDisable = -1;
		}
		
		
		if(timeToDisable > 0){
			timeToDisable -= Time.deltaTime;
			if(timeToDisable <= 0){
				box.enabled = false;
			}
		}
	}
}
