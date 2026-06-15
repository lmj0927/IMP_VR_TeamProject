using UnityEngine;

/// <summary>Tracks whether a diaper is clean or dirty.</summary>
public class DiaperItem : MonoBehaviour
{
    private bool isClean = true;
    public bool IsClean => isClean;

    // Mark as dirty after use
    public void SetClean(bool value)
    {
        isClean = value;
    }
}
