using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class PlayerCameraControl : MonoBehaviour
{
    public float pitch = 0;
    public float yaw = 0;
    public float distance = 5;
    public float sensitivity = 1;
    private Transform tr;
    private LayerMask mask;
    private Camera ca;
    private Transform trp;
    private Vector3 mouseOld;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        ca = GetComponent<Camera>();
        mask = LayerMask.GetMask("Default");
        trp = transform.parent.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 toGo = new Vector3(Mathf.Sin(pitch) * Mathf.Cos(yaw), Mathf.Cos(pitch), Mathf.Sin(pitch) * Mathf.Sin(yaw));
        RaycastHit hit;
        float tempDist = distance;
        if (Physics.Raycast(trp.position, trp.TransformVector(toGo), out hit, distance, mask))
        {
            tempDist = hit.distance;
            tr.localPosition = new Vector3(tempDist * Mathf.Sin(pitch) * Mathf.Cos(yaw), tempDist * Mathf.Cos(pitch), tempDist * Mathf.Sin(pitch) * Mathf.Sin(yaw));
        } else
        {
            tr.localPosition = new Vector3(distance * Mathf.Sin(pitch) * Mathf.Cos(yaw), distance * Mathf.Cos(pitch), distance * Mathf.Sin(pitch) * Mathf.Sin(yaw));
        }
        //float oldZ;
        tr.LookAt(trp, trp.localPosition.normalized);
        if (Input.GetMouseButtonDown(1))
        {
            // save mouse pos
            mouseOld = Input.mousePosition;
        }
        if (Input.GetMouseButton(1))
        {
            //
            Vector3 mouseDelta = (Input.mousePosition - mouseOld) * -1;
            float newPitch = Time.deltaTime * sensitivity * mouseDelta.y + pitch;
            float newYaw = (Time.deltaTime * sensitivity * mouseDelta.x) * -1 + yaw;
            if (newPitch < 0) newPitch = 0;
            if (newPitch > Mathf.PI) newPitch = Mathf.PI;
            if (newYaw < 0)
            {
                while (newYaw < 0)
                {
                    newYaw += 2 * Mathf.PI;
                }
            }
            if (newYaw > 2 * Mathf.PI)
            {
                while (newYaw > 2 * Mathf.PI)
                {
                    newYaw -= 2 * Mathf.PI;
                }
            }
            pitch = newPitch;
            yaw = newYaw;
            mouseOld = Input.mousePosition;
        }
        if (pitch == 0 || pitch == Mathf.PI)
        {
            tr.Rotate(0, 0, -1 * (Mathf.Rad2Deg * yaw) - tr.localEulerAngles.z, Space.Self);
        }
    }
}
