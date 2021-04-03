using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public float distance;
    public float axisRotation;
    public float yearLength;
    public GameObject parent;
    float currentAngle = 0f;
    Transform trans;
    Transform parTrans;
    Vector3 ToGo;
    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>().transform;
        if (parent != null)
        {
            parTrans = parent.GetComponent<Transform>().transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        ToGo.y = 0;
        ToGo.x = Mathf.Cos(currentAngle);
        ToGo.z = Mathf.Sin(currentAngle);
        trans.position = distance * ToGo;
        currentAngle += (Time.deltaTime / yearLength) * 2 * Mathf.PI;
        trans.Rotate(Vector3.up, axisRotation * Time.deltaTime * 360);
        trans.position += parTrans.position;
    }
}
