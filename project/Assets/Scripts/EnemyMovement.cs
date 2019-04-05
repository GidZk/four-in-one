using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    public float speed;
    public GameObject shark;
    public GameObject stone;
    public GameObject alga;
    private bool up = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {

        transform.Translate(Vector2.left * Time.deltaTime * speed);


        if (up == true && gameObject == shark )
        {
            shark.transform.Translate(Vector2.up * Time.deltaTime * speed);
        }

        if (up == false && gameObject == shark )
        {
            shark.transform.Translate(Vector2.down * Time.deltaTime * speed);
        }

        // if (BoundaryController.  object.objectcollide Equals(true))
        if (transform.position.y > 4 && gameObject == shark)
        {
            up = false;
        }

        if (transform.position.y < -13 && gameObject == shark)
        {
            up = true;
        }


    }
}
