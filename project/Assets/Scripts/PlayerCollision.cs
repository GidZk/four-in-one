using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{


    //klass som ska göra så att när player krockar med poäng-objekten, scoreValue ++.

    private void OnTriggerExit2D(Collider2D collision)
    {

        /*
         To next person working on this. We can use tag system in unity to 
         filter if the thing is a NPC or player.
         

        */
        Debug.Log(" --a collision between player and star. object outside of boundary --");
        Destroy(collision.gameObject);
        //ScoreScript.scoreValue++; // happens when its a collision between player and bubble.



/*
        Debug.Log("OnTriggerEnter() was called");
        if (other.tag == "Asteroid")
        {
            Debug.Log("Other object is a coin");
            score += 25;
            scoreText = "Damage%: " + score;
            Debug.Log("Score is now " + score);
            Destroy(other.gameObject);
            GameObject.Find("Main Camera").animation.Play();
        }
  */  }


}
