using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateClouds : MonoBehaviour {

	public float maxX;
	public Transform cloudObj;
	private Transform controllerT;
	private float maxClouds;

	// Use this for initialization
	void Start () {
		maxClouds = Mathf.Round (maxX / 3.0f);
		controllerT = GetComponent<Transform> ();
		for (int i = 0; i < maxClouds; i++) {
			Instantiate (cloudObj, new Vector3 (maxX, 0.0f, 0.0f), new Quaternion (0.0f, 0.0f, 0.0f, 0.0f), controllerT);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
