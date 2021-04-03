using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCharacter : MonoBehaviour
{
    private Rigidbody rb;
    private Transform tr;
    public float playerSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce((tr.forward * Input.GetAxis("Vertical") + tr.right * Input.GetAxis("Horizontal")) * Time.deltaTime * playerSpeed, ForceMode.VelocityChange);
    }
}
