using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.SubsystemManagement;
using System.Collections.Generic;

public class Travel : MonoBehaviour
{
    XRHandSubsystem handSubsystem;
    [Tooltip("Units per second in hand-forward direction")]
    public float moveSpeed = 1f;

    void Start()
    {
        // fetch any available hand‐tracking subsystem
        var subsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems<XRHandSubsystem>(subsystems);

        if (subsystems.Count > 0)
        {
            handSubsystem = subsystems[0];
            if (!handSubsystem.running)
                handSubsystem.Start();
        }
        else
        {
            Debug.LogError("XRHandSubsystem not found! Make sure 'XR Hands' is installed and Hand Tracking is enabled in your OpenXR settings.");
        }
    }

    void Update()
    {
        if (handSubsystem == null || !handSubsystem.running)
            return;

        var right = handSubsystem.rightHand;
        if (!right.isTracked)
            return;

        // get two joints
        if (TryGetJointPose(right, XRHandJointID.IndexTip, out var tipPose) &&
            TryGetJointPose(right, XRHandJointID.IndexProximal, out var proxPose))
        {
            Vector3 tip     = tipPose.position;
            Vector3 knuckle = proxPose.position;

            // direction = finger extension
            Vector3 dir = (tip - knuckle).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
    }

    bool TryGetJointPose(XRHand hand, XRHandJointID id, out Pose pose)
    {
        var joint = hand.GetJoint(id);
        // ensure we have a valid pose and tracking
        if ((joint.trackingState & XRHandJointTrackingState.Pose) != 0
            && joint.TryGetPose(out pose))
        {
            return true;
        }
        pose = default;
        return false;
    }
}