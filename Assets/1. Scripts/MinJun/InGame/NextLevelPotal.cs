using UnityEngine;

public class NextLevelPotal : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance.GameLevel == 1)
                GameManager.Instance.NextLevel();
            else if (GameManager.Instance.GameLevel == 2)
                GameManager.Instance.GameClear();
        }
    }
}
