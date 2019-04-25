using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class TouchInput : MonoBehaviour
{
    private List<InputListener> _inputListeners;

    [SerializeField] private float value;

    public float MaxDistance = 100;
    public Axis axis = Axis.Horizontal;

    private Vector3 _initialPos;

    private void Awake()
    {
        _inputListeners = new List<InputListener>();

        var nc = NetworkController.Instance;
        if (nc != null)
        {
            _inputListeners.Add(nc);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _initialPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            var diff = Input.mousePosition - _initialPos;

            // Normalize unwanted axis
            if (axis == Axis.Horizontal)
                diff.y = 0;
            else diff.x = 0;

            value = Mathf.Clamp(diff.magnitude / MaxDistance, -1, 1);
        }

        _inputListeners.ForEach(t =>
        {
            if (axis == Axis.Horizontal)
                t.OnHorizontalMovementInput(value);
            else
                t.OnVerticalMovementInput(value);
        });
    }
}

public enum Axis
{
    Horizontal,
    Vertical
}