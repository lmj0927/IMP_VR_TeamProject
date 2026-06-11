using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


[RequireComponent(typeof(XRGrabInteractable))]
[DisallowMultipleComponent]
public class BottleShakeController : MonoBehaviour
{
    [Header("Powder")]
    [SerializeField] string m_PowderTag = "powder";

    [Header("Shake")]
    [SerializeField] float m_ShakeDistance = 0.1f;
    [SerializeField] int m_RequiredShakeCount = 12;

    [Header("Socket")]
    [Tooltip("Baby mouth socket interaction layer (MouseSocket uses bit 4).")]
    [SerializeField] InteractionLayerMask m_SocketInteractionLayer = 4;

    XRGrabInteractable m_GrabInteractable;
    InteractionLayerMask m_InteractionLayersWhenSocketReady;

    bool m_IsGrabbed;
    bool m_HasPowder;
    bool m_IsShakeComplete;
    Vector3 m_GrabReferencePosition;
    bool m_MovedUp;
    bool m_MovedDown;
    int m_ShakeCount;

    /// <summary>
    /// 가루가 들어가 흔들기 전 상태입니다. (구 BottleCollide.BottleSet == true)
    /// </summary>
    public bool HasPowder => m_HasPowder;

    public int ShakeCount => m_ShakeCount;

    /// <summary>
    /// 흔들기가 끝나 입 소켓에 붙일 수 있는 상태인지 여부입니다.
    /// </summary>
    public bool CanAttachToSocket => m_IsShakeComplete;

    public event Action OnPowderAdded;
    public event Action OnShakeComplete;

    void Awake()
    {
        m_GrabInteractable = GetComponent<XRGrabInteractable>();
        m_InteractionLayersWhenSocketReady = m_GrabInteractable.interactionLayers | m_SocketInteractionLayer;
        SetSocketAttachAllowed(false);
    }

    void OnEnable()
    {
        m_GrabInteractable.selectEntered.AddListener(OnSelectEntered);
        m_GrabInteractable.selectExited.AddListener(OnSelectExited);
    }

    void OnDisable()
    {
        m_GrabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        m_GrabInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    void Update()
    {
        if (!m_IsGrabbed || !m_HasPowder)
            return;

        UpdateShakeCount();
    }

    void OnCollisionEnter(Collision collision)
    {
        TryAddPowder(collision.gameObject);
    }

    public void TryAddPowder(GameObject powderObject)
    {
        if (powderObject == null || !powderObject.CompareTag(m_PowderTag))
            return;

        Destroy(powderObject);
        m_HasPowder = true;
        m_ShakeCount = 0;
        ResetShakeCycle();

        Debug.Log("[Bottle] Powder added.");
        OnPowderAdded?.Invoke();
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        m_IsGrabbed = true;
        m_GrabReferencePosition = transform.position;
        ResetShakeCycle();
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        m_IsGrabbed = false;
        ResetShakeCycle();
    }

    void UpdateShakeCount()
    {
        var deltaY = transform.position.y - m_GrabReferencePosition.y;
        Debug.Log($"deltaY: {deltaY:F3}");
        if (deltaY >= m_ShakeDistance)
        {
            if (!m_MovedUp)
                Debug.Log($"[Bottle] Shake up (deltaY: {deltaY:F3})");
            m_MovedUp = true;
            m_GrabReferencePosition = transform.position;
        }
        else if (m_MovedUp && deltaY <= -m_ShakeDistance)
        {
            if (!m_MovedDown)
                Debug.Log($"[Bottle] Shake down (deltaY: {deltaY:F3})");
            m_MovedDown = true;
            m_GrabReferencePosition = transform.position;
        }

        if (!m_MovedUp || !m_MovedDown)
            return;

        m_ShakeCount++;
        Debug.Log($"[Bottle] Shake {m_ShakeCount}/{m_RequiredShakeCount}");
        ResetShakeCycle();

        if (m_ShakeCount < m_RequiredShakeCount)
            return;

        m_HasPowder = false;
        m_ShakeCount = 0;
        SetSocketAttachAllowed(true);
        Debug.Log("[Bottle] Shake complete. Socket attach enabled.");
        OnShakeComplete?.Invoke();
    }

    void SetSocketAttachAllowed(bool allowed)
    {
        m_IsShakeComplete = allowed;
        m_GrabInteractable.interactionLayers = allowed
            ? m_InteractionLayersWhenSocketReady
            : m_InteractionLayersWhenSocketReady & ~m_SocketInteractionLayer;
    }

    void ResetShakeCycle()
    {
        m_MovedUp = false;
        m_MovedDown = false;
    }
}
