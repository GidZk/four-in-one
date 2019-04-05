using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Networking;

// A PlayerUnit is a unit controlled by a player
// This could be a character in an FPS, a zergling in a RTS
// Or a scout in a TBS

public class PlayerUnit : NetworkBehaviour
{
    private Vector2 touchOrigin = -Vector2.one; //Used to store location of screen touch origin for mobile controls.

    private NetworkClient m_Client;
    Rigidbody2D rigidbody;



    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        var horizontal = 0;
        var vertical = 0;
        // This function runs on ALL PlayerUnits -- not just the ones that I own.

        // How do I verify that I am allowed to mess around with this object?
        if (hasAuthority == false)
        {
            return;
        }

        if (Input.touchCount > 0)
        {
            Debug.Log("read a touch");
            var myTouch = Input.touches[0];
            if (myTouch.phase == TouchPhase.Began)
                touchOrigin = myTouch.position;

            //If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
            else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                var touchEnd = myTouch.position;
                var x = touchEnd.x - touchOrigin.x;
                var y = touchEnd.y - touchOrigin.y;

                touchOrigin.x = -1;

                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    horizontal = x > 0 ? 1 : -1;
                    Debug.Log("read horizontal touch");
                }
                else
                {
                    vertical = y > 0 ? 1 : -1;
                    Debug.Log("read horizontal touch");
                }
            }
        }

        if (Input.GetKey(KeyCode.UpArrow)) vertical = 1;
        else if (Input.GetKey(KeyCode.DownArrow)) vertical = -1;

        if (Input.GetKey(KeyCode.RightArrow)) horizontal = 1;
        else if (Input.GetKey(KeyCode.LeftArrow)) horizontal = -1;

        rigidbody.AddForce(new Vector2(horizontal, vertical));
        //transform.Translate(horizontal * Time.deltaTime, vertical * Time.deltaTime, 0);
    }
}