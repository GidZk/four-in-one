using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface InputListener
{
    /// called on vertical input, -1 <= value <= 1 (positive is up)
    void OnVerticalMovementInput(float value);

    /// called on horizontal input, -1 <= value <= 1 (positive is right)
    void OnHorizontalMovementInput(float value);

    /// called on cannon angle changing, in radians
    void OnCannonAngleInput(float value);

    /// called on cannon force changing, 0 <= value <= 1
    void OnCannonLaunchInput(float value);
}