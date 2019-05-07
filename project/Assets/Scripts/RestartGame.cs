using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartGame : MonoBehaviour
{
    private void Start()
    {
        var nc = NetworkController.Instance;
        if (nc == null)
        {
            Debug.Log("No NetworkController");
            return;
        }

        if (!nc.IsServer()) gameObject.SetActive(false);
    }

    public void OnButtonPressed()
    {
        var nc = NetworkController.Instance;
        if (nc == null)
        {
            Debug.Log("No NetworkController");
            return;
        }

        nc.RestartGame();
    }
}