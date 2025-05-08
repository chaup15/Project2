using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Hands;

public class Travel : MonoBehaviour
{
    [Header("Gesture & Hand")]
    [SerializeField] InputActionReference pinchAction;
    [SerializeField] XRHandSubsystem   handSubsystem;
    [SerializeField] XRNode            handNode       = XRNode.LeftHand;

    [Header("Rig & Motion")]
    [SerializeField] Transform         xrRig;
    [SerializeField] float             movementSpeed  = 10f;
    [SerializeField] float             smoothing      = 5f;

    private bool isPinching = false;

    void OnEnable()
    {
        pinchAction.action.started  += _ => isPinching = true;
        pinchAction.action.canceled += _ => isPinching = false;
        pinchAction.action.Enable();
    }

    void OnDisable()
    {
        pinchAction.action.started  -= _ => isPinching = true;
        pinchAction.action.canceled -= _ => isPinching = false;
        pinchAction.action.Disable();
    }

    void Update()
    {
        if (isPinching)
            Move();
    }

    void Move()
    {
        // 1) Get the index‑tip joint pose
        if (!handSubsystem.TryGetHand(handNode, out XRHand hand)) return;
        if (!hand.TryGetJoint(XRHandJointID.IndexTip, out HandJoint tip)) return;

        // 2) Direction = index‑tip’s forward in world‑space
        Vector3 dir = tip.pose.forward.normalized;

        // 3) Compute smooth target
        Vector3 target = xrRig.position + dir * movementSpeed * Time.deltaTime;
        xrRig.position = Vector3.Lerp(xrRig.position, target, smoothing * Time.deltaTime);

        // 4) Debug
        Debug.Log($"[Travel] Moving along index tip – dir: {dir}");
    }
}
