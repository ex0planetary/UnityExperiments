using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

	// Initialize variables
	public GameObject playerObject;
	private Transform playerTransform;
	private Vector3 position;
	private Quaternion rotation;
	private Quaternion originalRot;
	private bool FirstPerson;
	// Init function
	void Start () {
		// Get playerTransform component
		playerTransform = playerObject.GetComponent<Transform> ();
		originalRot = transform.rotation;
		FirstPerson = false;
	}
	
	// Change camera position every frame
	void Update () {
		if (Input.GetButtonDown ("First Person")) {
			FirstPerson = !FirstPerson;
		}
		// Set camera position
		if (FirstPerson) {
			position = playerTransform.position;
			rotation = new Quaternion (0.0f, playerTransform.rotation.y, 0.0f, playerTransform.rotation.w);
		} else {
			position = playerTransform.position + new Vector3 (0, 14, 0);
			rotation = originalRot;
		}
		transform.position = position;
		transform.rotation = rotation;
	} 
}
