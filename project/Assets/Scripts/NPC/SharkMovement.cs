using UnityEngine;

public class SharkMovement : NPCMovement
{

    public float yFieldOfView;
    protected override void Move()
    {
        transform.Translate(direction * Time.deltaTime * movementSpeed, Space.World);
        if (transform.position.y > yFieldOfView || transform.position.y < -yFieldOfView )
            direction.y = -direction.y;
    }
}
