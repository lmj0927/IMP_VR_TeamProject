using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BabyOralNeedState : IBabyState
{
    readonly BabyItemKind m_ExpectedItem;
    bool m_IsFeeding;

    public BabyOralNeedState(BabyItemKind expectedItem)
    {
        m_ExpectedItem = expectedItem;
    }

    public void Enter(BabyStateContext context)
    {
        m_IsFeeding = false;
    }

    public void Tick(BabyStateContext context) { }

    public void Exit(BabyStateContext context) { }

    public void OnSocketAttach(BabyStateContext context, XRSocketInteractor socket, IXRSelectInteractable interactable)
    {
        if (socket != context.MouthSocket || m_IsFeeding)
            return;

        if (!BabyItemResolver.TryGetKind(interactable, out var kind) || kind != m_ExpectedItem)
        {
            BabySpitHelper.RejectFromSocket(socket, interactable, context.SpitForce);
            return;
        }

        m_IsFeeding = true;
        context.Controller.ScheduleOralFeedComplete(socket, interactable);
        context.Controller.ChangeState(BabyNeedState.Idle);
    }

    public void OnSocketDetach(BabyStateContext context, XRSocketInteractor socket, IXRSelectInteractable interactable) { }
}

public enum BabyNeedState
{
    Idle,
    Diaper,
    Hungry,
    Crying
}