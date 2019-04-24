using UnityEngine;


public class CrabMovement : NPCMovement
{
    protected override void Move()
    {
        // randomly simulate the crabby craaawlings
        if (Random.value < 0.06)
            direction.y = -direction.y;
        
        transform.Translate(direction * Time.deltaTime * movementSpeed,Space.World);
    }
    
}

