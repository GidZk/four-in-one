
using UnityEngine;

public class BoundaryController : MonoBehaviour
{

    private void OnTriggerExit2D(Collider2D collision)
    {

        /*
         To next person working on this. We can use tag system in unity to 
         filter if the thing is a NPC or player.

        some pseudocode
        if (collider.tag =="NPC") {destroy()}
        if (collider.tag =="player) {bounce()}
       s
         */
        Debug.Log(" --Destroying object, object outside of boundary --");
        Destroy(collision.gameObject);
       

    }
}

