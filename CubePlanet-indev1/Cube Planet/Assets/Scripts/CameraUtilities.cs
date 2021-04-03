using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CameraUtilities : MonoBehaviour
{
    //bool torchOn;
    public GameObject torch;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("F2"))
        {
            string path = Application.dataPath + "/Screenshots/" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
            //2 for ~720p, 3 for ~1080p, 6 for ~4k
            ScreenCapture.CaptureScreenshot(path,3);
            Debug.Log("Took screenshot at " + path);
        }
        if (Input.GetButtonDown("Torch"))
        {
            torch.SetActive(!torch.activeSelf);
        }
    }
}
