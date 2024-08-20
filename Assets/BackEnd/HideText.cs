using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideText : MonoBehaviour
{
    public GameObject yes;
    void Start()
    {
        yes.SetActive(false);
    }
    public void OnMouseOver()
    {
        yes.SetActive(true);
    }

    public void OnMouseExit()
    {
        yes.SetActive(false );
    }
}
