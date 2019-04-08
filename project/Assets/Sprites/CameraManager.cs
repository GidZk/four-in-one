using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera[] cams;

    private void Awake()
    {
        SetCamera(0);
    }

    private void SetCamera(int n)
    {
        if (n < 0 || n > 3)
        {
            Debug.Log($"Bad camera index ({n})");
            return;
        }

        foreach (var cam in cams)
        {
            cam.enabled = false;
        }

        cams[n].enabled = true;
    }

    public void SetCamera(Cam cam)
    {
        SetCamera((int) cam);
    }
}

public enum Cam
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}