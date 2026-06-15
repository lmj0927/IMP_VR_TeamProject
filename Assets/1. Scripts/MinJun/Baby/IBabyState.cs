using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>Defines Enter, Tick, and socket response contract for each baby need state.</summary>
public interface IBabyState
{
    void Enter(BabyStateContext context);
    void Tick(BabyStateContext context);
    void Exit(BabyStateContext context);
    void OnSocketAttach(BabyStateContext context, XRSocketInteractor socket, IXRSelectInteractable interactable);
    void OnSocketDetach(BabyStateContext context, XRSocketInteractor socket, IXRSelectInteractable interactable);
}
