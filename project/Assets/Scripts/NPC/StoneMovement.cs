using UnityEngine;

public class StoneMovement : NPCMovement
{
    protected override void Move()
    {
        transform.Translate(Vector2.left * Time.deltaTime * movementSpeed, Space.World);
    }
    
}
