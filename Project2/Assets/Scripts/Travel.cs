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
            if (handSubsystem == null || !handSubsystem.running) return;
            var left = handSubsystem.leftHand;
            if (!left.isTracked) return;

            if (TryGetJointPose(left, XRHandJointID.IndexTip, out var tipPose) &&
                TryGetJointPose(left, XRHandJointID.IndexProximal, out var proxPose))
            {
                Vector3 raw = tipPose.position - proxPose.position;
                Vector3 rawDir = raw.normalized;

                Vector3 horizDir = new Vector3(rawDir.x, 0f, rawDir.z);

                if (horizDir.sqrMagnitude > 0.001f)
                    horizDir = horizDir.normalized;
                else
                    horizDir = Vector3.zero;

                const float deadZone = 0.2f;
                float upDot   = Vector3.Dot(rawDir, Vector3.up);

                Vector3 vertDir;
                if (upDot > deadZone) {
                    // upward
                    vertDir = Vector3.up * (upDot - deadZone);
                }
                else if (upDot < -deadZone) {
                    // downward
                    vertDir = Vector3.down * ((-upDot) - deadZone);
                }
                else {
                    // dead zone
                    vertDir = Vector3.zero;
                }

                Vector3 movement = -(horizDir) + vertDir;
                xrOrigin.transform.position += movement * moveSpeed * Time.deltaTime;
                // var raw = tipPose.position - proxPose.position;

                // Vector3 horiz = new Vector3(raw.x, 0, raw.z).normalized;

                // Vector3 vert = new Vector3(0, raw.y, 0).normalized;

                // xrOrigin.transform.position += (-horiz * moveSpeed + vert * moveSpeed) * Time.deltaTime;

            }
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