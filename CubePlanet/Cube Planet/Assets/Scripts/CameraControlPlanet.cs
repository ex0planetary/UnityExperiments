using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlPlanet : MonoBehaviour
{
    Transform trans;
    Vector3 mouse;
    public float sensitivity = 0.5f;
    public float speed = 1.0f;
    public int jitterOffset = 5;
    public Vector2Int rotationMinMax = new Vector2Int(-90, 90);
    public GameObject planet;
    bool held = false;
    Vector3 oldMouse;
    Transform plTrans;
    Vector3 relativeRotation;
    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>().transform;
        plTrans = planet.GetComponent<Transform>().transform;
        relativeRotation = trans.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        float rho = Mathf.Sqrt((trans.position.x-plTrans.position.x) * (trans.position.x-plTrans.position.x) + (trans.position.y-plTrans.position.y) * (trans.position.y-plTrans.position.y) + (trans.position.z-plTrans.position.z) * (trans.position.z-plTrans.position.z));
        float psi = Mathf.Acos((trans.position.y-plTrans.position.y) / rho);
        float theta = Mathf.Atan2((trans.position.z - plTrans.position.z), (trans.position.x - plTrans.position.x));
        if (Input.GetMouseButton(1))
        {
            mouse = Input.mousePosition;
            if (!held)
            {
                oldMouse = mouse;
            }
            held = true;
            relativeRotation.x += (oldMouse.y - mouse.y) * Time.deltaTime * sensitivity;
            relativeRotation.y = (oldMouse.x - mouse.x) * Time.deltaTime * sensitivity;
            oldMouse = mouse;
        }
        else
        {
            held = false;
            relativeRotation.y = 0;
        }
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        trans.Translate(movement * speed * Time.deltaTime);
        Vector3 oldForward = trans.forward + trans.position;
        float rho2 = Mathf.Sqrt((trans.position.x - plTrans.position.x) * (trans.position.x - plTrans.position.x) + (trans.position.y - plTrans.position.y) * (trans.position.y - plTrans.position.y) + (trans.position.z - plTrans.position.z) * (trans.position.z - plTrans.position.z));
        float psi2 = Mathf.Acos((trans.position.y - plTrans.position.y) / rho2);
        float theta2 = Mathf.Atan2((trans.position.z - plTrans.position.z), (trans.position.x - plTrans.position.x));
        float rhof = Mathf.Sqrt((oldForward.x - plTrans.position.x) * (oldForward.x - plTrans.position.x) + (oldForward.y - plTrans.position.y) * (oldForward.y - plTrans.position.y) + (oldForward.z - plTrans.position.z) * (oldForward.z - plTrans.position.z));
        float psif = Mathf.Acos((oldForward.y - plTrans.position.y) / rhof);
        float thetaf = Mathf.Atan2((oldForward.z - plTrans.position.z), (oldForward.x - plTrans.position.x));
        float upDown = 0;
        if (Input.GetButton("Fire1")) upDown = -1;
        if (Input.GetButton("Jump")) upDown = 1;
        rho += upDown * speed * Time.deltaTime;
        trans.position = new Vector3(rho * Mathf.Sin(psi2) * Mathf.Cos(theta2) + plTrans.position.x, rho * Mathf.Cos(psi2) + plTrans.position.y, rho * Mathf.Sin(psi2) * Mathf.Sin(theta2) + plTrans.position.z);
        Vector3 newForward = new Vector3(rho * Mathf.Sin(psif) * Mathf.Cos(thetaf) + plTrans.position.x, rho * Mathf.Cos(psif) + plTrans.position.y, rho * Mathf.Sin(psif) * Mathf.Sin(thetaf) + plTrans.position.z);
        Vector3 fwdVector = newForward - trans.position;
        Vector3 degPlanet = new Vector3(rho, psi2 * Mathf.Rad2Deg, theta2 * Mathf.Rad2Deg);
        Vector3 oldUp = trans.up;
        Transform lookTrans = trans;
        lookTrans.LookAt(trans.position + fwdVector, Vector3.Normalize(trans.position - plTrans.position));
        lookTrans.RotateAround(trans.position, trans.right, relativeRotation.x);
        lookTrans.RotateAround(trans.position, trans.up, -1*relativeRotation.y);
        trans = lookTrans;
    }
}
