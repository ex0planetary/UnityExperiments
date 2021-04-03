using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateBirds : MonoBehaviour {

	public float maxX;
	public Transform birdObj;
	private Transform controllerT;
	private float flockSpeed;

	// Use this for initialization
	void Start () {
		flockSpeed = Random.Range (1.0f, 3.0f);
		controllerT = GetComponent<Transform> ();
		for (int i = 0; i < 6; i++) {
			Instantiate (birdObj, new Vector3 (maxX, flockSpeed, 0.0f), new Quaternion (0.0f, 0.0f, 0.0f, 0.0f), controllerT);
		}
	}

	// Update is called once per frame
	void Update () {

	}
}
