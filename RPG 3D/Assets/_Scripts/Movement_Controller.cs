using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Controller : MonoBehaviour {

	// Initial variable declaration
	public float speed;
	public float jumpForce;
	public Rigidbody charRB;
	public bool canJump;
	public float maxVelocity;

	// Initialization
	void Start () {
		// Get charRB component
		charRB = GetComponent<Rigidbody>();
	}

	// Call every update
	void Update () {
		// Manage top speed
		if (Input.GetButton ("Sneak")) {
			maxVelocity = 2.5f;
		} else if (Input.GetButton ("Sprint")) {
			maxVelocity = 10.0f;
		} else {
			maxVelocity = 5.0f;
		}

		// Allow motion
		if (charRB.velocity.magnitude <= maxVelocity) {
			if (canJump) {
				charRB.AddForce (Input.GetAxis ("Vertical") * speed * transform.forward);
			} else {
				charRB.AddForce (Input.GetAxis ("Vertical") * speed * 0.5f * transform.forward);
			}
		}
		charRB.AddTorque (0.0f, Input.GetAxis ("Horizontal") * speed, 0.0f);
		// Jumping
		if (Input.GetButtonDown ("Jump") && canJump) {
			charRB.velocity = new Vector3 (0.0f, jumpForce, 0.0f);
			canJump = false;
		}
	}

	// Reset canJump every time ground is touched
	void OnCollisionEnter (Collision collision) {
		canJump = true;
	}
}
