using GooglePlayGames;
using System.Collections;
using UnityEngine;

public class PlayGamesAuth : MonoBehaviour
{
    public bool m_authenticated { get; private set; }

    public IEnumerator Init()
    {
        PlayGamesPlatform.Activate();
        bool isCompleted = false;

        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log("Authentication was successful");
                m_authenticated = true;
            }
            else
            {
                Debug.Log("Authentication failed");
                m_authenticated = false;
            }

            isCompleted = true;
        });

        // Wait until the authentication process completes
        yield return new WaitUntil(() => isCompleted);

        Debug.Log("Authentication process completed.");
    }

    internal void ProcessAuthentication(GooglePlayGames.BasicApi.SignInStatus status)
    {
        Debug.Log("Statis is: " + status);
        if (status == GooglePlayGames.BasicApi.SignInStatus.Success)
        {
            m_authenticated = true;
            Debug.Log("User is authenticated");
        }
        else
        {
            Debug.Log("User is not authenticated");
            m_authenticated = false;
        }
    }
}
