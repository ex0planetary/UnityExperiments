using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float speedX;
    public float jump;
    public float gravity = 0.0025f;
    public float velocityY = 0;
    public float velocityYCap = 0.01f;
    //public bool grounded = false;
    public float groundSearchSize = 0.01f;
    public Transform cam;
    private Transform tr;
    private CharacterController cc;
    public bool g;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        cc = GetComponent<CharacterController>();
    }

    bool grounded()
    {
        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Ignore Raycast");
        mask = ~mask;
        return Physics.Raycast(tr.position, transform.TransformDirection(tr.up * -1), out hit, groundSearchSize, mask);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        g = grounded();
        Vector3 unitDown = (tr.localPosition.normalized);
        Vector3 front = new Vector3(cam.localPosition.x * -1, 0, cam.localPosition.z * -1);
        //tr.forward = front.normalized;
        //tr.LookAt(tr.position + tr.TransformVector(front).normalized, -1 * unitDown);
        Vector3 fakeForward = tr.TransformVector(front).normalized;
        Vector3 fakeRight = Vector3.Cross(fakeForward, tr.up);
        //if (cc.isGrounded) grounded = true;
        if (grounded() && Input.GetButton("Jump"))
        {
            velocityY = jump * -1;
            //grounded = false;
        }
        if (grounded() && velocityY > 0) velocityY = 0;
        Vector3 movementDir = ((fakeForward * Input.GetAxisRaw("Vertical") * speedX) + (fakeRight * Input.GetAxisRaw("Horizontal") * speedX) + (unitDown * velocityY * -1));
        /*if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            grounded = false;
        }*/
        if (!grounded())
        {
            velocityY += gravity * Time.deltaTime;
        }
        cc.Move(movementDir * Time.deltaTime);
        //if (cc.isGrounded) velocityY = 0.1f;
        tr.up = -1 * unitDown;
    }
}
