using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
///  Behaviour used to move the submarine displaying progressed time
/// </summary>
public class GameTimer : MonoBehaviour
{
    public float MaxTime = 20f;

    public float startPos;
    public float endPos;
    private float _diff;

    private float _startTime;

    private RectTransform rt;

    private void Awake()
    {
        _startTime = Time.time;
        _diff = endPos - startPos;
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        var elapsedTime = Time.time - _startTime;
        var newX = Mathf.Clamp(startPos + elapsedTime / MaxTime * _diff,
            startPos,
            endPos);

        var position = rt.position;
        var newPos = new Vector3(
            newX,
            position.y,
            position.z);


        rt.SetPositionAndRotation(
            newPos,
            Quaternion.identity
        );
        
        if (elapsedTime / MaxTime > 0.7f)
        {
            var size = 1f + 0.25f * Mathf.Cos(elapsedTime * 3);
            rt.localScale = new Vector3(size, size, 1);
        }

        if (elapsedTime > MaxTime)
        {
            if (NetworkController.Instance != null)
                NetworkController.Instance.EndGame();
            else
            {
                SceneManager.LoadScene(2);
            }
        }
    }
}