using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public new GameObject camera;
    Transform target;
    Transform trans;

    // Start is called before the first frame update
    void Start()
    {
        if (camera != null)
        {
            target = camera.GetComponent<Transform>().transform;
        }
        trans = GetComponent<Transform>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        trans.LookAt(target);
    }
}
