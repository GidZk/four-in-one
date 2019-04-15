using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugKeyboardInput : MonoBehaviour
{
    // TODO improve
    public NetworkController nc;

    private float _spaceDownTime = -1;
    private List<InputListener> inputListeners;
    private const float CHARGE_TIME_FACTOR = 1;

    private void Awake()
    {
        inputListeners = new List<InputListener>();
        inputListeners.Add(nc);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _spaceDownTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            var val = Math.Min((Time.time - _spaceDownTime) * CHARGE_TIME_FACTOR, 1.0f);
            Debug.Log($"Firing with force {val}");
            inputListeners.ForEach(it => it.OnCannonLaunchInput(val));
        }
    }
}