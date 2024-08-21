using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI panel")]
    [SerializeField] private GameObject GameOverUI;
    [SerializeField] private GameObject PauseUI;
    [SerializeField] private GameObject HeadUpDisplayUI;
    [SerializeField] private GameObject MainMenuUI;
    [SerializeField] private GameObject CreditsUI;
    [SerializeField] private Text HighScore;
    public Text CountdownText;

    //Game Bool
    private bool IsPaused = true;
    private bool gameHasEnded = false;

    private bool _isPlaying = false;
    public bool IsPlaying { get { return _isPlaying; } set { _isPlaying = value; } }

    public ParallaxManager ParallaxManager;

    //_-_-_-_-_-_-_-_-_-_-_-Data to Save To GameData_-_-_-_-_-_-_-_-_-_-_-//
    [HideInInspector] public GameData gameData;
    //_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-//

    //_-_-_-_-_-_-_-_-_-_-Static acces to GameManager_-_-_-_-_-_-_-_-_-_-_-//
    private static GameManager instance;
    // Accessor for the singleton instance
    public static GameManager Instance
    {
        get
        {
            // If the instance is null, try to find an existing GameManager object in the scene
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();

                // If no GameManager object exists, create a new one
                if (instance == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    instance = obj.AddComponent<GameManager>();
                }
            }

            return instance;
        }
    }

    public void SaveGame()
    {
        SaveSystem.SaveGame(gameData);
        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        gameData = SaveSystem.LoadGame();

        if (gameData == null)
        {
            gameData = new GameData()
            {
                IsFirstStart = 1,
                HighScore = 0,
            };
            SaveGame();
        }
        else
        {
            Debug.Log("Game Loaded");
        }

        DisplayHighScore(gameData.HighScore.ToString());
    }

    void Awake()
    {
        ParallaxManager = FindObjectOfType<ParallaxManager>();
        LoadGame();
    }

    // Update is called once per frame
    private void Update () 
	{
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!IsPaused && !gameHasEnded)
                Pause();
            else if (!gameHasEnded)
                Resume();
        }
#endif

        if (Input.GetButtonDown("Start") || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            if (!IsPaused && !gameHasEnded)
                Pause();
            else if (!gameHasEnded)
                Resume();
        }

        IsPlaying = (!IsPaused && !gameHasEnded);
    }

    public void Play()
    {
        IsPaused = false;
        FadeOut(MainMenuUI, 0.25f);
        FadeIn(HeadUpDisplayUI, 0.25f);
    }

    public void Pause()
    {
        IsPaused = true;

        FadeOut(HeadUpDisplayUI, 0.25f);
        FadeIn(PauseUI, 0.25f);

        Debug.Log("IsPause=true");
        SaveGame();
    }

    public void Resume()
    {
        IsPaused = false;

        FadeIn(HeadUpDisplayUI, 0.25f);
        FadeOut(PauseUI, 0.25f);

        Debug.Log("IsPause=false");
    }

    public void GameOver()
    {
        gameHasEnded = true;
        IsPlaying = false;

        FadeOut(HeadUpDisplayUI, 0.25f);
        FadeIn(GameOverUI, 0.25f);

        SaveGame();
    }

    public void Restart()
    {
        if(gameHasEnded)
            FadeOut(GameOverUI, 0.25f);
        if(IsPaused)
            FadeOut(PauseUI, 0.25f);

        FadeIn(HeadUpDisplayUI, 0.25f);

        ParallaxManager.Restart();

        gameHasEnded = IsPaused = false;
    }

    public void DisplayHighScore(string score)
    {
        HighScore.text = score;
        gameData.HighScore = int.Parse(score);
        SaveGame();
    }

    public void DisplayCountdown(string count)
    {
        CountdownText.text = count;
    }

    /// <summary>
    /// Fade In the Canvas Group attached to the gameObject UI
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="duration"></param>
    private void FadeIn(GameObject obj, float duration)
    {
        obj.SetActive(true);
        //return;
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        canvasGroup.DOFade(1f, duration).SetUpdate(true).OnComplete(() =>
        {
            canvasGroup.alpha = 1f;
        });
    }

    /// <summary>
    /// Fade Out the Canvas Group attached to the gameObject UI
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="duration"></param>
    private void FadeOut(GameObject obj, float duration)
    {
        obj.SetActive(false);
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;

        canvasGroup.DOFade(0f, duration).SetUpdate(true).OnComplete(() =>
        {
            canvasGroup.alpha = 0f;
            obj.SetActive(false);
        });
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!IsPaused && !gameHasEnded)
            if (!hasFocus)
                Pause();
    }

    public void DotweenWebsite()
    {
        Application.OpenURL("http://dotween.demigiant.com");
    }

    public void PayPalMe()
    {
        Application.OpenURL("https://www.paypal.com/qrcodes/managed/85bf0d78-1cd4-4570-97c9-7e85b37ccddb?utm_source=consapp_download");
    }
}
