using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>Displays fade effect and clear text on game clear.</summary>
public class GameClearUI : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private TMP_Text gameClearText;

    public void Show()
    {
        gameObject.SetActive(true);
        // Show clear text after fade completes
        fadeImage.DOFade(1, 1f).OnComplete(() => {
            gameClearText.gameObject.SetActive(true);
        });
    }
}
