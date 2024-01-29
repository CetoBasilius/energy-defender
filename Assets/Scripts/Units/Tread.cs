using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class Tread : MonoBehaviour
{
    public Vector2 speed;
    private Material scrollMaterial;
    private Vector2 currentscroll;

    void Start()
    {
        scrollMaterial = GetComponent<SpriteRenderer>().material;
    }

    void Update()
    {
        this.scrollMaterial.SetVector("_ScrollSpeed", speed);
    }
}
