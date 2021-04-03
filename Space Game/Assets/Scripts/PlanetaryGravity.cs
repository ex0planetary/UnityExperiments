using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Why the fuck is collision not working?

public class PlanetaryGravity : MonoBehaviour
{
    public float strength = 0.0025f;
    public float velocityY = 0;
    public float velocityCap = 0.01f;
    Transform tr;
    CharacterController cc;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        cc = GetComponent<CharacterController>();
        velocityY = 0;
    }

    // Update is called once per frame
    void Update()
    {
        /*velocityY -= strength;
        float yDisp = velocityY * Time.deltaTime;
        // convert current local position into spherical coordinates
        float rho = tr.localPosition.magnitude;
        float psi = Mathf.Acos(tr.localPosition.z / rho);
        float theta = Mathf.Atan2(tr.localPosition.y, tr.localPosition.x);
        // raycast and subtract from rho accordingly???
        rho += yDisp;
        // convert back to cartesian coords
        tr.localPosition = new Vector3(rho * Mathf.Sin(psi) * Mathf.Cos(theta), rho * Mathf.Sin(psi) * Mathf.Sin(theta), rho * Mathf.Cos(psi));*/
    }

    private void FixedUpdate()
    {
        Vector3 unitDown = -1 * (tr.localPosition.normalized);
        velocityY += strength;
        if (velocityY > velocityCap) velocityY = velocityCap;
        cc.Move(unitDown * velocityY * Time.deltaTime);
        if (cc.isGrounded) velocityY = 0;
        tr.up = -1 * unitDown;
    }
}
