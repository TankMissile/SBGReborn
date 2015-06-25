using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Light))]
public class TorchFlicker : MonoBehaviour {
	Light l;

	float noisePoint = 0;

	// Use this for initialization
	void Start () {
		l = GetComponent<Light>();
		noisePoint = Random.Range(1,100000);
	}
	
	// Update is called once per frame
	void Update () {
		noisePoint += Time.deltaTime * Random.Range(8,10);
		l.intensity = Mathf.PerlinNoise(noisePoint, 0) * 2 + 1f;
	}
}
