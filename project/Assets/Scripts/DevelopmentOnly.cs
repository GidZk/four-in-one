using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script to hide object outside of debug builds/unity editor
/// </summary>
public class DevelopmentOnly : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(Debug.isDebugBuild);
    }
}