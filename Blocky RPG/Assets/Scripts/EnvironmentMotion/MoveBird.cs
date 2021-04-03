using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBird : MonoBehaviour {

	private float maxX;
	private float speed;
	private Vector3 newPos;

	// Use this for initialization
	void Start () {
		maxX = transform.position.x;
		speed = transform.position.y;
		newPos = new Vector3 (Random.Range (-6.5f, -4.5f), Random.Range(2.5f, 4.5f), 0.0f);
		transform.position = newPos;
	}

	// Update is called once per frame
	void Update () {
		newPos.x = newPos.x + speed * Time.deltaTime;
		if (newPos.x > maxX) {
			newPos = new Vector3 (-6.5f, Random.Range(2.5f, 4.5f), 0.0f);
		}
		transform.position = newPos;
	}
}
