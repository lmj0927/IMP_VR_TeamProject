using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private int gameLevel = 1;
    [SerializeField] private int requiredClearState = 5;
    [SerializeField] private GameObject nextLevelPotal;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private float gameTime = 300f;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private Image timeSliderFillImage;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private GameClearUI gameClearUI;
    public int GameLevel => gameLevel;
    private int currentClearState = -1;
    private GameState currentGameState = GameState.Processing;
    public GameState CurrentGameState => currentGameState;
    void Start()
    {
        timeSlider.maxValue = gameTime;
    }
    public void ClearState()
    {
        currentClearState++;

        if (currentClearState >= requiredClearState)
        {
            currentGameState = GameState.GameClear;

            gameLevel++;
            currentClearState = 0;
            if (gameLevel == 2)
            {
                nextLevelPotal.SetActive(true);
            }
        }
        UpdateDescriptionText();
    }

    public void NextLevel()
    {
        SceneManager.LoadScene("level 2");
    }

    void Update()
    {
        if (currentGameState == GameState.Processing)
        {
            gameTime -= Time.deltaTime;
            timeSlider.value = gameTime;
            if (gameTime <= 0)
            {
                gameOverUI.Show();
            }
        }
    }

    void UpdateDescriptionText()
    {
        if (currentGameState == GameState.Processing)
        {
            descriptionText.text = "The baby needs your help. Use the right items to calm them down." + "\n" + "Clear : " + currentClearState + " / " + requiredClearState;
        }
        else if (currentGameState == GameState.GameClear)
        {
            descriptionText.text = "The baby is calm now. Let's head to the bedroom.";
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameClear()
    {
        gameClearUI.Show();
    }
}

public enum GameState
{
    Processing,
    GameClear
}
