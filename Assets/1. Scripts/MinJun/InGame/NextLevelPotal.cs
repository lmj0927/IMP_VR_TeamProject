using UnityEngine;

/// <summary>Handles next level or final clear when player enters.</summary>
public class NextLevelPotal : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Branch by level
            if (GameManager.Instance.GameLevel == 1)
                GameManager.Instance.NextLevel();
            else if (GameManager.Instance.GameLevel == 2)
                GameManager.Instance.GameClear();
        }
    }
}
