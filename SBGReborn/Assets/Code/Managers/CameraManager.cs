using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

	public Camera perspectiveCamera, angledOrthoCamera, horizOrthoCamera;

	public enum CameraState { PERSPECTIVE, ANGLEDORTHO, HORIZORTHO };
	public CameraState cState = CameraState.PERSPECTIVE;
	
	// Use this for initialization
	void Start () {
	
	}
	
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
	}
}
