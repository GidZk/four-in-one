
    using UnityEngine;

    public class StoneController : NPCController
    {
        protected override void Move()
        {
            transform.Translate(Vector2.left * Time.deltaTime * movementSpeed, Space.World);

        }
    }
