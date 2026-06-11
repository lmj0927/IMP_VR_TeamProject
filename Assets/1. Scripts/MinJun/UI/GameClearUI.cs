using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameClearUI : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private TMP_Text gameClearText;

    public void Show()
    {
        gameObject.SetActive(true);
        fadeImage.DOFade(1, 1f).OnComplete(() => {
            gameClearText.gameObject.SetActive(true);
        });
    }
}
