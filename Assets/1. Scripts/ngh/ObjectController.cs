using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// 구 씬 호환용 빈 껍데기. 분유병 프리팹에는 BottleShakeController를 붙이세요.
/// </summary>
[Obsolete("Attach BottleShakeController to the baby bottle prefab instead.")]
public class ObjectController : MonoBehaviour
{
    [SerializeField] GameObject m_Cube;

    public void HandleSelectEnter(SelectEnterEventArgs args)
    {
        Debug.LogWarning("[ObjectController] HandleSelectEnter is obsolete. Use BottleShakeController on the bottle prefab.", this);
    }

    public void CanSelectEnter(SelectEnterEventArgs args)
    {
        if (m_Cube == null || args.interactableObject == null)
            return;

        var position = args.interactableObject.transform.position;
        Instantiate(m_Cube, new Vector3(position.x, position.y + 0.02f, position.z), Quaternion.identity);
    }
}
