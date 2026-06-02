using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TargetInteractionHandler : MonoBehaviour
{

    private bool floating = false;

    public void HandleHoverEnter(HoverEnterEventArgs args)
    {
        floating = true;
    }

    public void HandleHoverExit(HoverExitEventArgs args)
    {
        floating = false;
    }

    public void HandleActivate(ActivateEventArgs args)
    {
        Debug.Log("I'm dead after 3 seconds");
        Destroy(gameObject, 3);
    }

    private void FixedUpdate()
    {
        if (floating)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * 10f);
        }
    }

}
