using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarFadeIn : MonoBehaviour
{
    public float fadeTime = 1.0f;
    private float currTime = 0.0f;
    private int updoot = -1;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.SetFloat("_StarPower1_", 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (updoot > -2)
        {
            updoot += 1;
        }
        if (updoot % 4 == 0)
        {
            currTime += Time.deltaTime;
            if (currTime > fadeTime)
            {
                GetComponent<Renderer>().material.SetFloat("_StarPower1_", 0.79f);
                updoot = -2;
            }
            else
            {
                GetComponent<Renderer>().material.SetFloat("_StarPower1_", Mathf.Lerp(0.0f, 0.79f, currTime / fadeTime));
            }
        }
    }
}
