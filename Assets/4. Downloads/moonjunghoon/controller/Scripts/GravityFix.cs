using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;

public class GravityFix : MonoBehaviour
{
    private CharacterController characterController;
    private ClimbProvider climbProvider;
    private bool forceGravityCheck = true; // whether to force gravity check on the character controller


     void Awake()
    {
        characterController = FindAnyObjectByType<CharacterController>();
        climbProvider = GetComponent<ClimbProvider>();

    }

    // Update is called once per frame
    void Update()
    {
        if(forceGravityCheck)
        {
            characterController.SimpleMove(Vector3.zero); // even zero movement forces gravity check!
        }
    }

    private void OnEnable()
    {
        // subscribe to locomotion events
        climbProvider.locomotionStarted += LocomotionStarted;
        climbProvider.locomotionEnded += LocomotionEnded;
    }

    private void OnDisable()
    {
        // unsubscribe from locomotion events
        climbProvider.locomotionStarted -= LocomotionStarted;
        climbProvider.locomotionEnded -= LocomotionEnded;
    }


    private void LocomotionEnded(LocomotionProvider provider)
    {
        forceGravityCheck = true;
    }
    private void LocomotionStarted(LocomotionProvider provider)
    {
        forceGravityCheck = false;
    }

}
