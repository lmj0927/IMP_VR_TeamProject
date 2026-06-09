using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public static class BabySpitHelper
{
    public static void RejectFromSocket(
        XRSocketInteractor socket,
        IXRSelectInteractable interactable,
        float spitForce)
    {
        if (socket == null || interactable == null)
            return;

        var manager = socket.interactionManager;
        if (manager == null)
            return;

        foreach (var selected in socket.interactablesSelected)
        {
            if (selected != interactable)
                continue;

            manager.SelectExit(socket, interactable);
            break;
        }

        ApplySpitForce(interactable, socket.transform.position, spitForce);
    }

    static void ApplySpitForce(IXRSelectInteractable interactable, Vector3 mouthPosition, float spitForce)
    {
        var rb = interactable.transform.GetComponent<Rigidbody>();
        if (rb == null)
            return;

        rb.isKinematic = false;
        var direction = (interactable.transform.position - mouthPosition).normalized;
        if (direction.sqrMagnitude < 0.001f)
            direction = interactable.transform.forward;

        rb.AddForce(direction * spitForce + Vector3.up * (spitForce * 0.25f), ForceMode.Impulse);
    }
}
