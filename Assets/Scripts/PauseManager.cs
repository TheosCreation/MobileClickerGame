using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Pause()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;

        Debug.Log("Game Paused");
    }

    public void UnPause()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;

        Debug.Log("Game Unpaused");
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            UnPause();
        }
        else
        {
            Pause();
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}