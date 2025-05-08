using UnityEngine;
using UnityEngine.XR.Hands;
using Unity.XR.CoreUtils;
using System.Collections.Generic;

public class Travel : MonoBehaviour
{
    XRHandSubsystem handSubsystem;
    public XROrigin xrOrigin;
    public float moveSpeed = 1f;
    Timer timer;
    [SerializeField] GameObject canvas;

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

        timer = canvas.GetComponent<Timer>();
    }

    void Update()
    {
        if(timer.canMove)
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
                // displayTransform is some GameObject's Transform component
                tip = pose1.position;
                knuckle = pose2.position;
            }

            if ((tipJoint.trackingState & XRHandJointTrackingState.Pose) == 0
            || (knuckleJoint.trackingState & XRHandJointTrackingState.Pose) == 0)
            return;

            //Vector3 tip = tipJoint.;
            //Vector3 knuckle = knuckleJoint.pose.position;

            Vector3 direction = (tip - knuckle).normalized;
            // direction.y = 0; // Optional: restrict to horizontal movement

            // Move the object (e.g., XR Origin or camera rig)
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    // bool TryGetJointPose(XRHand hand, XRHandJointID id, out Pose pose)
    // {
    //     var j = hand.GetJoint(id);
    //     if ((j.trackingState & XRHandJointTrackingState.Pose) != 0 && j.TryGetPose(out pose))
    //         return true;
    //     pose = default;
    //     return false;
    // }
}