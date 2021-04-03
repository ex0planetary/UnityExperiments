using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlSpace : MonoBehaviour
{
    Transform trans;
    Vector3 mouse;
    public float sensitivity = 0.5f;
    public float speed = 1.0f;
    bool held = false;
    Vector3 oldMouse;
    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            mouse = Input.mousePosition;
            if (!held)
            {
                oldMouse = mouse;
            }
            held = true;
            //trans.Rotate(((Screen.height / 2) - mouse.y) * Time.deltaTime * sensitivity, (mouse.x - (Screen.width / 2)) * Time.deltaTime * sensitivity, 0);
            trans.Rotate((oldMouse.y - mouse.y) * Time.deltaTime * sensitivity, (mouse.x - oldMouse.x) * Time.deltaTime * sensitivity, 0);
            Vector3 oldAngles = new Vector3(trans.eulerAngles.x, trans.eulerAngles.y, 0);
            trans.eulerAngles = oldAngles;
            oldMouse = mouse;
        } else
        {
            held = false;
        }
        //Forward: vector3.right, back = left, right = forward, left = back
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (Input.GetButton("Fire1")) movement.y = -1;
        if (Input.GetButton("Jump")) movement.y = 1;
        trans.Translate(movement * speed * Time.deltaTime);
    }
}
