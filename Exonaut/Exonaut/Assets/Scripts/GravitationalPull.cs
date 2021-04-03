using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalPull : MonoBehaviour
{
    public Transform body2;
    public float g = 9.8f;
    private Rigidbody rb;
    private Transform body1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        body1 = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 normGravVec = Vector3.Normalize(body2.position - body1.position);
        rb.AddForce(normGravVec * g * Time.deltaTime, ForceMode.VelocityChange);
        body1.up = normGravVec * -1;
    }
}
