using UnityEngine;
using System.Collections.Generic;
using Mediapipe.Tasks.Components.Containers;
using Mediapipe.Tasks.Vision.PoseLandmarker;

public class LandmarkRig : MonoBehaviour
{
    private Animator animator;

    private enum LandmarkID
    {
        Nose = 0,
        LeftEyeInner = 1,
        LeftEye = 2,
        LeftEyeOuter = 3,
        RightEyeInner = 4,
        RightEye = 5,
        RightEyeOuter = 6,
        LeftEar = 7,
        RightEar = 8,
        MouthLeft = 9,
        MouthRight = 10,
        LeftShoulder = 11,
        RightShoulder = 12,
        LeftElbow = 13,
        RightElbow = 14,
        LeftWrist = 15,
        RightWrist = 16,
        LeftPinky = 17,
        RightPinky = 18,
        LeftIndex = 19,
        RightIndex = 20,
        LeftThumb = 21,
        RightThumb = 22,
        LeftHip = 23,
        RightHip = 24,
        LeftKnee = 25,
        RightKnee = 26,
        LeftAnkle = 27,
        RightAnkle = 28,
        LeftHeel = 29,
        RightHeel = 30,
        LeftFootIndex = 31,
        RightFootIndex = 32
    }

    private Landmarks landmarks;
    private Bone boneRightShoulder;
    private Bone boneRightUpperArm;
    private Bone boneRightLowerArm;
    private Bone boneLeftShoulder;
    private Bone boneLeftUpperArm;
    private Bone boneLeftLowerArm;
    private Bone boneRightUpperLeg;
    private Bone boneRightLowerLeg;
    private Bone boneRightFoot;
    private Bone boneLeftUpperLeg;
    private Bone boneLeftLowerLeg;
    private Bone boneLeftFoot;
    private Bone boneHead;
    private Vector3 initialUpDown;
    private Vector3 initialTwist;
    private Vector3 initialShoulderTwist;
    private Quaternion initialHipRotation;
    private Quaternion initialSpineRotation;
    private Quaternion initialChestRotation;

    void Start()
    {
        animator = GetComponent<Animator>();

        boneRightShoulder = new Bone(animator.GetBoneTransform(HumanBodyBones.RightShoulder), animator.GetBoneTransform(HumanBodyBones.RightUpperArm));
        boneRightUpperArm = new Bone(animator.GetBoneTransform(HumanBodyBones.RightUpperArm), animator.GetBoneTransform(HumanBodyBones.RightLowerArm));
        boneRightLowerArm = new Bone(animator.GetBoneTransform(HumanBodyBones.RightLowerArm), animator.GetBoneTransform(HumanBodyBones.RightHand));
        boneLeftShoulder = new Bone(animator.GetBoneTransform(HumanBodyBones.LeftShoulder), animator.GetBoneTransform(HumanBodyBones.LeftUpperArm));
        boneLeftUpperArm = new Bone(animator.GetBoneTransform(HumanBodyBones.LeftUpperArm), animator.GetBoneTransform(HumanBodyBones.LeftLowerArm));
        boneLeftLowerArm = new Bone(animator.GetBoneTransform(HumanBodyBones.LeftLowerArm), animator.GetBoneTransform(HumanBodyBones.LeftHand));
        boneRightUpperLeg = new Bone(animator.GetBoneTransform(HumanBodyBones.RightUpperLeg), animator.GetBoneTransform(HumanBodyBones.RightLowerLeg));
        boneRightLowerLeg = new Bone(animator.GetBoneTransform(HumanBodyBones.RightLowerLeg), animator.GetBoneTransform(HumanBodyBones.RightFoot));
        boneRightFoot = new Bone(animator.GetBoneTransform(HumanBodyBones.RightFoot), animator.GetBoneTransform(HumanBodyBones.RightToes));
        boneLeftUpperLeg = new Bone(animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg), animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
        boneLeftLowerLeg = new Bone(animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg), animator.GetBoneTransform(HumanBodyBones.LeftFoot));
        boneLeftFoot = new Bone(animator.GetBoneTransform(HumanBodyBones.LeftFoot), animator.GetBoneTransform(HumanBodyBones.LeftToes));
        boneHead = new Bone(animator.GetBoneTransform(HumanBodyBones.Neck), animator.GetBoneTransform(HumanBodyBones.Head));
        
        Transform neck = animator.GetBoneTransform(HumanBodyBones.Neck);
        Transform hips = animator.GetBoneTransform(HumanBodyBones.Hips);
        initialUpDown = (neck.position - hips.position).normalized;
        initialHipRotation = hips.rotation;
        initialSpineRotation = animator.GetBoneTransform(HumanBodyBones.Spine).rotation;
        initialChestRotation = animator.GetBoneTransform(HumanBodyBones.Chest).rotation;

        Transform rightUpperLeg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        Transform leftUpperLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        initialTwist = (rightUpperLeg.position - leftUpperLeg.position).normalized;
        
        Transform rightShoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        Transform leftShoulder = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
        initialShoulderTwist = (rightShoulder.position - leftShoulder.position).normalized;
    }    

    void Update()
    {
        if (landmarks.landmarks is null) return;
        
        boneRightUpperLeg.Update(LandmarkPos(LandmarkID.RightHip), LandmarkPos(LandmarkID.RightKnee), BonePresence(LandmarkID.RightHip, LandmarkID.RightKnee));
        boneRightLowerLeg.Update(LandmarkPos(LandmarkID.RightKnee), LandmarkPos(LandmarkID.RightAnkle), BonePresence(LandmarkID.RightHip, LandmarkID.RightAnkle));
        boneRightFoot.Update(LandmarkPos(LandmarkID.RightAnkle), LandmarkPos(LandmarkID.RightHeel), BonePresence(LandmarkID.RightHip, LandmarkID.RightHeel));
        boneLeftUpperLeg.Update(LandmarkPos(LandmarkID.LeftHip), LandmarkPos(LandmarkID.LeftKnee), BonePresence(LandmarkID.RightHip, LandmarkID.LeftKnee));
        boneLeftLowerLeg.Update(LandmarkPos(LandmarkID.LeftKnee), LandmarkPos(LandmarkID.LeftAnkle), BonePresence(LandmarkID.RightHip, LandmarkID.LeftAnkle));
        boneLeftFoot.Update(LandmarkPos(LandmarkID.LeftAnkle), LandmarkPos(LandmarkID.LeftHeel), BonePresence(LandmarkID.RightHip, LandmarkID.LeftHeel));   
        boneRightUpperArm.Update(LandmarkPos(LandmarkID.RightShoulder), LandmarkPos(LandmarkID.RightElbow), BonePresence(LandmarkID.RightHip, LandmarkID.RightElbow));
        boneRightLowerArm.Update(LandmarkPos(LandmarkID.RightElbow), LandmarkPos(LandmarkID.RightWrist), BonePresence(LandmarkID.RightHip, LandmarkID.RightWrist));
        boneLeftUpperArm.Update(LandmarkPos(LandmarkID.LeftShoulder), LandmarkPos(LandmarkID.LeftElbow), BonePresence(LandmarkID.RightHip, LandmarkID.LeftElbow));
        boneLeftLowerArm.Update(LandmarkPos(LandmarkID.LeftElbow), LandmarkPos(LandmarkID.LeftWrist), BonePresence(LandmarkID.RightHip, LandmarkID.LeftWrist));
        
        Vector3 neckPos = (LandmarkPos(LandmarkID.RightShoulder) + LandmarkPos(LandmarkID.LeftShoulder)) / 2;
        Vector3 hipsPos = (LandmarkPos(LandmarkID.RightHip) + LandmarkPos(LandmarkID.LeftHip)) / 2;
        
        boneHead.Update(neckPos, LandmarkPos(LandmarkID.Nose), true);

        Vector3 currentUpDown = (neckPos - hipsPos).normalized;
        // Vector3 currentUpDown = Vector3.up;
        Quaternion deltaRotUpDown = Quaternion.FromToRotation(initialUpDown, currentUpDown);

        Vector3 currentTwist = (LandmarkPos(LandmarkID.RightHip) - LandmarkPos(LandmarkID.LeftHip)).normalized;
        Quaternion deltaRotTwist = Quaternion.FromToRotation(initialTwist, currentTwist);
        Vector3 currentShoulderTwist = (LandmarkPos(LandmarkID.RightShoulder) - LandmarkPos(LandmarkID.LeftShoulder)).normalized;
        Quaternion deltaRotShoulderTwist = Quaternion.FromToRotation(initialShoulderTwist, currentShoulderTwist);
        
        animator.GetBoneTransform(HumanBodyBones.Hips).rotation = deltaRotUpDown * deltaRotTwist * initialHipRotation;
        animator.GetBoneTransform(HumanBodyBones.Spine).rotation = deltaRotUpDown * Quaternion.Slerp(deltaRotTwist, deltaRotShoulderTwist, 0.5f) * initialSpineRotation;
        animator.GetBoneTransform(HumanBodyBones.Chest).rotation = deltaRotUpDown * deltaRotShoulderTwist * initialChestRotation;
    }

    public void SetPose(PoseLandmarkerResult result)
    {
        var resultLandmarks  = result.poseWorldLandmarks[0];
        resultLandmarks.CloneTo(ref landmarks);
    }

    private Vector3 LandmarkPos(LandmarkID id)
    {
        var landmark = landmarks.landmarks[(int)id];
        var x = landmark.x;
        var y = -landmark.y;
        var z = landmark.z;
        return new Vector3(x, y, z);
    }
    
    private bool BonePresence(LandmarkID id1, LandmarkID id2, float threshold = 0.2f)
    {
        var p1 = landmarks.landmarks[(int)id1].presence > threshold;
        var p2 = landmarks.landmarks[(int)id2].presence > threshold;
        return p1 && p2;
    }

    private class Bone
    {
        public Transform parent;
        public Transform child;
        public Vector3 initialDir;
        public Quaternion initialRotation;
        public Bone(Transform parent, Transform child) 
        {
            this.parent = parent;
            this.child = child;
            initialDir = (child.position - parent.position).normalized;
            initialRotation = parent.rotation;
        }
        public void Update(Vector3 parentPos, Vector3 childPos, bool presence)
        {
            if (!presence)
            {
                parent.rotation = initialRotation;
                return;
            }
            Vector3 currentDir = (childPos - parentPos).normalized;
            Quaternion deltaRotTracked = Quaternion.FromToRotation(initialDir, currentDir);
            parent.rotation = deltaRotTracked * initialRotation;
        }
    } 
}

