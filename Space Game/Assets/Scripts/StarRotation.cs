using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarRotation : MonoBehaviour
{
    public Transform cam;
    public int frequency = 4;
    private Transform self;
    private int freqVal = 0;

    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (freqVal == 0)
        {
            self.LookAt(cam);
            self.Rotate(90, 0, 0);
            freqVal = frequency;
        } else
        {
            freqVal--;
        }
    }
}
