using UnityEngine;

public class PauseManager : MonoBehaviour
{
    // Singleton pattern to ensure only one instance of PauseManager exists
    public static PauseManager Instance { get; private set; }

    // Boolean variable to track if the game is paused
    private bool isPaused = false;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Check if an instance of PauseManager already exists
        if (Instance == null)
        {
            // If no instance exists, set this as the instance and prevent it from being destroyed when loading a new scene
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this duplicate instance to ensure only one PauseManager
            Destroy(gameObject);
        }
    }

    // Method to pause the game by setting timeScale to 0
    public void Pause()
    {
        // If the game is already paused, no need to pause again
        if (isPaused) return;

        // Set isPaused to true to indicate the game is paused
        isPaused = true;

        // Set Time.timeScale to 0, effectively freezing the game (pausing it)
        Time.timeScale = 0f;

        // Log to the console that the game has been paused
        Debug.Log("Game Paused");
    }

    // Method to unpause the game by restoring timeScale to 1
    public void UnPause()
    {
        // If the game is not paused, no need to unpause
        if (!isPaused) return;

        // Set isPaused to false to indicate the game is resumed
        isPaused = false;

        // Set Time.timeScale to 1, which resumes normal game flow
        Time.timeScale = 1f;

        // Log to the console that the game has been unpaused
        Debug.Log("Game Unpaused");
    }

    // Method to toggle the pause state (pause if unpaused, unpause if paused)
    public void TogglePause()
    {
        // If the game is paused, unpause it; otherwise, pause it
        if (isPaused)
        {
            UnPause();
        }
        else
        {
            Pause();
        }
    }

    // Method to check if the game is currently paused
    public bool IsPaused()
    {
        // Return the current pause state
        return isPaused;
    }
}