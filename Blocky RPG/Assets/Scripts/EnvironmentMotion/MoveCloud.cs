using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCloud : MonoBehaviour {

	private float maxX;
	private float speed;
	private Vector3 newPos;

	// Use this for initialization
	void Start () {
		maxX = transform.position.x;
		newPos = new Vector3 (Random.Range (-6.5f, maxX), Random.Range(0.0f, 3.5f), 0.0f);
		speed = Random.Range (1.0f, 3.0f);
		transform.position = newPos;
	}
	
	// Update is called once per frame
	void Update () {
		newPos.x = newPos.x + speed * Time.deltaTime;
		if (newPos.x > maxX) {
			speed = Random.Range (1.0f, 3.0f);
			newPos = new Vector3 (-6.5f, Random.Range(0.0f, 3.5f), 0.0f);
		}
		transform.position = newPos;
	}
}
