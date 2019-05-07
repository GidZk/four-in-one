using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddScore : MonoBehaviour
{
    public static int scoreValue;
    Text scoreText;


    // Start is called before the first frame update
    void Start()
    {
        scoreValue = 0;
        scoreText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "" + scoreValue;
    }
}