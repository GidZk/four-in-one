using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface ScoreListener
{
    void OnScoreChanged(int oldVal, int newVal);
}

public class AddScore : MonoBehaviour
{
    private static int _scoreValue;
    Text _score;

    private static List<ScoreListener> _listeners = new List<ScoreListener>();


    // Start is called before the first frame update
    void Start() => _score = GetComponent<Text>();

    // Update is called once per frame
    void Update() => _score.text = "" + _scoreValue;

    public static void IncScore(int add)
    {
        _scoreValue += add;
        NotifyListeners(_scoreValue - add, _scoreValue);
    }

    private static void NotifyListeners(int oldVal, int newVal)
    {
        _listeners.ForEach(l => l.OnScoreChanged(oldVal, newVal));
    }

    public static void SetScore(int newValue, bool silent)
    {
        var oldVal = _scoreValue;
        _scoreValue = newValue;
        if (!silent)
            NotifyListeners(oldVal, _scoreValue);
    }


    public static void Register(ScoreListener sl) => _listeners.Add(sl);


    public static int GetScore() => _scoreValue;
}