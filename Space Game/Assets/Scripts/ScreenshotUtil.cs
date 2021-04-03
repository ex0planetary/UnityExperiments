using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ScreenshotUtil : MonoBehaviour
{
    public int resFactor = 2;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Screenshot"))
        {
            string filepath = Application.dataPath + "/Screenshots/screenshot" + DateTime.Now.ToString("MM-dd-yyyy_HH-mm-ss") + ".png";
            ScreenCapture.CaptureScreenshot(filepath, resFactor);
            Debug.Log("Succussfully took screenshot at " + filepath + " at resolution x" + resFactor);
        }
    }
}
