using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camerabuttons1 : MonoBehaviour
{
    public Camera Firstcamera;
    public Camera Secondcamera;
    // Start is called before the first frame update
    void Start()
    {
       Firstcamera.enabled = true;
       Secondcamera.enabled = false;
    }

    // Update is called once per frame
    public void S()
    {
        Firstcamera.enabled = true;
        Secondcamera.enabled = false;
    }
    public void F()
    {
        Firstcamera.enabled = false;
        Secondcamera.enabled = true;
    }
}
