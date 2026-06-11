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
        if (controller == null)
            return;

        // mantle 탈출을 이미 마친 뒤, 위쪽 구역에서만 재그립 클라이밍 차단
        if (m_ExitSequenceComplete && IsAboveExitZone())
            return;

        Collider[] colliders = Physics.OverlapSphere(controller.position, grabRadius, climbableLayer);
        if (colliders.Length > 0)
            StartClimbing(controller);
    }

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

    void StartClimbing(Transform controller)
    {
        m_StartedBelowExitZone = !IsAboveExitZone();
        if (m_StartedBelowExitZone)
            m_ExitSequenceComplete = false;

        m_ClimbStartHeight = transform.position.y;
        isClimbing = true;
        activeController = controller;
        lastControllerLocalPosition = controller.localPosition;

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

        Vector3 currentControllerLocalPosition = activeController.localPosition;
        Vector3 localDelta = currentControllerLocalPosition - lastControllerLocalPosition;
        Vector3 worldMoveDirection = xrOrigin.TransformDirection(-localDelta);
        Vector3 verticalMove = new Vector3(0, worldMoveDirection.y, 0);

        characterController.Move(verticalMove);

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

        if (continuousMoveProvider != null)
            continuousMoveProvider.enabled = true;
    }

    void TriggerLedgeExit()
    {
        if (m_ExitSequenceComplete || exitPoint == null)
            return;

        m_ExitSequenceComplete = true;
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