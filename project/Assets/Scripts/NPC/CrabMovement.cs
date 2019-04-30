using UnityEngine;


public class CrabMovement : NPCMovement
{
    public bool isHooked;

    public bool IsHooked
    {
        get => isHooked;
        set => isHooked = value;
    }


    private void Awake()
    {
        isHooked = false;
    }


    protected override void Move()
    {
        // randomly simulate the crabby craaawlings
        if (Random.value < 0.06)
            direction.y = -direction.y;
        
        transform.Translate(direction * Time.deltaTime * movementSpeed,Space.World);
    }
    
}

