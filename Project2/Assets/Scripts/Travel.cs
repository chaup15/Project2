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
            var right = handSubsystem.rightHand;
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

                float speedMultiplier = 1f;
                if (right.isTracked)
                {
                    // a) get wrist position
                    if (TryGetJointPose(right, XRHandJointID.Wrist, out var wristPose))
                    {
                        Vector3 wristPos = wristPose.position;

                        // b) list of fingertip joints to sample
                        XRHandJointID[] tips = {
                            XRHandJointID.IndexTip,
                            XRHandJointID.MiddleTip,
                            XRHandJointID.RingTip,
                            XRHandJointID.LittleTip,
                            XRHandJointID.ThumbTip
                        };

                        float totalDist = 0f;
                        int   sampleCnt = 0;
                        foreach (var id in tips)
                        {
                            var joint = right.GetJoint(id);
                            if ((joint.trackingState & XRHandJointTrackingState.Pose) != 0
                                && joint.TryGetPose(out var tipPose))
                            {
                                totalDist += Vector3.Distance(tipPose.position, wristPos);
                                sampleCnt++;
                            }
                        }

                        if (sampleCnt > 0)
                        {
                            float avgDist = totalDist / sampleCnt;

                            // c) calibrate closed vs open distances (you may need to tweak these)
                            const float closedDist = 0.02f;  // average fingertip–wrist when making a fist
                            const float openDist   = 0.10f;  // average when hand fully splayed

                            // d) normalize and clamp to [0,1]
                            speedMultiplier = Mathf.InverseLerp(closedDist, openDist, avgDist);
                        }
                    }
                }

                float currentSpeed = moveSpeed * speedMultiplier;
                xrOrigin.transform.position += movement * currentSpeed * Time.deltaTime;
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