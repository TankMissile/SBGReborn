using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

	public Camera perspectiveCamera, angledOrthoCamera, horizOrthoCamera;

	public enum CameraState { PERSPECTIVE, ANGLEDORTHO, HORIZORTHO };
	public CameraState cState = CameraState.PERSPECTIVE;
	
	public float speedModifier = 1;
	
	public float minZoom = 100, maxZoom = 30, currentZoom = 55;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("CameraSwitch")){
			switch(cState){
			case CameraState.PERSPECTIVE:
				angledOrthoCamera.enabled = true;
				perspectiveCamera.enabled = false;
				cState = CameraState.ANGLEDORTHO;
				break;
			case CameraState.ANGLEDORTHO:
				horizOrthoCamera.enabled = true;
				angledOrthoCamera.enabled = false;
				cState = CameraState.HORIZORTHO;
				break;
			case CameraState.HORIZORTHO:
				perspectiveCamera.enabled = true;
				horizOrthoCamera.enabled = false;
				cState = CameraState.PERSPECTIVE;
				break;
			}
		}
		
		if(Input.GetAxis ("CameraScroll") != 0){
			currentZoom -= Input.GetAxis("CameraScroll");
			if(currentZoom > minZoom){
				currentZoom = minZoom;
			}
			else if(currentZoom <= maxZoom){
				currentZoom = maxZoom;
			}
			
			perspectiveCamera.fieldOfView = currentZoom;
			angledOrthoCamera.orthographicSize = currentZoom/10;
			horizOrthoCamera.orthographicSize = currentZoom/10;
		}
		
		transform.position = Vector2.MoveTowards(transform.position, GameManager.getPlayer().transform.position, Vector3.Distance(transform.position, GameManager.getPlayer().transform.position) * speedModifier/40);
	}
}
