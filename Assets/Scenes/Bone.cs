using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Bone 
{
    private readonly Transform _head;
    protected internal readonly Quaternion InitialRotation;
    protected internal readonly Bone Parent;
    private Vector3 _angularVelocity = Vector3.zero;
    
    public Bone(Transform head, Bone parent = null)
    {
        _head = head;
        InitialRotation = _head.rotation;
        Parent = parent;
    }

    public Quaternion Rotation
    {
        get => _head.rotation;
        // set => _head.rotation = value;
        set => SmoothSetRotation(value);
    }

    public Vector3 Position => _head.position;

    private void SmoothSetRotation(Quaternion targetRotation)
    {
        var smoothTime = Time.deltaTime;
        var qDelta = targetRotation * Quaternion.Inverse(Rotation);
        qDelta.Normalize();
        qDelta.ToAngleAxis(out var angle, out var axis);
        var axisAngle = Vector3.SmoothDamp(Vector3.zero, angle * axis, ref _angularVelocity, smoothTime);
        qDelta = Quaternion.AngleAxis(axisAngle.magnitude, axisAngle.normalized);
        _head.rotation = qDelta * Rotation;
    }
}

public class BoneRotationHelper
{
    protected readonly LandmarkWrapper _landmarkWrapper;

    protected BoneRotationHelper(LandmarkWrapper landmarkWrapper)
    {
        _landmarkWrapper = landmarkWrapper;
    }
    
    public virtual void UpdateRotation()
    {
        throw new NotImplementedException();
    }
}

public class TwoLandmarkBoneHelper : BoneRotationHelper
{
    private readonly LandmarkID _headID, _tailID;
    private readonly Bone _bone;

    public TwoLandmarkBoneHelper(LandmarkWrapper landmarkWrapper, LandmarkID headID, LandmarkID tailID, Bone bone): base(landmarkWrapper)
    {
        _headID = headID;
        _tailID = tailID;
        _bone = bone;
    }

    public override void UpdateRotation()
    {
        var headPresence = _landmarkWrapper.PresenceOf(_headID, 0.8f);
        var tailPresence = _landmarkWrapper.PresenceOf(_headID, 0.8f);
        if (!headPresence || !tailPresence) return;
        
        var headPos = _landmarkWrapper.PosOf(_headID);
        var tailPos = _landmarkWrapper.PosOf(_tailID);
        var currentDir = (tailPos - headPos).normalized;

        var initialDir = _bone.InitialRotation * Vector3.up;
        var initialRotation = _bone.InitialRotation;
            
        if (_bone.Parent is not null)
        {
            var initialParentRotation = _bone.Parent.InitialRotation;
            var currentParentRotation = _bone.Parent.Rotation;
            var deltaRotParent = currentParentRotation * Quaternion.Inverse(initialParentRotation);
            initialDir = deltaRotParent * initialDir;
            initialRotation = deltaRotParent * initialRotation;
        }
            
        var deltaRotation = Quaternion.FromToRotation(initialDir, currentDir);
        _bone.Rotation = deltaRotation * initialRotation;
    }
}

public class BodyLandmarkBonesHelper : BoneRotationHelper
{
    private readonly Vector3 _initialUpDown;
    private readonly Quaternion _initialHipRotation;
    private readonly Quaternion _initialSpineRotation;
    private readonly Quaternion _initialChestRotation;
    private readonly Vector3 _initialTwist;
    private readonly Vector3 _initialShoulderTwist;

    private Bone _hips, _spine, _chest, _neck;
    private Bone _leftUpperLeg, _rightUpperLeg;
    private Bone _leftShoulder, _rightShoulder;
    
    public BodyLandmarkBonesHelper(
        LandmarkWrapper landmarkWrapper,
        Bone hips,
        Bone spine,
        Bone chest,
        Bone neck,
        Bone leftUpperLeg,
        Bone rightUpperLeg,
        Bone leftShoulder,
        Bone rightShoulder) : base(landmarkWrapper)
    {
        _hips = hips;
        _spine = spine;
        _chest = chest;
        _neck = neck;
        _leftUpperLeg = leftUpperLeg;
        _rightUpperLeg = rightUpperLeg;
        _leftShoulder = leftShoulder;
        _rightShoulder = rightShoulder;
        
        _initialUpDown = (neck.Position - hips.Position).normalized;
        _initialHipRotation = hips.Rotation;
        _initialSpineRotation = spine.Rotation;
        _initialChestRotation = chest.Rotation;
        _initialTwist = (rightUpperLeg.Position - leftUpperLeg.Position).normalized;
        _initialShoulderTwist = (rightShoulder.Position - leftShoulder.Position).normalized;
    }

    public override void UpdateRotation()
    {
        var rsPresence = _landmarkWrapper.PresenceOf(LandmarkID.RightShoulder, 0.8f);
        var rhPresence = _landmarkWrapper.PresenceOf(LandmarkID.RightHip, 0.8f);
        var lsPresence = _landmarkWrapper.PresenceOf(LandmarkID.LeftShoulder, 0.8f);
        var lhPresence = _landmarkWrapper.PresenceOf(LandmarkID.LeftHip, 0.8f);
        if (!rsPresence || !rhPresence || !lsPresence || !lhPresence ) return;
        
        var neckPos = (_landmarkWrapper.PosOf(LandmarkID.RightShoulder) + _landmarkWrapper.PosOf(LandmarkID.LeftShoulder)) / 2;
        var hipsPos = (_landmarkWrapper.PosOf(LandmarkID.RightHip) + _landmarkWrapper.PosOf(LandmarkID.LeftHip)) / 2;
        
        var currentUpDown = (neckPos - hipsPos).normalized;
        var deltaRotUpDown = Quaternion.FromToRotation(_initialUpDown, currentUpDown);

        var currentTwist = (_landmarkWrapper.PosOf(LandmarkID.RightHip) - _landmarkWrapper.PosOf(LandmarkID.LeftHip)).normalized;
        var deltaRotTwist = Quaternion.FromToRotation(_initialTwist, currentTwist);
        var currentShoulderTwist = (_landmarkWrapper.PosOf(LandmarkID.RightShoulder) - _landmarkWrapper.PosOf(LandmarkID.LeftShoulder)).normalized;
        var deltaRotShoulderTwist = Quaternion.FromToRotation(_initialShoulderTwist, currentShoulderTwist);

        _hips.Rotation = deltaRotUpDown * deltaRotTwist * _initialHipRotation;
        _spine.Rotation = deltaRotUpDown * Quaternion.Slerp(deltaRotTwist, deltaRotShoulderTwist, 0.5f) * _initialSpineRotation;
        _chest.Rotation = deltaRotUpDown * deltaRotShoulderTwist * _initialChestRotation;
    }
}

public class HeadRotationHelper : BoneRotationHelper
{
    private Bone _neck, _head;
    
    public HeadRotationHelper (LandmarkWrapper landmarkWrapper, Bone neck, Bone head): base(landmarkWrapper)
    {
        _neck = neck;
        _head = head;
    }

    public override void UpdateRotation()
    {
        var leftEarPos = _landmarkWrapper.PosOf(LandmarkID.LeftEar);
        var rightEarPos = _landmarkWrapper.PosOf(LandmarkID.RightEar);
        var centerEarPos = (leftEarPos + rightEarPos) / 2;
        var nosePos = _landmarkWrapper.PosOf(LandmarkID.Nose);
        
        var faceForward = (nosePos - centerEarPos).normalized;
        var faceUp = Vector3.Cross( leftEarPos - rightEarPos, nosePos - centerEarPos).normalized;
        var faceRotation = Quaternion.LookRotation(faceForward, faceUp);
        
        _neck.Rotation = faceRotation;
    }
}