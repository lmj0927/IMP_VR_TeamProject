using System;
using UnityEngine;

/// <summary>
/// 구 프리팹 호환용. 새 분유병에는 BottleShakeController만 사용하세요.
/// </summary>
[Obsolete("Use BottleShakeController on the baby bottle prefab instead.")]
[DisallowMultipleComponent]
public class BottleCollide : MonoBehaviour
{
    BottleShakeController m_Controller;

    public bool BottleSet
    {
        get => ResolveController() != null && ResolveController().HasPowder;
        set
        {
            if (value || ResolveController() == null)
                return;

            Debug.LogWarning("[BottleCollide] BottleSet = false is not supported. Complete shaking via BottleShakeController.", this);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        var controller = ResolveController();
        if (controller != null)
        {
            controller.TryAddPowder(collision.gameObject);
            return;
        }

        if (!collision.gameObject.CompareTag("powder"))
            return;

        Destroy(collision.gameObject);
        Debug.LogWarning("[BottleCollide] No BottleShakeController found. Add BottleShakeController to the bottle prefab.", this);
    }

    BottleShakeController ResolveController()
    {
        if (m_Controller == null)
            m_Controller = GetComponent<BottleShakeController>() ?? GetComponentInParent<BottleShakeController>();

        return m_Controller;
    }
}
