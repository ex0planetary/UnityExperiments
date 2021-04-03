using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceCloudPositionData : MonoBehaviour
{
    private Renderer rend;
    public Transform trans;
    public CreateStars.GalaxySector sector;
    public Vector3 center = new Vector3(0, 0, 0);

    private void Start()
    {
        rend = GetComponent<Renderer>();
        int sectorInt;
        switch (sector)
        {
            case CreateStars.GalaxySector.EPSILON:
                sectorInt = 0;
                break;
            case CreateStars.GalaxySector.DELTA:
                sectorInt = 1;
                break;
            case CreateStars.GalaxySector.GAMMA:
                sectorInt = 2;
                break;
            case CreateStars.GalaxySector.BETA:
                sectorInt = 3;
                break;
            case CreateStars.GalaxySector.ALPHA:
                sectorInt = 4;
                break;
            default:
                sectorInt = 0;
                break;
        }
        rend.material.SetInt("_Sector", sectorInt);
    }

    private void Update()
    {
        Vector4 vec = new Vector4(0, 0, 0, 0);
        vec.x -= center.x * 40;
        vec.y -= center.y * 40;
        vec.z -= center.z * 40;
        vec.w = 1;
        if (Input.GetKeyDown(KeyCode.X))
            Debug.Log(vec);
        rend.material.SetVector("_Center", vec);
    }

    private void OnPreRender()
    {
        Vector4 vec = new Vector4(0, 0, 0, 0);
        vec.x -= center.x * 40;
        vec.y -= center.y * 40;
        vec.z -= center.z * 40;
        vec.w = 1;
        if (Input.GetKeyDown(KeyCode.X))
            Debug.Log(vec);
        rend.material.SetVector("_Center", vec);
    }
}
