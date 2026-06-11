using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BabyDiaperState : IBabyState
{
    public void Enter(BabyStateContext context) {}

    public void Tick(BabyStateContext context) { }

    public void Exit(BabyStateContext context) { MarkCurrentDiaperDirty(context); }

    public void OnSocketAttach(BabyStateContext context, XRSocketInteractor socket, IXRSelectInteractable interactable)
    {
        if (socket != context.DiaperSocket)
            return;

        if (!BabyItemResolver.TryGetKind(interactable, out var kind) || kind != BabyItemKind.Diaper)
        {
            BabySpitHelper.RejectFromSocket(socket, interactable, context.SpitForce);
            return;
        }

        if (!BabyItemResolver.IsCleanDiaper(interactable))
        {
            BabySpitHelper.RejectFromSocket(socket, interactable, context.SpitForce);
            return;
        }

        context.Controller.ChangeState(BabyNeedState.Idle);
    }

    public void OnSocketDetach(BabyStateContext context, XRSocketInteractor socket, IXRSelectInteractable interactable) { }

    static void MarkCurrentDiaperDirty(BabyStateContext context)
    {
        if (context.DiaperSocket != null)
        {
            foreach (var interactable in context.DiaperSocket.interactablesSelected)
            {
                var diaper = interactable.transform.GetComponentInParent<DiaperItem>();
                if (diaper != null)
                    diaper.SetClean(false);
            }
        }

        if (context.Controller == null)
            return;

        var wornDiapers = context.Controller.GetComponentsInChildren<DiaperItem>(true);
        foreach (var diaper in wornDiapers)
            diaper.SetClean(false);
    }
}
