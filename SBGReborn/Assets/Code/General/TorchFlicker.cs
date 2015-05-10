using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Light))]
public class TorchFlicker : MonoBehaviour {
	Light l;

	float noisePoint = 0;

	// Use this for initialization
	void Start () {
		l = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
		noisePoint += Time.deltaTime * 8;
		l.intensity = Mathf.PerlinNoise(noisePoint, 0) * 2 + 1f;
	}
}
