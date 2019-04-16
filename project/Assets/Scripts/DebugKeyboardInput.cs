using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable CompareOfFloatsByEqualityOperator

public class DebugKeyboardInput : MonoBehaviour
{
    private List<InputListener> _inputListeners;
    
    public float _spaceDownTime = -1;
    public float ChargeTimeFactor = 1;
    public float _aimRotationSpeed = Mathf.PI;

    [SerializeField] private float _aimAngle;

    private void Awake()
    {
        _inputListeners = new List<InputListener>();
        var nc = NetworkController.Instance;
        if (nc != null)
            _inputListeners.Add(nc);
    }


    private float lastH;
    private float lastV;

    // Update is called once per frame
    void Update()
    {
        var h = (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0) +
                (Input.GetKey(KeyCode.RightArrow) ? 1 : 0);
        var v = (Input.GetKey(KeyCode.DownArrow) ? -1 : 0) +
                (Input.GetKey(KeyCode.UpArrow) ? 1 : 0);

        if (lastH != h) _inputListeners.ForEach(it => it.OnHorizontalMovementInput(h));
        if (lastV != v) _inputListeners.ForEach(it => it.OnVerticalMovementInput(v));

        lastH = h;
        lastV = v;

        if (Input.GetKey(KeyCode.Q))
        {
            _aimAngle -= _aimRotationSpeed * Time.deltaTime;
            _aimAngle = _aimAngle % (Mathf.PI * 2);
            _inputListeners.ForEach(it => it.OnCannonAngleInput(_aimAngle));
        }

        if (Input.GetKey(KeyCode.E))
        {
            _aimAngle += _aimRotationSpeed * Time.deltaTime;
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