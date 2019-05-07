using UnityEngine;
using PlayerController = player.PlayerController;

/// <summary>
/// Class used to lock the game until all users press the screen
///
/// This is referred to as the users building a "puzzle" to
/// establish relative positions
///
/// This is to allow them to change the orientation of the devices
/// </summary>
public class RemovePuzzle : MonoBehaviour
{
    /// <summary>
    /// GameObjects that should be disabled while the puzzle is active
    /// </summary>
    public GameObject[] freezeWhileActive;

    private float targetAlpha;
    private float currentAlpha;
    private bool _hasOkd;

    private SpriteRenderer[] sprites;
    private bool _clear;

    /// <summary>
    /// Current alpha of the sprites that form the "puzzle"
    /// Used for fancy fading, could probably be implemented
    /// with co-routines in a simpler way
    /// </summary>
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


    private void Awake()
    {
        sprites = gameObject.GetComponentsInChildren<SpriteRenderer>();
        targetAlpha = 0.5f;
        CurrentAlpha = 0.5f;
    }

    private void StartGame(bool start)
    {
        if (!NetworkController.Instance.IsServer()) return;
        var pc = PlayerController.Instance;
        if (pc != null) pc.gameObject.SetActive(start);
        foreach (var o in freezeWhileActive)
        {
            o.SetActive(start);
        }
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

    /// <summary>
    /// Updates the alpha value
    /// If the alpha is close to zero, i.e. the puzzle is being
    /// hidden, the game starts
    /// </summary>
    private void InterpolateAlpha()
    {
        CurrentAlpha += (targetAlpha - CurrentAlpha) * 0.1f;
        if (_clear && CurrentAlpha < 0.1f)
        {
            StartGame(true);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// called by the NetworkController once all players have
    /// pressed the button
    /// </summary>
    public void Clear()
    {
        _clear = true;
        targetAlpha = 0;
    }
}