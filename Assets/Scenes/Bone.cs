using System;
using System.Collections.Generic;
using UnityEngine;

public class Bone 
{
    private readonly Transform _head;
    protected internal readonly Quaternion InitialRotation;
    protected internal readonly Bone Parent;
    
    public Bone(Transform head, Bone parent = null)
    {
        _head = head;
        InitialRotation = _head.rotation;
        Parent = parent;
    }

    public Quaternion Rotation
    {
        get => _head.rotation;
        set => _head.rotation = value;
    }

    public Vector3 Position => _head.position;
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
        if (_headID == default && _tailID == default)
        {
            base.UpdateRotation();
            return;
        }

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
        Vector3 neckPos = (_landmarkWrapper.PosOf(LandmarkID.RightShoulder) + _landmarkWrapper.PosOf(LandmarkID.LeftShoulder)) / 2;
        Vector3 hipsPos = (_landmarkWrapper.PosOf(LandmarkID.RightHip) + _landmarkWrapper.PosOf(LandmarkID.LeftHip)) / 2;
        
        Vector3 currentUpDown = (neckPos - hipsPos).normalized;
        Quaternion deltaRotUpDown = Quaternion.FromToRotation(_initialUpDown, currentUpDown);

        Vector3 currentTwist = (_landmarkWrapper.PosOf(LandmarkID.RightHip) - _landmarkWrapper.PosOf(LandmarkID.LeftHip)).normalized;
        Quaternion deltaRotTwist = Quaternion.FromToRotation(_initialTwist, currentTwist);
        Vector3 currentShoulderTwist = (_landmarkWrapper.PosOf(LandmarkID.RightShoulder) - _landmarkWrapper.PosOf(LandmarkID.LeftShoulder)).normalized;
        Quaternion deltaRotShoulderTwist = Quaternion.FromToRotation(_initialShoulderTwist, currentShoulderTwist);

        _hips.Rotation = deltaRotUpDown * deltaRotTwist * _initialHipRotation;
        _spine.Rotation = deltaRotUpDown * Quaternion.Slerp(deltaRotTwist, deltaRotShoulderTwist, 0.5f) * _initialSpineRotation;
        _chest.Rotation = deltaRotUpDown * deltaRotShoulderTwist * _initialChestRotation;
    }
}