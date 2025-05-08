using UnityEngine;
using UnityEngine.XR.Hands;

public class HandTravel : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The Transform of your XR Rig (camera/rig root).")]
    [SerializeField] private Transform xrRig;

    [Header("Movement Settings")]
    [Tooltip("Speed in meters per second.")]
    [SerializeField] private float movementSpeed = 1.0f;
    [Tooltip("Max thumb–index distance (in meters) to register a pinch.")]
    [SerializeField] private float pinchThreshold = 0.025f;

    // XR Hands subsystem
    private XRHandSubsystem handSubsystem;

    private void OnEnable()
    {
        // Find and cache the hand-tracking subsystem
        handSubsystem = XRGeneralSettings.Instance.Manager.activeLoader
                         .GetLoadedSubsystem<XRHandSubsystem>();

        if (handSubsystem != null)
            handSubsystem.updatedHands += OnHandsUpdated;
        else
            Debug.LogWarning("No XRHandSubsystem found – make sure Hand Tracking is enabled in your XR plug-in settings.");
    }

    private void OnDisable()
    {
        if (handSubsystem != null)
            handSubsystem.updatedHands -= OnHandsUpdated;
    }

    private void OnHandsUpdated(XRHandSubsystem subsystem)
    {
        // Grab the left hand
        var left = handSubsystem.leftHand;

        // Try get the needed joints
        if (   left.TryGetJoint(XRHandJointID.IndexTip,      out var tip)
            && left.TryGetJoint(XRHandJointID.ThumbTip,      out var thumb)
            && left.TryGetJoint(XRHandJointID.IndexMetacarpal, out var meta))
        {
            // Measure thumb–index distance
            float dist = Vector3.Distance(tip.pose.position, thumb.pose.position);

            // If pinched, move along pointing direction
            if (dist < pinchThreshold)
                PerformMove(tip.pose.position, meta.pose.position);
        }
    }

    private void PerformMove(Vector3 tipPos, Vector3 metaPos)
    {
        // Direction from metacarpal to tip = finger pointing
        Vector3 dir = (tipPos - metaPos).normalized;
        xrRig.position += dir * movementSpeed * Time.deltaTime;
    }
}
