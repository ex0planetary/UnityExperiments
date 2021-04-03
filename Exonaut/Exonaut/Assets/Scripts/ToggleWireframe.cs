using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleWireframe : MonoBehaviour
{
    public bool wireframe = false;
    private bool wireframeOld = false;
    public Material wireframeMaterial;
    public Material emptyMaterial;
    private MeshRenderer mr;
    private Material[] mats;

    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mats = mr.materials;
        mats[1] = emptyMaterial;
        mr.materials = mats;
    }

    // Update is called once per frame
    void Update()
    {
        if (wireframe != wireframeOld)
        {
            mats = mr.materials;
            if (wireframe)
            {
                mats[1] = wireframeMaterial;
            }
            else
            {
                mats[1] = emptyMaterial;
            }
            mr.materials = mats;
            wireframeOld = wireframe;
        }
    }
}
