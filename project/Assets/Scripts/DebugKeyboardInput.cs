using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugKeyboardInput : MonoBehaviour
{
    // TODO improve
    public NetworkController nc;

    private float _spaceDownTime = -1;
    private List<InputListener> _inputListeners;
    private const float ChargeTimeFactor = 1;
    private float _aimAngle;
    private float _aimRotationSpeed = 0.018f;

    private void Awake()
    {
        _inputListeners = new List<InputListener> {nc};
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            _aimAngle -= _aimRotationSpeed;
            _aimAngle = _aimAngle % (Mathf.PI * 2);
            _inputListeners.ForEach(it => it.OnCannonAngleInput(_aimAngle));
        }

        if (Input.GetKey(KeyCode.E))
        {
            _aimAngle += _aimRotationSpeed;
            _aimAngle = _aimAngle % (Mathf.PI * 2);
            _inputListeners.ForEach(it => it.OnCannonAngleInput(_aimAngle));
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _spaceDownTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            var val = Math.Min((Time.time - _spaceDownTime) * ChargeTimeFactor, 1.0f);
            Debug.Log($"Firing with force {val}");
            _inputListeners.ForEach(it => it.OnCannonLaunchInput(val));
        }
    }
}