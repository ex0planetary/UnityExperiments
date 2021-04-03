using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNebulae : MonoBehaviour
{
    public GameObject nebulaPrefab;
    public CreateStars.GalaxySector galaxySector;
    public int count = 3;
    public float multiplier = 10;

    // Start is called before the first frame update
    void Start()
    {
        for (int i=0; i<count; i++)
        {
            GameObject inst = (GameObject)Instantiate(nebulaPrefab, GetComponent<Transform>());
            //inst.GetComponent<Transform>().localScale *= Mathf.Pow(multiplier, i);
            inst.GetComponent<SpaceCloudPositionData>().sector = galaxySector;
        }
    }
}
