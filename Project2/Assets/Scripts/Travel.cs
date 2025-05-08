using UnityEngine;
using UnityEngine.InputSystem;

public class Travel : MonoBehaviour
{
    [Header("Gesture Input")]
    [Tooltip("Bind this to your pinch/grip InputAction (e.g. XRController Pinch).")]
    [SerializeField] private InputActionReference pinchAction;

    [Header("References")]
    [Tooltip("Drag in the IndexTip joint Transform from your XR Hand Skeleton Driver.")]
    [SerializeField] private Transform indexTip;

    [Tooltip("Your XR Rig root Transform (the thing you want to move).")]
    [SerializeField] private Transform xrRig;

    [Header("Movement Settings")]
    [Tooltip("World‐space units per second.")]
    [SerializeField] private float movementSpeed = 10f;

    [Tooltip("Higher = snappier; Lower = more glide.")]
    [SerializeField] private float smoothing = 5f;

    private bool isPinching;

    private void OnEnable()
    {
        pinchAction.action.performed  += OnPinchStarted;
        pinchAction.action.canceled += OnPinchCanceled;
        pinchAction.action.Enable();
    }

    private void OnDisable()
    {
        pinchAction.action.performed  -= OnPinchStarted;
        pinchAction.action.canceled -= OnPinchCanceled;
        pinchAction.action.Disable();
    }

    private void OnPinchStarted(InputAction.CallbackContext ctx)
    {
        isPinching = true;
    }

    private void OnPinchCanceled(InputAction.CallbackContext ctx)
    {
        isPinching = false;
    }

    private void Update()
    {
        if (!isPinching || indexTip == null || xrRig == null)
            return;

        // 1) Compute target direction from the index‑tip
        Vector3 dir = indexTip.forward.normalized;

        // 2) Compute smoothed new position
        Vector3 desiredPos = xrRig.position + dir * movementSpeed * Time.deltaTime;
        xrRig.position = Vector3.Lerp(xrRig.position, desiredPos, smoothing * Time.deltaTime);
    }
}
