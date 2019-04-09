using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    public float speed;
    public GameObject shark;
    public GameObject stone;
    public GameObject crabplast;
    private bool sharkUp = true;
    private bool crabUp = true;

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


        if (sharkUp == true && gameObject == shark )
        {
            shark.transform.Translate(Vector2.up * Time.deltaTime * speed);
        }

        if (sharkUp == false && gameObject == shark )
        {
            shark.transform.Translate(Vector2.down * Time.deltaTime * speed);
        }

        // if (BoundaryController.  object.objectcollide Equals(true))
        if (transform.position.y > 4 && gameObject == shark)
        {
            sharkUp = false;
        }

        if (transform.position.y < -13 && gameObject == shark)
        {
            sharkUp = true;
        }

        if (Random.value < 0.06 && gameObject == crabplast)
        {
            if (crabUp == true)
            {
                crabUp = false;
            }

            else 
            {
                crabUp = true;
            }


        }

        if (crabUp == true && gameObject == crabplast)
        {
            crabplast.transform.Translate(Vector2.up * Time.deltaTime * speed);
        }

        if (crabUp == false && gameObject == crabplast)
        {
            crabplast.transform.Translate(Vector2.down * Time.deltaTime * speed);
        }



    }
}
