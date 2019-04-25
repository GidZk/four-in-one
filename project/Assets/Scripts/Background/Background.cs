using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    Renderer rend;
    public float backgroundSpeed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 offset = new Vector2(Time.time * backgroundSpeed, 0);

        rend.material.mainTextureOffset = offset;

    }
}
