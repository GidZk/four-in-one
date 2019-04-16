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
    private float _aimAngle = 0;
    private float _aimRotationSpeed = 0.018f;

    private void Awake()
    {
        inputListeners = new List<InputListener> {nc};
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            _aimAngle -= _aimRotationSpeed;
            _aimAngle = _aimAngle % (Mathf.PI * 2);
            inputListeners.ForEach(it => it.OnCannonAngleInput(_aimAngle));
        }

        if (Input.GetKey(KeyCode.E))
        {
            _aimAngle += _aimRotationSpeed;
            _aimAngle = _aimAngle % (Mathf.PI * 2);
            inputListeners.ForEach(it => it.OnCannonAngleInput(_aimAngle));
        }

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