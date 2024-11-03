using System.Reflection;
using UnityEngine;

public class UiPage : MonoBehaviour
{
    [SerializeField] private UiButton[] buttons;

    private void OnEnable()
    {
        foreach (UiButton uiButton in buttons)
        {
            if (uiButton.button != null && !string.IsNullOrEmpty(uiButton.clickFunction))
            {
                MethodInfo method = GetType().GetMethod(uiButton.clickFunction, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (method != null)
                {
                    uiButton.button.onClick.AddListener(() => method.Invoke(this, null));
                }
                else
                {
                    Debug.LogWarning($"Method '{uiButton.clickFunction}' not found on {nameof(MainMenuMainPage)}");
                }
            }
        }
    }

    private void OnDisable()
    {
        foreach (UiButton uiButton in buttons)
        {
            uiButton.button?.onClick.RemoveAllListeners();
        }
    }
}