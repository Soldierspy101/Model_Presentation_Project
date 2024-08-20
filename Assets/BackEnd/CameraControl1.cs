using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls1 : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Camera1;
    public GameObject Camera2;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("1"))
        {
            CameraOne();
        }
        if(Input.GetKeyDown("2"))
        {
            CameraTwo();
        }
    }
    void CameraOne()
    {
        Camera1.SetActive(true);
        Camera2.SetActive(false);
    }
    void CameraTwo()
    {
        Camera1.SetActive(false);
        Camera2.SetActive(true);
    }
}
