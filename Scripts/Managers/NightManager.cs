using UnityEngine;
using UnityEngine.SceneManagement;

public class NightManager : MonoBehaviour
{
    public static NightManager Instance;

    [Header("Current Night")]
    [SerializeField] private int currentNight = 1;

    public int CurrentNight => currentNight;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void LoadNextNight()
    {
        currentNight++;

        string nextScene = "Night_0" + currentNight;

        Debug.Log("Loading " + nextScene);

        SceneManager.LoadScene(nextScene);
    }

    public void LoadNight(int night)
    {
        currentNight = night;

        string sceneName = "Night_0" + night;

        Debug.Log("Loading " + sceneName);

        SceneManager.LoadScene(sceneName);
    }
}