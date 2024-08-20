using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger1 : MonoBehaviour
{
    [SerializeField]
    private Color childrensColor;
    private Renderer[] renderers;

    void Awake()
    {
        RefreshRenderersList();
        ChangeColor();
    }

    void RefreshRenderersList()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void ChangeColor()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.material.color = childrensColor;
        }
    }
}
