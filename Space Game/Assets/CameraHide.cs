using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHide : MonoBehaviour
{
    public Transform cam;
    public float renderDist = 100;
    private Transform tr;
    private MeshRenderer mr;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        mr = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float dist = (tr.position - cam.position).magnitude;
        if (dist > renderDist)
        {
            mr.enabled = false;
        } else
        {
            mr.enabled = true;
        }
    }
}
