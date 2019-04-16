using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public Camera[] cams;

    public Camera godCam;

    private void Awake()
    {
        var nc = FindObjectOfType(typeof(NetworkController)) as NetworkController;
        if (nc == null)
        {
            Debug.Log("No network manager");
            SetCamera(0);
            return;
        }

        if (nc.SingleGameDebug)
        {
            SetGodCamera();
            Debug.Log("Enable god camera");
        }
        else
        {
            SetCamera(nc.NetworkId);
            Debug.Log($"Set camera {nc.NetworkId}");
        }
    }

    private void SetGodCamera()
    {
        foreach (var cam in cams)
        {
            cam.enabled = false;
        }

        godCam.enabled = true;
    }

    private void SetCamera(int n)
    {
        if (n < 0 || n > 3)
        {
            Debug.Log($"Bad camera index ({n})");
            return;
        }

        Debug.Log($"Set camera to {n}");

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