using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODRecalculation : MonoBehaviour
{
    LODGroup group;
    // Start is called before the first frame update
    void Start()
    {
        group = GetComponent<LODGroup>();
        group.RecalculateBounds();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
