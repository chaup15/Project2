using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Hands;
using UnityEngine.SubsystemsImplementation;

public class HandGestureFlySmooth : MonoBehaviour
{
    [SerializeField] XRNode handNode = XRNode.RightHand;
    [SerializeField] Transform xrRig;
    [SerializeField] float movementSpeed = 3f;
    [SerializeField] float acceleration = 5f;
    [SerializeField] float damping = 3f;

    private XRHandSubsystem handSubsystem;
    private Vector3 currentVelocity = Vector3.zero;
    private bool isPinching = false;

    void Start()
    {
        // Get the XRHandSubsystem instance
        List<XRHandSubsystem> subsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetInstances(subsystems);
        if (subsystems.Count > 0)
        {
            handSubsystem = subsystems[0];
        }
        else
        {
            Debug.LogWarning("No XRHandSubsystem found. Make sure XR Hands is enabled in XR Plugin Management.");
        }
    }

    void Update()
    {
        if (handSubsystem == null || !handSubsystem.running)
            return;

        XRHand hand = (handNode == XRNode.RightHand) ? handSubsystem.rightHand : handSubsystem.leftHand;
        if (!hand.isTracked) return;

        isPinching = IsPinching(hand);
        if (isPinching)
        {
            MoveTowardFingerDirection(hand);
        }
        else if (currentVelocity.magnitude > 0.01f)
        {
            ApplyDamping();
        }
    }

    bool IsPinching(XRHand hand)
    {
        if (!hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose indexTipPose)) return false;
        if (!hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out Pose thumbTipPose)) return false;

        float pinchDistance = Vector3.Distance(indexTipPose.position, thumbTipPose.position);
        return pinchDistance < 0.03f;  // You can tweak this threshold
    }

    void MoveTowardFingerDirection(XRHand hand)
    {
        if (hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose tipPose) &&
            hand.GetJoint(XRHandJointID.IndexIntermediate).TryGetPose(out Pose midPose))
        {
            Vector3 targetDirection = (tipPose.position - midPose.position).normalized;
            Vector3 targetVelocity = targetDirection * movementSpeed;

            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
            xrRig.position += currentVelocity * Time.deltaTime;
        }
    }

    void ApplyDamping()
    {
        currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, damping * Time.deltaTime);
        xrRig.position += -currentVelocity * Time.deltaTime;
    }
}