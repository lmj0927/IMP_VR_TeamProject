using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>Manages baby need state transitions and socket interactions.</summary>
public class BabyStateController : MonoBehaviour
{
    [SerializeField] XRSocketInteractor m_DiaperSocket;
    [SerializeField] XRSocketInteractor m_MouthSocket;

    [Header("Timers")]
    [SerializeField] float m_IdleToNeedDelay = 15f;
    [SerializeField] float m_FeedDuration = 5f;
    [SerializeField] float m_SpitForce = 2f;

    readonly Dictionary<BabyNeedState, IBabyState> m_States = new();
    readonly BabyStateContext m_Context = new();

    IBabyState m_CurrentState;
    Coroutine m_OralFeedRoutine;

    [Header("현재 상태")]
    [SerializeField] BabyNeedState m_CurrentNeed = BabyNeedState.Idle;

    public BabyNeedState CurrentNeed => m_CurrentNeed;

    void Awake()
    {
        ResolveSockets();
        SetupSocketMonitors();
        RegisterSceneItems();

        // Inject sockets and timers into context
        m_Context.Controller = this;
        m_Context.DiaperSocket = m_DiaperSocket;
        m_Context.MouthSocket = m_MouthSocket;
        m_Context.IdleToNeedDelay = m_IdleToNeedDelay;
        m_Context.FeedDuration = m_FeedDuration;
        m_Context.SpitForce = m_SpitForce;

        // Register state instances
        m_States[BabyNeedState.Idle] = new BabyIdleState();
        m_States[BabyNeedState.Diaper] = new BabyDiaperState();
        m_States[BabyNeedState.Hungry] = new BabyOralNeedState(BabyItemKind.Bottle);
        m_States[BabyNeedState.Crying] = new BabyOralNeedState(BabyItemKind.Pacifier);
    }

    void Start()
    {
        ChangeState(BabyNeedState.Idle);
    }

    void Update()
    {
        m_CurrentState?.Tick(m_Context);
    }

    public void ChangeState(BabyNeedState next)
    {
        // Prevent re-entering same state
        if (m_CurrentState != null && m_CurrentNeed == next)
            return;

        m_CurrentState?.Exit(m_Context);
        m_CurrentNeed = next;
        m_CurrentState = m_States[next];
        m_CurrentState.Enter(m_Context);

        HandleStateAudio(next);

        Debug.Log($"[Baby] State -> {next}");
    }

    void HandleStateAudio(BabyNeedState next)
    {
        if (AudioManager.Instance == null)
            return;

        // Stop crying on Idle, play crying otherwise
        if (next == BabyNeedState.Idle)
            AudioManager.Instance.StopSound();
        else
            AudioManager.Instance.PlaySound(AudioType.Crying);
    }

    public void NotifySocketAttach(XRSocketInteractor socket, IXRSelectInteractable interactable)
    {
        // Reject if socket not allowed in current state
        if (!IsSocketInteractionAllowed(socket))
        {
            BabySpitHelper.RejectFromSocket(socket, interactable, m_SpitForce);
            return;
        }

        m_CurrentState?.OnSocketAttach(m_Context, socket, interactable);
    }

    public void ScheduleOralFeedComplete(XRSocketInteractor socket, IXRSelectInteractable interactable)
    {
        if (m_OralFeedRoutine != null)
            StopCoroutine(m_OralFeedRoutine);

        m_OralFeedRoutine = StartCoroutine(OralFeedCompleteRoutine(socket, interactable));
    }

    void OnDestroy()
    {
        if (m_OralFeedRoutine == null)
            return;

        StopCoroutine(m_OralFeedRoutine);
        m_OralFeedRoutine = null;
    }

    static bool TryGetFeedObject(IXRSelectInteractable interactable, out GameObject feedObject)
    {
        feedObject = null;
        if (interactable is not Component component)
            return false;

        feedObject = component.gameObject;
        return feedObject != null;
    }

    IEnumerator OralFeedCompleteRoutine(XRSocketInteractor socket, IXRSelectInteractable interactable)
    {
        if (!TryGetFeedObject(interactable, out var feedObject))
        {
            m_OralFeedRoutine = null;
            yield break;
        }

        yield return new WaitForSeconds(m_FeedDuration);

        if (feedObject == null || socket == null)
        {
            m_OralFeedRoutine = null;
            yield break;
        }

        // Spit out and remove item after feeding
        if (interactable is Object unityInteractable && unityInteractable != null)
            BabySpitHelper.RejectFromSocket(socket, interactable, m_SpitForce);

        if (feedObject != null)
            Destroy(feedObject, 1f);

        m_OralFeedRoutine = null;
    }

    bool IsSocketInteractionAllowed(XRSocketInteractor socket)
    {
        // Check allowed socket per state
        if (socket == m_DiaperSocket)
            return m_CurrentNeed == BabyNeedState.Diaper;

        if (socket == m_MouthSocket)
            return m_CurrentNeed == BabyNeedState.Hungry || m_CurrentNeed == BabyNeedState.Crying;

        return false;
    }

    void ResolveSockets()
    {
        // Find by name in children if not assigned in Inspector
        if (m_DiaperSocket == null || m_MouthSocket == null)
        {
            var sockets = GetComponentsInChildren<XRSocketInteractor>(true);
            foreach (var socket in sockets)
            {
                if (m_DiaperSocket == null && socket.name == "DiaperSocket")
                    m_DiaperSocket = socket;
                else if (m_MouthSocket == null && socket.name == "MouseSocket")
                    m_MouthSocket = socket;
            }
        }

        if (m_DiaperSocket == null)
            Debug.LogWarning("[Baby] DiaperSocket을 찾지 못했습니다.");
        if (m_MouthSocket == null)
            Debug.LogWarning("[Baby] MouseSocket(입)을 찾지 못했습니다.");
    }

    void SetupSocketMonitors()
    {
        RegisterSocketMonitor(m_DiaperSocket, this);
        RegisterSocketMonitor(m_MouthSocket, this);
    }

    static void RegisterSocketMonitor(XRSocketInteractor socket, BabyStateController baby)
    {
        if (socket == null || baby == null)
            return;

        var monitor = socket.GetComponent<BabySocketMonitor>();
        if (monitor == null)
            monitor = socket.gameObject.AddComponent<BabySocketMonitor>();

        monitor.Initialize(baby);
    }

    void RegisterSceneItems()
    {
        // Attach item type component to scene objects
        EnsureItem("Baby Bottle", BabyItemKind.Bottle);
        EnsureItem("Pacifiler", BabyItemKind.Pacifier);
        EnsureItem("BabyLunaClothDiaper", BabyItemKind.Diaper, addDiaperItem: true);
    }

    static void EnsureItem(string objectName, BabyItemKind kind, bool addDiaperItem = false)
    {
        var target = GameObject.Find(objectName);
        if (target == null)
            return;

        var itemType = target.GetComponent<BabyItemTypeComponent>();
        if (itemType == null)
            itemType = target.AddComponent<BabyItemTypeComponent>();
        itemType.Kind = kind;

        if (addDiaperItem && target.GetComponent<DiaperItem>() == null)
            target.AddComponent<DiaperItem>();
    }
}
