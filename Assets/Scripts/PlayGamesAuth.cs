using GooglePlayGames;
using System.Collections;
using UnityEngine;

public class PlayGamesAuth : MonoBehaviour
{
    // Boolean property to track if the user is authenticated with Google Play Games
    public bool m_authenticated { get; private set; }

    // Coroutine to initialize the authentication process
    public IEnumerator Init()
    {
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();

        // Flag to check if authentication process is completed
        bool isCompleted = false;

        // Attempt to authenticate the user using Google Play Games
        Social.localUser.Authenticate((bool success) =>
        {
            // Check if authentication was successful
            if (success)
            {
                // Log success and set m_authenticated to true
                Debug.Log("Authentication was successful");
                m_authenticated = true;
            }
            else
            {
                // Log failure and set m_authenticated to false
                Debug.Log("Authentication failed");
                m_authenticated = false;
            }

            // Mark the authentication process as complete
            isCompleted = true;
        });

        // Wait until the authentication process has finished
        yield return new WaitUntil(() => isCompleted);

        // Log when the authentication process is completed
        Debug.Log("Authentication process completed.");
    }

    // Internal method to handle the result of the authentication process
    internal void ProcessAuthentication(GooglePlayGames.BasicApi.SignInStatus status)
    {
        // Log the status of the authentication process
        Debug.Log("Status is: " + status);

        // Check the status of authentication
        if (status == GooglePlayGames.BasicApi.SignInStatus.Success)
        {
            // If successful, set m_authenticated to true and log the result
            m_authenticated = true;
            Debug.Log("User is authenticated");
        }
        else
        {
            // If failed, set m_authenticated to false and log the result
            Debug.Log("User is not authenticated");
            m_authenticated = false;
        }
    }
}