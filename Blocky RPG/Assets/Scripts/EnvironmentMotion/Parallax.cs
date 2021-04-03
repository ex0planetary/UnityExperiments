using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {

	private Transform camTransform;

	public float parallaxAmount = 0.25f;
	private float initX;
	private float initCamX;
	private float camXChange;
	private Vector3 currentPos;

	// Use this for initialization
	void Start () {
		camTransform = Camera.main.GetComponent<Transform> ();
		initX = transform.position.x;
		initCamX = camTransform.position.x;
		currentPos = transform.position;
	}

	// Update is called once per frame
	void Update () {
		camXChange = camTransform.position.x - initCamX;
		currentPos.x = parallaxAmount * camXChange + initX;
		transform.position = currentPos;
	}
}
