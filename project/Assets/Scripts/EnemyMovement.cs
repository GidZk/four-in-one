using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed;
    public GameObject shark;
    public GameObject stone;
    public GameObject crabplast;
    private bool sharkUp = true;
    private bool crabUp = true;

    private void FixedUpdate()
    {
        transform.Translate(Vector2.left * Time.deltaTime * speed, Space.World);

        if (sharkUp && gameObject == shark)
        {
            shark.transform.Translate(Vector2.up * Time.deltaTime * speed);
        }

        if (!sharkUp && gameObject == shark)
        {
            shark.transform.Translate(Vector2.down * Time.deltaTime * speed);
        }

        // if (BoundaryController.  object.objectcollide Equals(true))
        if (transform.position.y > 29 && gameObject == shark)
        {
            sharkUp = false;
        }

        if (transform.position.y < -29 && gameObject == shark)
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