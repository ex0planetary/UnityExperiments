using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSizeAdjustment : MonoBehaviour 
{
    // Plane default scale is 5 on each side
    // x = 0.065, y = 1, z = 0.035
    public Camera cam;
    public float dist = 0.1f;
    private Transform tra;
    // Start is called before the first frame update
    void Start()
    {
        tra = GetComponent<Transform>();
        Vector3 p1 = cam.ScreenToWorldPoint(new Vector3(0, 0, dist));
        Vector3 p2 = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0, dist));
        Vector3 p3 = cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, dist));
        float realWidth = Vector3.Distance(p1, p2);
        float realHeight = Vector3.Distance(p1, p3);
        tra.localScale = new Vector3(realWidth / 10 + 0.05f, 1, realHeight / 10 + 0.05f);
        tra.Translate(new Vector3(0, -1*dist, 0), Space.Self);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
