using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;


public abstract class NPCMovement : MonoBehaviour
{
    public float movementSpeed;
    protected Vector3 direction;



    void FixedUpdate()
    {
        Move();
    }

    protected abstract void Move();



    public Vector2 Direction
    {
        set => direction = value;
    }


}



