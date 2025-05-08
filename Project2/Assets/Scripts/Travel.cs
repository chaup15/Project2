using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.SubsystemManagement; // actually lives in UnityEngine
using Unity.XR.CoreUtils;              // for XROrigin
using System.Collections.Generic;

public class Travel : MonoBehaviour
{
    XRHandSubsystem handSubsystem;
    [Tooltip("Your Scene’s XROrigin")]
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

        // if you forgot to assign in Inspector:
        if (xrOrigin == null)
            xrOrigin = FindObjectOfType<XROrigin>();
    }

    void Update()
    {
        if (handSubsystem == null || !handSubsystem.running) return;
        var right = handSubsystem.rightHand;
        if (!right.isTracked) return;

        if (TryGetJointPose(right, XRHandJointID.IndexTip,     out var tipPose) &&
            TryGetJointPose(right, XRHandJointID.IndexProximal, out var proxPose))
        {
            Vector3 dir = (tipPose.position - proxPose.position).normalized;
            // **Move the XR Origin** so the camera follows
            xrOrigin.transform.position += dir * moveSpeed * Time.deltaTime;
        }
    }

    bool TryGetJointPose(XRHand hand, XRHandJointID id, out Pose pose)
    {
        var j = hand.GetJoint(id);
        if ((j.trackingState & XRHandJointTrackingState.Pose) != 0
            && j.TryGetPose(out pose))
            return true;
        pose = default;
        return false;
    }
}