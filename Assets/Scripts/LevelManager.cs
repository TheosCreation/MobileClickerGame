using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Transform generatorHolder;
    [HideInInspector] public ClickerButton[] generators;
    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        generators = generatorHolder.GetComponentsInChildren<ClickerButton>();
    }
}