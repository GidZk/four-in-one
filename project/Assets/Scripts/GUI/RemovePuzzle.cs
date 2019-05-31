using UnityEngine;
using PlayerController = player.PlayerController;

public class RemovePuzzle : MonoBehaviour
{
    private float targetAlpha;
    private float currentAlpha;
    private bool _hasOkd;

    private SpriteRenderer[] sprites;
    public GameObject[] disableWhileActive;
    private bool _clear;

    public float CurrentAlpha
    {
        get { return currentAlpha; }
        set
        {
            currentAlpha = value;
            foreach (var sprite in sprites)
            {
                var color = sprite.color;
                sprite.color = new Color(
                    color.r,
                    color.g,
                    color.b,
                    value
                );
            }
        }
    }

    private void Start()
    {
        foreach (var g in disableWhileActive)
            g.SetActive(false);
    }

    private void Awake()
    {
        sprites = gameObject.GetComponentsInChildren<SpriteRenderer>();
        targetAlpha = 0.5f;
        CurrentAlpha = 0.5f;
    }

    private static void StartGame(bool start)
    {
        if (NetworkController.Instance != null)
        {
            if (!NetworkController.Instance.IsServer()) return;
        }

        var pc = PlayerController.Instance;
        if (pc != null) pc.gameObject.SetActive(start);
        var sp = Spawner.Instance;
        if (sp != null) sp.spawnDisabled = !start;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_clear)
            StartGame(false);
        InterpolateAlpha();
    }

    public void PointerDown()
    {
        if (_hasOkd) return;
        if (_clear) return;
        Debug.Log("Pointer down");
        targetAlpha = 1;
        NetworkController.Instance.PuzzleReady(true);
        _hasOkd = true;
    }

    public void PointerUp()
    {
        if (Debug.isDebugBuild) return;
        if (_clear) return;
        Debug.Log("Pointer up");
        targetAlpha = 0.5f;
        NetworkController.Instance.PuzzleReady(false);
        _hasOkd = false;
    }

    private void InterpolateAlpha()
    {
        CurrentAlpha += (targetAlpha - CurrentAlpha) * 0.1f;
        if (_clear && CurrentAlpha < 0.1f)
        {
            StartGame(true);
            foreach (var g in disableWhileActive)
                g.SetActive(true);

            Destroy(gameObject);
        }
    }

    /// called with SendMessage in NetworkController
    public void Clear()
    {
        _clear = true;
        targetAlpha = 0;
    }
}