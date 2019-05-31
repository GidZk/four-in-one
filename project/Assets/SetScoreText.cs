using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetScoreText : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Text>().text = AddScore.GetScore().ToString();
    }
}