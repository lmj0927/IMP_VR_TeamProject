using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private int GameLevel = 1;
    [SerializeField] private int RequiredClearState = 5;


    private int currentClearState = -1;
    public void ClearState()
    {
        currentClearState++;
        Debug.Log("Current Clear State: " + currentClearState);
        if (currentClearState >= RequiredClearState)
        {
            GameLevel++;
            currentClearState = 0;
            if (GameLevel == 2)
            {
                SceneManager.LoadScene("Level2");
            }
            else
                return;
        }
    }
}
