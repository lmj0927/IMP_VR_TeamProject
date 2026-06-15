using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>Triggers a random need after a timer in calm state.</summary>
public class BabyIdleState : IBabyState
{
    static readonly BabyNeedState[] NeedStates =
    {
        BabyNeedState.Diaper,
        BabyNeedState.Hungry,
        BabyNeedState.Crying
    };

    float m_Timer;

    public void Enter(BabyStateContext context)
    {
        m_Timer = context.IdleToNeedDelay;
        // Increment clear count when baby calms down
        GameManager.Instance.ClearState();
    }

    public void Tick(BabyStateContext context)
    {
        if (GameManager.Instance.CurrentGameState == GameState.GameClear)
            return;
        m_Timer -= Time.deltaTime;
        if (m_Timer > 0f)
            return;

        // Transition to random need state
        var next = NeedStates[Random.Range(0, NeedStates.Length)];
        context.Controller.ChangeState(next);
    }

    public void Exit(BabyStateContext context) { }

    public void OnSocketAttach(BabyStateContext context, XRSocketInteractor socket, IXRSelectInteractable interactable) { }

    public void OnSocketDetach(BabyStateContext context, XRSocketInteractor socket, IXRSelectInteractable interactable) { }
}
