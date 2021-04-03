using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	public float speed;
	public float endX;
	private Vector3 newPos;

	// Use this for initialization
	void Start () {
		newPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.x > 0f && transform.position.x < endX || transform.position.x == 0f && Input.GetAxis ("Horizontal") > 0 || transform.position.x == endX && Input.GetAxis ("Horizontal") < 0) {
			if (Input.GetButton ("Run")) {
				newPos.x = newPos.x + Input.GetAxis ("Horizontal") * speed * Time.deltaTime * 2;
			} else {
				newPos.x = newPos.x + Input.GetAxis ("Horizontal") * speed * Time.deltaTime;
			}
		} else if (transform.position.x <= 0f) {
			newPos.x = 0f;
		} else {
			newPos.x = endX;
		}
		transform.position = newPos;
	}
}
