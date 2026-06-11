using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameOverUI : MonoBehaviour
{

    [SerializeField] private Image fadeImage;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private Button retryButton;
    
    void Start()
    {
        retryButton.onClick.AddListener(GameManager.Instance.Retry);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        fadeImage.DOFade(1, 1f).OnComplete(() => {
            gameOverText.gameObject.SetActive(true);
            retryButton.gameObject.SetActive(true);
        });
    }
}
