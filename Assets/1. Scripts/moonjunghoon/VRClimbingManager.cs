using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

public class VRClimbingManager : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform xrOrigin;

    [Header("Controller Hands")]
    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;

    [Header("Input Actions (Grip)")]
    [SerializeField] private InputActionProperty leftGripAction;
    [SerializeField] private InputActionProperty rightGripAction;

    [Header("Climbing Settings")]
    [SerializeField] private LayerMask climbableLayer;
    [SerializeField] private float grabRadius = 0.12f;
    [Range(0f, 1f)] [SerializeField] private float gripThreshold = 0.2f;

    [Header("XRI Locomotion Interfere")]
    [SerializeField] private ContinuousMoveProvider continuousMoveProvider;

    [Header("★ Super Generous Ledge Exit ★")]
    [SerializeField] private Transform exitPoint; 
    [SerializeField] private float exitCheckHeightOffset = 0.8f; 
    [SerializeField] private bool useAutoMantle = true; 

    private Transform activeController;
    private Vector3 lastControllerLocalPosition;
    private bool isClimbing = false;
    private bool leftWasPressed = false;
    private bool rightWasPressed = false;

    void Update()
    {
        float leftGripValue = leftGripAction.action.ReadValue<float>();
        float rightGripValue = rightGripAction.action.ReadValue<float>();

        bool leftIsInGrip = leftGripValue >= gripThreshold;
        bool rightIsInGrip = rightGripValue >= gripThreshold;

        if (isClimbing)
        {
            ContinueClimbing(leftIsInGrip, rightIsInGrip);
        }
        else
        {
            if (leftIsInGrip && !leftWasPressed) CheckGrab(leftController);
            if (rightIsInGrip && !rightWasPressed) CheckGrab(rightController);
        }

        leftWasPressed = leftIsInGrip;
        rightWasPressed = rightIsInGrip;
    }

    void CheckGrab(Transform controller)
    {
        Collider[] colliders = Physics.OverlapSphere(controller.position, grabRadius, climbableLayer);
        if (colliders.Length > 0)
        {
            StartClimbing(controller);
        }
    }

    void StartClimbing(Transform controller)
    {
        isClimbing = true;
        activeController = controller;
        lastControllerLocalPosition = controller.localPosition;

        if (continuousMoveProvider != null) 
            continuousMoveProvider.enabled = false;
    }

    void ContinueClimbing(bool leftIsInGrip, bool rightIsInGrip)
    {
        bool isActiveHandStillGripping = (activeController == leftController) ? leftIsInGrip : rightIsInGrip;

        
        if (useAutoMantle && exitPoint != null)
        {
           
            if (transform.position.y >= (exitPoint.position.y - exitCheckHeightOffset))
            {
                TriggerLedgeExit();
                return;
            }
        }

        if (!isActiveHandStillGripping)
        {
            Transform otherController = (activeController == leftController) ? rightController : leftController;
            bool otherIsInGrip = (activeController == leftController) ? rightIsInGrip : leftIsInGrip;

            Collider[] colliders = Physics.OverlapSphere(otherController.position, grabRadius, climbableLayer);
            if (otherIsInGrip && colliders.Length > 0)
            {
                StartClimbing(otherController);
                return;
            }

            EndClimbing();
            return;
        }

       
        Vector3 currentControllerLocalPosition = activeController.localPosition;
        Vector3 localDelta = currentControllerLocalPosition - lastControllerLocalPosition;
        Vector3 worldMoveDirection = xrOrigin.TransformDirection(-localDelta);
        Vector3 verticalMove = new Vector3(0, worldMoveDirection.y, 0); 
        
        characterController.Move(verticalMove);

        lastControllerLocalPosition = activeController.localPosition;
    }

    void EndClimbing()
    {
      
        if (exitPoint != null && transform.position.y >= (exitPoint.position.y - (exitCheckHeightOffset * 1.5f)))
        {
            TriggerLedgeExit();
            return;
        }

        isClimbing = false;
        activeController = null;

        if (continuousMoveProvider != null) 
            continuousMoveProvider.enabled = true;
    }

   
    void TriggerLedgeExit()
    {
        isClimbing = false;
        activeController = null;


        characterController.enabled = false;
        transform.position = exitPoint.position;
        characterController.enabled = true;

        if (continuousMoveProvider != null) 
            continuousMoveProvider.enabled = true;

     
    }

    private void OnDrawGizmosSelected()
    {
        if (leftController != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(leftController.position, grabRadius);
        }
        if (rightController != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(rightController.position, grabRadius);
        }
    }
}