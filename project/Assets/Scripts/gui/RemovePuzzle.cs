using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Networking;

public class RemovePuzzle : MonoBehaviour
{
    private float targetAlpha;
    private float currentAlpha;
    private bool _hasOkd;

    private SpriteRenderer[] sprites;
    private bool _clear;

    public float CurrentAlpha
    {
        get { return currentAlpha; }
        set
        {
            currentAlpha = value;
            foreach (var sprite in sprites)
            {
                var color = sprite.color;
                sprite.color = new Color(
                    color.r,
                    color.g,
                    color.b,
                    value
                );
            }
        }
    }


    private void Awake()
    {
        sprites = gameObject.GetComponentsInChildren<SpriteRenderer>();
        targetAlpha = 0.5f;
        CurrentAlpha = 0.5f;
    }

    private static void StartGame(bool start)
    {
        if (!NetworkController.Instance.IsServer()) return;
        var pc = playerController.Instance.gameObject;
        if (pc != null) pc.SetActive(start);
        var sp = Spawner.Instance;
        if (sp != null) sp.spawnDisabled = !start;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_clear)
            StartGame(false);
        InterpolateAlpha();
    }

    public void PointerDown()
    {
        if (_hasOkd) return;
        if (_clear) return;
        Debug.Log("Pointer down");
        targetAlpha = 1;
        NetworkController.Instance.PuzzleReady(true);
        _hasOkd = true;
    }

    public void PointerUp()
    {
        if (Debug.isDebugBuild) return;
        if (_clear) return;
        Debug.Log("Pointer up");
        targetAlpha = 0.5f;
        NetworkController.Instance.PuzzleReady(false);
        _hasOkd = false;
    }

    private void InterpolateAlpha()
    {
        CurrentAlpha += (targetAlpha - CurrentAlpha) * 0.1f;
        if (_clear && CurrentAlpha < 0.1f)
        {
            StartGame(true);
            Destroy(gameObject);
        }
    }

    /// called with SendMessage in NetworkControll (TODO kill me)
    public void Clear()
    {
        _clear = true;
        targetAlpha = 0;
    }
}