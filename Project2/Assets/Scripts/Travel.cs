using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.Subsystems;

public class Travel : MonoBehaviour
{
    [SerializeField] XRHandSubsystem handSubsystem;
    [SerializeField] XRNode            handNode = XRNode.LeftHand;
    [SerializeField] Transform         xrRig;
    [SerializeField] float             movementSpeed = 10f;
    [SerializeField] float             smoothing     = 5f;

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
        XRHand hand = handNode == XRNode.LeftHand
            ? handSubsystem.leftHand 
            : handSubsystem.rightHand;

        // Make sure it’s actually being tracked
        if (!hand.isTracked) return;

        // Grab the index‐tip joint
        XRHandJoint tipJoint = hand.GetJoint(XRHandJointID.IndexTip);
        if (!tipJoint.TryGetPose(out Pose tipPose)) return;

        // Move the rig along the tip’s forward direction
        Vector3 dir    = tipPose.forward.normalized;
        Vector3 target = xrRig.position + dir * movementSpeed * Time.deltaTime;
        xrRig.position = Vector3.Lerp(xrRig.position, target, smoothing * Time.deltaTime);

        // 4) Debug
        Debug.Log($"[Travel] Moving along index tip – dir: {dir}");
    }
}
