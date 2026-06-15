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
    [SerializeField] private float exitCheckHeightOffset = 0.3f; 
    [SerializeField] private bool useAutoMantle = true; 

    private Transform activeController;
    private Vector3 lastControllerLocalPosition;
    private bool isClimbing = false;
    private bool leftWasPressed = false;
    private bool rightWasPressed = false;
    private bool m_ExitSequenceComplete;
    private float m_ClimbStartHeight;
    private bool m_StartedBelowExitZone;

    // --- INPUT & STATE DISPATCHER ---
    void Update()
    {
        float leftGripValue = leftGripAction.action.ReadValue<float>();
        float rightGripValue = rightGripAction.action.ReadValue<float>();

        bool leftIsInGrip = leftGripValue >= gripThreshold;
        bool rightIsInGrip = rightGripValue >= gripThreshold;

        // Switches between climbing locomotion and grab detection
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

    // --- GRAB & DETECT LOGIC ---
    void CheckGrab(Transform controller)
    {
        if (controller == null)
            return;

        // Disallow climbing re-triggering if already successfully mantled
        if (m_ExitSequenceComplete && IsAboveExitZone())
            return;

        Collider[] colliders = Physics.OverlapSphere(controller.position, grabRadius, climbableLayer);
        if (colliders.Length > 0)
            StartClimbing(controller);
    }
    // --- CONDITIONS FOR LEDGE EXIT (MANTLE) ---
    bool IsAboveExitZone()
    {
        return exitPoint != null
            && transform.position.y >= exitPoint.position.y - exitCheckHeightOffset;
    }

    bool ShouldTriggerMantle(float verticalMoveY)
    {
        if (m_ExitSequenceComplete || exitPoint == null || !useAutoMantle || !IsAboveExitZone())
            return false;

        if (!m_StartedBelowExitZone)
            return verticalMoveY > 0f;

        bool movedUpThisFrame = verticalMoveY > 0f;
        bool climbedEnoughThisSession = transform.position.y >= m_ClimbStartHeight + exitCheckHeightOffset * 0.5f;

        return movedUpThisFrame || climbedEnoughThisSession;
    }
    // --- CLIMBING LIFECYCLE MANAGEMENT ---
    void StartClimbing(Transform controller)
    {
        m_StartedBelowExitZone = !IsAboveExitZone();
        if (m_StartedBelowExitZone)
            m_ExitSequenceComplete = false;

        m_ClimbStartHeight = transform.position.y;
        isClimbing = true;
        activeController = controller;
        lastControllerLocalPosition = controller.localPosition;

        // Disable ground movement to prevent physics interference
        if (continuousMoveProvider != null)
            continuousMoveProvider.enabled = false;
    }

    void ContinueClimbing(bool leftIsInGrip, bool rightIsInGrip)
    {
        
        if (activeController == null || xrOrigin == null || characterController == null)
        {
            EndClimbing();
            return;
        }

        bool isActiveHandStillGripping = (activeController == leftController) ? leftIsInGrip : rightIsInGrip;
        // Handle hand-to-hand switching or release
        if (!isActiveHandStillGripping)
        {
            Transform otherController = (activeController == leftController) ? rightController : leftController;
            bool otherIsInGrip = (activeController == leftController) ? rightIsInGrip : leftIsInGrip;

            if (otherController != null)
            {
                Collider[] colliders = Physics.OverlapSphere(otherController.position, grabRadius, climbableLayer);
                if (otherIsInGrip && colliders.Length > 0)
                {
                    StartClimbing(otherController);
                    return;
                }
            }

            EndClimbing();
            return;
        }
        
        // Calculate hand movement delta and translate into player vertical movement
        Vector3 currentControllerLocalPosition = activeController.localPosition;
        Vector3 localDelta = currentControllerLocalPosition - lastControllerLocalPosition;
        Vector3 worldMoveDirection = xrOrigin.TransformDirection(-localDelta);
        Vector3 verticalMove = new Vector3(0, worldMoveDirection.y, 0);

        characterController.Move(verticalMove);
        // Process automatic vaulting over the ledge
        if (useAutoMantle && ShouldTriggerMantle(verticalMove.y))
        {
            TriggerLedgeExit();
            return;
        }

        lastControllerLocalPosition = activeController.localPosition;
    }

    void EndClimbing()
    {
        isClimbing = false;
        activeController = null;
        // Restore normal ground movement
        if (continuousMoveProvider != null)
            continuousMoveProvider.enabled = true;
    }
// --- TELEPORTATION TO LEDGE TOP ---
    void TriggerLedgeExit()
    {
        if (m_ExitSequenceComplete || exitPoint == null)
            return;

        m_ExitSequenceComplete = true;
        isClimbing = false;
        activeController = null;
        
        // Warp player to the top of the platform and restore movement
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
