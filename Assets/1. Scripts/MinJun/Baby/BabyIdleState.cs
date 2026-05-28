using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

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
    }

    public void Tick(BabyStateContext context)
    {
        m_Timer -= Time.deltaTime;
        if (m_Timer > 0f)
            return;

        var next = NeedStates[Random.Range(0, NeedStates.Length)];
        context.Controller.ChangeState(next);
    }

    public void Exit(BabyStateContext context) { }

    public void OnSocketAttach(BabyStateContext context, XRSocketInteractor socket, IXRSelectInteractable interactable) { }

    public void OnSocketDetach(BabyStateContext context, XRSocketInteractor socket, IXRSelectInteractable interactable) { }
}
