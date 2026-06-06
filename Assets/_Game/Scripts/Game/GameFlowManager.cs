using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    public enum GameState
    {
        Countdown,
        Playing,
        Paused,
        Success,
        Failed
    }

    [Header("任务设置")]
    public int targetKillCount = 50;
    public float countdownTime = 3f;

    [Header("场景引用")]
    public PlayerHealth playerHealth;
    public EnemySpawnManager enemySpawnManager;
    public DualViewPlayerController playerController;

    [Header("运行状态")]
    public GameState currentState = GameState.Countdown;
    public int currentKillCount = 0;
    public float currentCountdown = 3f;

    [Header("鼠标灵敏度设置")]
    public float minMouseSensitivity = 0.5f;
    public float maxMouseSensitivity = 6f;

    [Header("UI 样式")]
    public int hudFontSize = 24;
    public int centerFontSize = 36;
    public int buttonFontSize = 26;

    private float countdownEndRealtime;

    private GUIStyle hudStyle;
    private GUIStyle centerStyle;
    private GUIStyle buttonStyle;
    private GUIStyle menuTitleStyle;
    private GUIStyle sliderLabelStyle;

    public bool IsPlaying
    {
        get { return currentState == GameState.Playing; }
    }

    public bool IsPaused
    {
        get { return currentState == GameState.Paused; }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<DualViewPlayerController>();
        }

        StartCountdown();
    }

    void StartCountdown()
    {
        currentKillCount = 0;
        currentState = GameState.Countdown;

        currentCountdown = countdownTime;
        countdownEndRealtime = Time.realtimeSinceStartup + countdownTime;

        Time.timeScale = 0f;

        UnlockCursor();
    }

    void Update()
    {
        if (currentState == GameState.Countdown)
        {
            currentCountdown = countdownEndRealtime - Time.realtimeSinceStartup;

            if (currentCountdown <= 0f)
            {
                currentCountdown = 0f;
                StartGame();
            }

            return;
        }

        // 游戏进行中按 Esc 打开暂停菜单
        if (currentState == GameState.Playing && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
            return;
        }

        // 暂停菜单中再次按 Esc，继续游戏
        if (currentState == GameState.Paused && Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGame();
            return;
        }
    }

    void StartGame()
    {
        currentState = GameState.Playing;

        Time.timeScale = 1f;

        LockCursor();

        Debug.Log("游戏正式开始。");
    }

    public void PauseGame()
    {
        if (currentState != GameState.Playing)
        {
            return;
        }

        currentState = GameState.Paused;

        Time.timeScale = 0f;

        UnlockCursor();

        Debug.Log("游戏暂停。");
    }

    public void ResumeGame()
    {
        if (currentState != GameState.Paused)
        {
            return;
        }

        currentState = GameState.Playing;

        Time.timeScale = 1f;

        LockCursor();

        Debug.Log("继续游戏。");
    }

    public void OnEnemyKilled()
    {
        if (currentState != GameState.Playing)
        {
            return;
        }

        currentKillCount++;

        Debug.Log($"当前击杀数：{currentKillCount}/{targetKillCount}");

        if (currentKillCount >= targetKillCount)
        {
            MissionSuccess();
        }
    }

    public void OnPlayerDead()
    {
        if (currentState != GameState.Playing)
        {
            return;
        }

        MissionFailed();
    }

    void MissionSuccess()
    {
        currentState = GameState.Success;

        Time.timeScale = 0f;

        UnlockCursor();

        Debug.Log("任务成功！");
    }

    void MissionFailed()
    {
        currentState = GameState.Failed;

        Time.timeScale = 0f;

        UnlockCursor();

        Debug.Log("任务失败！");
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnGUI()
    {
        InitStyles();

        if (currentState == GameState.Playing)
        {
            DrawHUD();
        }
        else if (currentState == GameState.Countdown)
        {
            DrawCountdownUI();
        }
        else if (currentState == GameState.Paused)
        {
            DrawPauseMenuUI();
        }
        else if (currentState == GameState.Success)
        {
            DrawResultUI("任务成功！", "你已经成功击败废弃厂区 50 个敌人。");
        }
        else if (currentState == GameState.Failed)
        {
            DrawResultUI("任务失败！", "玩家生命值归零，行动失败。");
        }
    }

    void InitStyles()
    {
        if (hudStyle == null)
        {
            hudStyle = new GUIStyle(GUI.skin.label);
            hudStyle.fontSize = hudFontSize;
            hudStyle.normal.textColor = Color.white;
        }

        if (centerStyle == null)
        {
            centerStyle = new GUIStyle(GUI.skin.label);
            centerStyle.fontSize = centerFontSize;
            centerStyle.alignment = TextAnchor.MiddleCenter;
            centerStyle.normal.textColor = Color.white;
            centerStyle.wordWrap = true;
        }

        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = buttonFontSize;
        }

        if (menuTitleStyle == null)
        {
            menuTitleStyle = new GUIStyle(GUI.skin.label);
            menuTitleStyle.fontSize = 42;
            menuTitleStyle.alignment = TextAnchor.MiddleCenter;
            menuTitleStyle.normal.textColor = Color.white;
        }

        if (sliderLabelStyle == null)
        {
            sliderLabelStyle = new GUIStyle(GUI.skin.label);
            sliderLabelStyle.fontSize = 24;
            sliderLabelStyle.normal.textColor = Color.white;
        }
    }

    void DrawHUD()
    {
        float health = 0f;
        float maxHealth = 100f;

        if (playerHealth != null)
        {
            health = playerHealth.currentHealth;
            maxHealth = playerHealth.maxHealth;
        }

        int aliveEnemies = 0;

        if (enemySpawnManager != null)
        {
            aliveEnemies = enemySpawnManager.currentAliveCount;
        }

        GUI.Label(new Rect(20, 20, 500, 40), $"生命值：{health:0}/{maxHealth:0}", hudStyle);

        DrawHealthBar(
            new Rect(20, 55, 260, 24),
            health,
            maxHealth
        );

        GUI.Label(new Rect(20, 90, 500, 40), $"任务进度：{currentKillCount}/{targetKillCount}", hudStyle);
        GUI.Label(new Rect(20, 125, 500, 40), $"当前敌人数：{aliveEnemies}/10", hudStyle);
    }

    void DrawHealthBar(Rect rect, float current, float max)
    {
        float percent = 0f;

        if (max > 0f)
        {
            percent = Mathf.Clamp01(current / max);
        }

        GUI.color = new Color(0.15f, 0.15f, 0.15f, 0.85f);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);

        Color healthColor = Color.Lerp(Color.red, Color.green, percent);
        GUI.color = healthColor;

        Rect fillRect = new Rect(
            rect.x,
            rect.y,
            rect.width * percent,
            rect.height
        );

        GUI.DrawTexture(fillRect, Texture2D.whiteTexture);

        GUI.color = Color.white;
        GUI.Box(rect, "");
    }

    void DrawCountdownUI()
    {
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

        string text =
            "任务开始！\n" +
            "任务目标为击败废弃厂区 50 个敌人\n\n" +
            $"倒计时：{Mathf.CeilToInt(currentCountdown)}";

        GUI.Label(
            new Rect(Screen.width / 2f - 500, Screen.height / 2f - 180, 1000, 360),
            text,
            centerStyle
        );
    }

    void DrawPauseMenuUI()
    {
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

        float panelWidth = 620f;
        float panelHeight = 460f;
        float panelX = Screen.width / 2f - panelWidth / 2f;
        float panelY = Screen.height / 2f - panelHeight / 2f;

        GUI.Box(new Rect(panelX, panelY, panelWidth, panelHeight), "");

        GUI.Label(
            new Rect(panelX, panelY + 35, panelWidth, 60),
            "游戏暂停",
            menuTitleStyle
        );

        if (GUI.Button(
            new Rect(panelX + 190, panelY + 120, 240, 60),
            "继续游戏",
            buttonStyle))
        {
            ResumeGame();
        }

        DrawMouseSensitivitySlider(panelX, panelY);

        if (GUI.Button(
            new Rect(panelX + 190, panelY + 300, 240, 60),
            "重新开始",
            buttonStyle))
        {
            RestartGame();
        }

        if (GUI.Button(
            new Rect(panelX + 190, panelY + 375, 240, 60),
            "退出游戏",
            buttonStyle))
        {
            QuitGame();
        }
    }

    void DrawMouseSensitivitySlider(float panelX, float panelY)
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<DualViewPlayerController>();
        }

        float currentSensitivity = 2f;

        if (playerController != null)
        {
            currentSensitivity = playerController.mouseSensitivity;
        }

        GUI.Label(
            new Rect(panelX + 120, panelY + 205, 380, 35),
            $"鼠标灵敏度：{currentSensitivity:0.0}",
            sliderLabelStyle
        );

        float newSensitivity = GUI.HorizontalSlider(
            new Rect(panelX + 120, panelY + 250, 380, 30),
            currentSensitivity,
            minMouseSensitivity,
            maxMouseSensitivity
        );

        if (playerController != null)
        {
            playerController.mouseSensitivity = newSensitivity;
        }
    }

    void DrawResultUI(string title, string description)
    {
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

        string text =
            title + "\n\n" +
            description + "\n\n" +
            $"最终击杀数：{currentKillCount}/{targetKillCount}";

        GUI.Label(
            new Rect(Screen.width / 2f - 500, Screen.height / 2f - 240, 1000, 260),
            text,
            centerStyle
        );

        float buttonWidth = 240;
        float buttonHeight = 65;
        float centerX = Screen.width / 2f;
        float startY = Screen.height / 2f + 90;

        if (GUI.Button(
            new Rect(centerX - buttonWidth - 30, startY, buttonWidth, buttonHeight),
            "重新开始",
            buttonStyle))
        {
            RestartGame();
        }

        if (GUI.Button(
            new Rect(centerX + 30, startY, buttonWidth, buttonHeight),
            "退出游戏",
            buttonStyle))
        {
            QuitGame();
        }
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        Instance = null;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}