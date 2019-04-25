using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    Image timerBar;
    public float maxTime;
    public static float timeLeft;
    public GameObject GameOver;

    // Start is called before the first frame update
    void Start()
    {
        GameOver.SetActive(false);
        timerBar = GetComponent<Image>();
        timeLeft = maxTime;
    }

    // Update is called once per frame
    void Update()
    {

        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerBar.fillAmount = timeLeft / maxTime;
        }
        else
        {
            GameOver.SetActive(true);
            Time.timeScale = 0;

        }
    }
}
