using System.Collections.Generic;
using UnityEngine;

public class OnScreenButtonController : MonoBehaviour
{
    private bool left;
    private bool right;
    private bool up;
    private bool down;

    public Canvas upDownCanvas;
    public Canvas leftRightCanvas;

    private List<InputListener> _inputListeners;


    private void Awake()
    {
        _inputListeners = new List<InputListener>();

        var nc = NetworkController.Instance;
        if (nc != null)
        {
            _inputListeners.Add(nc);
            // TODO improve this such that "roles" aren't solely determined by net id
            upDownCanvas.enabled = nc.NetworkId == 0;
            leftRightCanvas.enabled = nc.NetworkId == 3;
        }
    }

    public void OnLeftPressed()
    {
        left = true;
        _inputListeners.ForEach(it => it.OnHorizontalMovementInput(-1));
    }

    public void OnRightPressed()
    {
        right = true;
        _inputListeners.ForEach(it => it.OnHorizontalMovementInput(1));
    }

    public void OnDownPressed()
    {
        down = true;
        _inputListeners.ForEach(it => it.OnVerticalMovementInput(-1));
    }

    public void OnUpPressed()
    {
        up = true;
        _inputListeners.ForEach(it => it.OnVerticalMovementInput(1));
    }

    public void OnLeftReleased()
    {
        left = false;
        if (!right) _inputListeners.ForEach(it => it.OnHorizontalMovementInput(0));
    }

    public void OnRightReleased()
    {
        right = false;
        if (!left) _inputListeners.ForEach(it => it.OnHorizontalMovementInput(0));
    }

    public void OnDownReleased()
    {
        down = false;
        if (!up) _inputListeners.ForEach(it => it.OnVerticalMovementInput(0));
    }

    public void OnUpReleased()
    {
        up = false;
        if (!down) _inputListeners.ForEach(it => it.OnVerticalMovementInput(0));
    }
}