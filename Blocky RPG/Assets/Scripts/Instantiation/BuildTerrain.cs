using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTerrain : MonoBehaviour {

	public Transform GroundPref;
	public Transform BGPref;
	public float screens;
	private Transform controllerT;
	// Use this for initialization
	void Start () {
		controllerT = GetComponent<Transform> ();
		for (int i = 0; i < screens; i++) {
			Instantiate(GroundPref, new Vector3 (12 * i, -3.5f, 0.0f), new Quaternion (0.0f,0.0f,0.0f,0.0f), controllerT);
		}
		for (int i = 0; i < screens; i++) {
			Instantiate(BGPref, new Vector3 (12 * i, 0.0f, 0.0f), new Quaternion (0.0f,0.0f,0.0f,0.0f), controllerT);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
