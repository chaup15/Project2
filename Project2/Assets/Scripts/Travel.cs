using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.SubsystemsImplementation;
using System.Collections.Generic;

public class HandDirectionMover : MonoBehaviour
{
    XRHandSubsystem handSubsystem;
    public float moveSpeed;

    void Start()
    {
        var subsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetInstances(subsystems);
        if (subsystems.Count > 0)
        {
            handSubsystem = subsystems[0];
        }
    }

    void Update()
    {
        if (handSubsystem == null || !handSubsystem.running)
            return;

        XRHand rightHand = handSubsystem.rightHand;

        if (!rightHand.isTracked)
            return;

        var tipJoint = rightHand.GetJoint(XRHandJointID.IndexTip);
        var knuckleJoint = rightHand.GetJoint(XRHandJointID.IndexProximal);

        Vector3 tip = new Vector3();
        Vector3 knuckle = new Vector3();

        if (tipJoint.TryGetPose(out Pose pose1) && knuckleJoint.TryGetPose(out Pose pose2))
        {
            tip = pose1.position;
            knuckle = pose2.position;
        }

        if ((tipJoint.trackingState & XRHandJointTrackingState.Pose) == 0
           || (knuckleJoint.trackingState & XRHandJointTrackingState.Pose) == 0)
           return;

        Vector3 direction = (tip - knuckle).normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}