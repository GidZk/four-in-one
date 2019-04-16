using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevelopmentOnly : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.SetActive(Debug.isDebugBuild);
    }
}