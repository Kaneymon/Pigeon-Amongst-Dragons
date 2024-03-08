using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class terrinFix : MonoBehaviour
{
    TerrainCollider t;
    private void Awake()
    {
        t = transform.GetComponent<TerrainCollider>();
        t.enabled = false; t.enabled = true;
    }
}
