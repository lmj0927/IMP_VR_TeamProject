using UnityEngine;

/// <summary>
/// 새 기저귀(IsClean=true)를 DiaperSocket에 끼우면 기저귀 갈기 완료로 처리합니다.
/// </summary>
public class DiaperItem : MonoBehaviour
{
    private bool isClean = true;
    public bool IsClean => isClean;
    public void SetClean(bool value)
    {
        isClean = value;
    }
}
