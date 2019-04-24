using UnityEngine;

public class SharkMovement : NPCMovement
{
    protected override void Move()
    {
        transform.Translate(direction * Time.deltaTime * movementSpeed, Space.World);
        if (transform.position.y > 29 || transform.position.y < -29 )
            direction.y = -direction.y;
    }
}
