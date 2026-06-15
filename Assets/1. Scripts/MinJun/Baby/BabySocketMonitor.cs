using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>Notifies BabyStateController when an item is attached to a socket.</summary>
[RequireComponent(typeof(XRSocketInteractor))]
public class BabySocketMonitor : MonoBehaviour
{
    BabyStateController m_Baby;
    XRSocketInteractor m_Socket;

    public void Initialize(BabyStateController baby)
    {
        m_Baby = baby;
    }

    void Awake()
    {
        m_Socket = GetComponent<XRSocketInteractor>();
    }

    void OnEnable()
    {
        m_Socket.selectEntered.AddListener(OnSelectEntered);
    }

    void OnDisable()
    {
        m_Socket.selectEntered.RemoveListener(OnSelectEntered);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (m_Baby == null)
            return;

        // Forward attach event to controller
        m_Baby.NotifySocketAttach(m_Socket, args.interactableObject);
    }
}
