using UnityEngine;
using UnityEngine.XR.Hands;
using Unity.XR.CoreUtils;
using System.Collections.Generic;

public class Travel : MonoBehaviour
{
    XRHandSubsystem handSubsystem;
    public XROrigin xrOrigin;
    public float moveSpeed = 1f;

    void Start()
    {
        // grab the hand subsystem
        var subs = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems<XRHandSubsystem>(subs);
        if (subs.Count > 0)
        {
            handSubsystem = subs[0];
            if (!handSubsystem.running) handSubsystem.Start();
        }
        else Debug.LogError("No XRHandSubsystem found");

        if (xrOrigin == null)
            xrOrigin = FindObjectOfType<XROrigin>();
    }

    void Update()
    {
        if (handSubsystem == null || !handSubsystem.running) return;
        var left = handSubsystem.leftHand;
        if (!left.isTracked) return;

        if (TryGetJointPose(left, XRHandJointID.IndexTip,     out var tipPose) &&
            TryGetJointPose(left, XRHandJointID.IndexProximal, out var proxPose))
        {
            Vector3 dir = (tipPose.position - proxPose.position).normalized;
            xrOrigin.transform.position += -dir * moveSpeed * Time.deltaTime;
        }
    }

    bool TryGetJointPose(XRHand hand, XRHandJointID id, out Pose pose)
    {
        var j = hand.GetJoint(id);
        if ((j.trackingState & XRHandJointTrackingState.Pose) != 0 && j.TryGetPose(out pose))
            return true;
        pose = default;
        return false;
    }
}